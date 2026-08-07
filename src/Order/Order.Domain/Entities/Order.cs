using FluentResults;
using Order.Domain.Enums;

namespace Order.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public EOrderStatus Status { get; private set; } = EOrderStatus.Pending;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private Order(Guid productId, int quantity)
    {
        ProductId = productId;
        Quantity = quantity;
    }

    public static Result<Order> Create(Guid productId, int quantity)
    {
        if (productId == Guid.Empty)
            return Result.Fail<Order>("ProductId is required.");

        return quantity <= 0
            ? Result.Fail<Order>("Quantity must be greater than zero.")
            : Result.Ok(new Order(productId, quantity));
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