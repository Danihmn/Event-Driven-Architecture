using Contracts.Infra.Repository;
using Inventory.Domain.Entities;

namespace Inventory.Domain.Repository;

public interface IProductRepository : IRepository<Product>;