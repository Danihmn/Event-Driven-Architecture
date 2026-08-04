using FluentResults;

namespace Inventory.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public int QuantityAvailable { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public Product(string name, decimal price, int quantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        if (name.Length > 50)
            throw new ArgumentException("Name must be at most 50 characters long.", nameof(name));

        if (price < 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");

        if (quantity < 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity cannot be negative.");

        Name = name;
        Price = price;
        QuantityAvailable = quantity;
    }

    public Result Reserve(int quantity)
    {
        if (quantity <= 0)
            return Result.Fail("Quantity must be greater than zero.");

        if (quantity > QuantityAvailable)
            return Result.Fail("Insufficient stock available.");

        QuantityAvailable -= quantity;
        return Result.Ok();
    }

    public void Restock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");

        QuantityAvailable += quantity;
    }
}