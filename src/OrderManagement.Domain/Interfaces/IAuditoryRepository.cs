using OrderManagement.Domain.Entities;

namespace OrderManagement.Domain.Interfaces;

public interface IAuditoryRepository
{
    Task CreateIsolatedAsync(LogAuditory logAuditory, CancellationToken cancellationToken = default);
}
