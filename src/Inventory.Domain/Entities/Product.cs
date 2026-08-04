namespace Inventory.Domain.Entities;

public class Product(string name, decimal price, int quantity)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = name;
    public decimal Price { get; private set; } = price;
    public int QuantityAvailable { get; private set; } = quantity;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}