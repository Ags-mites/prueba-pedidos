using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;

namespace OrderManagement.Infrastructure.Data.Repositories;

public class AuditoryRepository : IAuditoryRepository
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public AuditoryRepository(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task CreateIsolatedAsync(LogAuditory logAuditory, CancellationToken cancellationToken = default)
    {
        await using var isolatedContext = await _contextFactory.CreateDbContextAsync(cancellationToken);
        await isolatedContext.LogAuditory.AddAsync(logAuditory, cancellationToken);
        await isolatedContext.SaveChangesAsync(cancellationToken);
    }
}
