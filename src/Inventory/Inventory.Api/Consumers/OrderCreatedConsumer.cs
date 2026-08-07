using System.Text.Json;
using Confluent.Kafka;
using Contracts;
using Inventory.Application.Commands.ReserveStock;
using MediatR;
using Shared.Implementations.Event;

namespace Inventory.Api.Consumers;

public class OrderCreatedConsumer(IConsumer<string, string> consumer, IServiceProvider serviceProvider)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        consumer.Subscribe(Topics.OrderCreated);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);
                var @event = JsonSerializer.Deserialize<OrderCreatedEvent>(result.Message.Value)
                             ?? throw new Exception("OrderCreatedEvent is null");

                using var scope = serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                await mediator.Send(new ReserveStockCommand(@event.ProductId, @event.OrderId, @event.Quantity),
                    stoppingToken);

                consumer.Commit(result);
            }
            catch (ConsumeException)
            {
                await Task.Delay(2000, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}