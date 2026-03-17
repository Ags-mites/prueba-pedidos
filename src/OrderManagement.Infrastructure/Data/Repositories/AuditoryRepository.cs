using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;

namespace OrderManagement.Infrastructure.Data.Repositories;

public class AuditoryRepository : IAuditoryRepository
{
    private readonly AppDbContext _context;

    public AuditoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(LogAuditory logAuditory, CancellationToken cancellationToken = default)
    {
        await _context.LogAuditory.AddAsync(logAuditory, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
