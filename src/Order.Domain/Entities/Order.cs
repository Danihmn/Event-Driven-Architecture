using Order.Domain.Enums;

namespace Order.Domain.Entities;

public class Order(int quantity)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public int Quantity { get; private set; } = quantity;
    public EOrderStatus Status { get; private set; } = EOrderStatus.Pending;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public void Confirm() => Status = EOrderStatus.Confirmed;
    public void Cancel() => Status = EOrderStatus.Cancelled;
}