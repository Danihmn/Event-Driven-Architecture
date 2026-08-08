using System.Text.Json;
using Confluent.Kafka;
using Contracts.Infra.EventPublisher;

namespace Shared.Implementations.EventPublisher;

public class KafkaEventPublisher(IProducer<string, string> producer) : IEventPublisher
{
    public async Task PublishAsync<TEvent>(string topic, TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : Contracts.Event
        => await producer.ProduceAsync(topic, new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = JsonSerializer.Serialize(@event)
        }, cancellationToken);
}