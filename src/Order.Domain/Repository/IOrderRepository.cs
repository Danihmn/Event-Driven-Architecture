using Contracts.Infra.Repository;

namespace Order.Domain.Repository;

public interface IOrderRepository : IRepository<Entities.Order>;