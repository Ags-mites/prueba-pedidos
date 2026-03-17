using OrderManagement.Application.DTOs;

namespace OrderManagement.Application.Interfaces;

public interface IOrderService
{
    Task<OrderResponse> CreateOrderAsync(OrderRequest request, CancellationToken cancellationToken = default);
}
