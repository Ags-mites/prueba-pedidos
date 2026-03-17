using OrderManagement.Domain.Entities;

namespace OrderManagement.Domain.Interfaces;

public interface IOrderRepository
{
    Task<IReadOnlyList<OrderHeader>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<OrderHeader?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<OrderHeader> CreateAsync(OrderHeader orderHeader, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
