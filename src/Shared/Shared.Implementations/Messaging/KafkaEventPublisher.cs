using System.Text.Json;
using Confluent.Kafka;
using Contracts.Event.Abstractions;
using Contracts.Infra.Event;

namespace Shared.Implementations.Messaging;

public class KafkaEventPublisher(IProducer<string, string> producer) : IEventPublisher
{
    public async Task PublishAsync<TEvent>(string topic, TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : Event
        => await producer.ProduceAsync(topic, new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = JsonSerializer.Serialize(@event)
        }, cancellationToken);
}