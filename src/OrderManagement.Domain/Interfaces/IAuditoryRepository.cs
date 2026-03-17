using OrderManagement.Domain.Entities;

namespace OrderManagement.Domain.Interfaces;

public interface IAuditoryRepository
{
    Task CreateAsync(LogAuditory logAuditory, CancellationToken cancellationToken = default);
    Task CreateIsolatedAsync(LogAuditory logAuditory, CancellationToken cancellationToken = default);
}
