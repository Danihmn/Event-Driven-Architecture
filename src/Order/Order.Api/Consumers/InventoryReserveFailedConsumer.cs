using System.Text.Json;
using Confluent.Kafka;
using Contracts;
using MediatR;
using Order.Application.Commands.CancelOrder;
using Shared.Implementations.Event;

namespace Order.Api.Consumers;

public class InventoryReserveFailedConsumer(IConsumer<string, string> consumer, IServiceProvider serviceProvider)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        consumer.Subscribe(Topics.StockReservationFailed);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);
                var @event = JsonSerializer.Deserialize<InventoryReserveFailEvent>(result.Message.Value)
                             ?? throw new Exception("InventoryReserveFailEvent is null");

                using var scope = serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                await mediator.Send(new CancelOrderCommand(@event.OrderId), stoppingToken);

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