using OrderManagement.Domain.Entities;

namespace OrderManagement.Domain.Interfaces;

public interface IOrderRepository
{
    Task<bool> AllProductsExistAsync(IEnumerable<int> productIds, CancellationToken cancellationToken = default);
    Task<OrderHeader> CreateAsync(OrderHeader orderHeader, CancellationToken cancellationToken = default);
}
