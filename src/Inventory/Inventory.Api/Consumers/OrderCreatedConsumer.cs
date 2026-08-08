using System.Text.Json;
using Confluent.Kafka;
using Contracts;
using Contracts.Consumer;
using Inventory.Application.Commands.ReserveStock;
using MediatR;
using Shared.Implementations.Event;

namespace Inventory.Api.Consumers;

public class OrderCreatedConsumer(
    IKafkaConsumerFactory kafkaConsumerFactory,
    IServiceProvider serviceProvider,
    ILogger<OrderCreatedConsumer> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var consumer = kafkaConsumerFactory.Create("order-created");
        consumer.Subscribe(Topics.OrderCreated);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);
                var @event = JsonSerializer.Deserialize<OrderCreatedEvent>(result.Message.Value)
                             ?? throw new Exception("OrderCreatedEvent is null");

                logger.LogInformation("Received OrderCreatedEvent for OrderId {OrderId}", @event.OrderId);

                using var scope = serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                await mediator.Send(new ReserveStockCommand(@event.ProductId, @event.OrderId, @event.Quantity),
                    stoppingToken);

                consumer.Commit(result);
            }
            catch (ConsumeException ex)
            {
                logger.LogWarning(ex, "Error consuming from topic {Topic}, retrying", Topics.OrderCreated);
                await Task.Delay(2000, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}