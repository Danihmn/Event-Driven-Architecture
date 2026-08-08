using System.Text.Json;
using Confluent.Kafka;
using Contracts;
using Contracts.EventConsumer;
using MediatR;
using Order.Application.Commands.ConfirmOrder;
using Shared.Implementations.Event;

namespace Order.Api.Consumers;

public class InventoryReservedConsumer(
    IKafkaConsumerFactory kafkaConsumerFactory,
    IServiceProvider serviceProvider,
    ILogger<InventoryReservedConsumer> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var consumer = kafkaConsumerFactory.Create("inventory-reserved");
        consumer.Subscribe(Topics.StockReserved);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);
                var @event = JsonSerializer.Deserialize<InventoryReservedEvent>(result.Message.Value)
                             ?? throw new Exception("InventoryReservedEvent is null");

                logger.LogInformation("Received InventoryReservedEvent for OrderId {OrderId}", @event.OrderId);

                using var scope = serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                await mediator.Send(new ConfirmOrderCommand(@event.OrderId), stoppingToken);

                consumer.Commit(result);
            }
            catch (ConsumeException ex)
            {
                logger.LogWarning(ex, "Error consuming from topic {Topic}, retrying", Topics.StockReserved);
                await Task.Delay(2000, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}