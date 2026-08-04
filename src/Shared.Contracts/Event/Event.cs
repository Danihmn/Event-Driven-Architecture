namespace Contracts.Event;

public abstract record Event
{
    protected Guid EventId;
    protected DateTime OccurredAt;
}