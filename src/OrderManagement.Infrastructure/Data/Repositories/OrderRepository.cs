using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;

namespace OrderManagement.Infrastructure.Data.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AllProductsExistAsync(IEnumerable<int> productIds, CancellationToken cancellationToken = default)
    {
        var ids = productIds.Distinct().ToList();
        var existingCount = await _context.Products.CountAsync(p => ids.Contains(p.Id), cancellationToken);
        return existingCount == ids.Count;
    }

    public async Task<OrderHeader> CreateAsync(OrderHeader orderHeader, CancellationToken cancellationToken = default)
    {
        await _context.OrderHeaders.AddAsync(orderHeader, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return orderHeader;
    }
}
