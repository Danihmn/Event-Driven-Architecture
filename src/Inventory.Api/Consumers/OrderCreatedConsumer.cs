using System.Text.Json;
using Confluent.Kafka;
using Contracts;
using Contracts.Event;
using Inventory.Application.Commands.ReserveStock;
using MediatR;

namespace Inventory.Api.Consumers;

public class OrderCreatedConsumer(IConsumer<string, string> consumer, IServiceProvider serviceProvider)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        consumer.Subscribe(Topics.OrderCreated);

        while (!stoppingToken.IsCancellationRequested)
        {
            var result = consumer.Consume(stoppingToken);
            var @event = JsonSerializer.Deserialize<OrderCreatedEvent>(result.Message.Value)
                         ?? throw new Exception("OrderCreatedEvent is null");

            using var scope = serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Send(new ReserveStockCommand(@event.ProductId, @event.Quantity));

            consumer.Commit(result);
        }
    }
}