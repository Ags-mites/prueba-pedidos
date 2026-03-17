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

    public async Task<IReadOnlyList<OrderHeader>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.OrderHeaders
            .Include(o => o.Details)
            .ThenInclude(d => d.Product)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<OrderHeader?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.OrderHeaders
            .Include(o => o.Details)
            .ThenInclude(d => d.Product)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<OrderHeader> CreateAsync(OrderHeader orderHeader, CancellationToken cancellationToken = default)
    {
        await _context.OrderHeaders.AddAsync(orderHeader, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return orderHeader;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await _context.OrderHeaders
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

        if (order is null)
        {
            return false;
        }

        _context.OrderHeaders.Remove(order);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
