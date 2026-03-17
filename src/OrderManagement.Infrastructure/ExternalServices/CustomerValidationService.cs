using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OrderManagement.Application.Interfaces;

namespace OrderManagement.Infrastructure.ExternalServices;

public class CustomerValidationService : IExternalValidationService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CustomerValidationService> _logger;

    public CustomerValidationService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<CustomerValidationService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> ValidateClientAsync(int clientId, CancellationToken cancellationToken = default)
    {
        var baseUrl = _configuration["ExternalServices:ValidationUrl"]
            ?? "https://jsonplaceholder.typicode.com/users/1";

        try
        {
            _logger.LogInformation("Consultando servicio externo de validación: {Url}", baseUrl);

            var response = await _httpClient.GetAsync(baseUrl, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Validación externa exitosa para ClienteId: {ClienteId}", clientId);
                return true;
            }

            _logger.LogWarning(
                "Servicio externo respondió con código {StatusCode} para ClienteId: {ClienteId}",
                (int)response.StatusCode, clientId);

            return false;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error de conexión con el servicio externo de validación");
            throw new ApplicationException("No se pudo conectar con el servicio externo de validación.", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout al conectar con el servicio externo de validación");
            throw new ApplicationException("El servicio externo de validación no respondió a tiempo.", ex);
        }
    }
}
