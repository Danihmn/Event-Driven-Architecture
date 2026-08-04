using Contracts.Domain;

namespace Inventory.Domain.Entities;

public class Product : Entity
{
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public int QuantityAvailable { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Product(string name, decimal price, int quantity)
    {
        Id = Guid.NewGuid();
        Name = name;
        Price = price;
        QuantityAvailable = quantity;
        CreatedAt = DateTime.UtcNow;
    }
}