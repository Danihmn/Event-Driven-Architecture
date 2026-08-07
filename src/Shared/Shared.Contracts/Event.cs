namespace Contracts.Event.Abstractions;

public abstract record Event
{
    protected Guid EventId;
    protected DateTime OccurredAt;
}