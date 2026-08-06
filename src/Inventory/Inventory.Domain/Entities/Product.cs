using FluentResults;

namespace Inventory.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public int QuantityAvailable { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private Product(string name, decimal price, int quantityAvailable)
    {
        Name = name;
        Price = price;
        QuantityAvailable = quantityAvailable;
    }

    public static Result<Product> Create(string name, decimal price, int quantityAvailable)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Fail<Product>("Name is required.");

        if (name.Length > 50)
            return Result.Fail<Product>("Name must be at most 50 characters long.");

        if (price < 0)
            return Result.Fail<Product>("Price cannot be negative.");

        if (quantityAvailable < 0)
            return Result.Fail<Product>("Quantity cannot be negative.");

        return Result.Ok(new Product(name, price, quantityAvailable));
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