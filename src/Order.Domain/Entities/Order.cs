using Contracts.Domain;
using Order.Domain.Enums;

namespace Order.Domain.Entities;

public class Order : Entity
{
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public EOrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Order(Guid productId, int quantity)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Quantity = quantity;
        Status = EOrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void Confirm() => Status = EOrderStatus.Confirmed;
    public void Cancel() => Status = EOrderStatus.Cancelled;
}