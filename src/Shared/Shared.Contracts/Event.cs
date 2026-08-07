namespace Contracts;

public abstract record Event
{
    protected Guid EventId;
    protected DateTime OccurredAt;
}