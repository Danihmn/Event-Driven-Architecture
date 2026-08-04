namespace Contracts.Infra.Event;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(string topic, TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : Contracts.Event.Event;
}