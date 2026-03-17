namespace OrderManagement.Application.Interfaces;

public interface IExternalValidationService
{
    Task<bool> ValidateClientAsync(int clientId, CancellationToken cancellationToken = default);
}
