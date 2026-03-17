using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;

namespace OrderManagement.Infrastructure.Data.Repositories;

public class AuditoryRepository : IAuditoryRepository
{
    private readonly AppDbContext _context;
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public AuditoryRepository(AppDbContext context, IDbContextFactory<AppDbContext> contextFactory)
    {
        _context = context;
        _contextFactory = contextFactory;
    }

    public async Task CreateAsync(LogAuditory logAuditory, CancellationToken cancellationToken = default)
    {
        await _context.LogAuditory.AddAsync(logAuditory, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateIsolatedAsync(LogAuditory logAuditory, CancellationToken cancellationToken = default)
    {
        await using var isolatedContext = await _contextFactory.CreateDbContextAsync(cancellationToken);
        await isolatedContext.LogAuditory.AddAsync(logAuditory, cancellationToken);
        await isolatedContext.SaveChangesAsync(cancellationToken);
    }
}
