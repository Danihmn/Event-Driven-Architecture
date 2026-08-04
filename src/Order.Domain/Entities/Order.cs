using Order.Domain.Enums;

namespace Order.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public int Quantity { get; private set; }
    public EOrderStatus Status { get; private set; } = EOrderStatus.Pending;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public Order(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");

        Quantity = quantity;
    }

    public void Confirm()
    {
        if (Status != EOrderStatus.Pending)
            throw new InvalidOperationException($"Cannot confirm an order in status {Status}.");

        Status = EOrderStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status == EOrderStatus.Cancelled)
            throw new InvalidOperationException("Order is already cancelled.");

        Status = EOrderStatus.Cancelled;
    }
}