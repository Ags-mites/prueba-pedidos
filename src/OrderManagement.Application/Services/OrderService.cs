using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Interfaces;
using OrderManagement.Application.Mappings;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;
using OrderManagement.Infrastructure.Data;

namespace OrderManagement.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IAuditoryRepository _auditoryRepository;
    private readonly IExternalValidationService _validationService;
    private readonly AppDbContext _context;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orderRepository,
        IAuditoryRepository auditoryRepository,
        IExternalValidationService validationService,
        AppDbContext context,
        ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _auditoryRepository = auditoryRepository;
        _validationService = validationService;
        _context = context;
        _logger = logger;
    }

    public async Task<OrderResponse> CreateOrderAsync(OrderRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Iniciando registro de pedido para ClienteId: {ClienteId}", request.ClienteId);

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await _auditoryRepository.CreateIsolatedAsync(new LogAuditory
            {
                Date = DateTime.UtcNow,
                Event = "INICIO",
                Description = $"Inicio de registro de pedido para ClienteId: {request.ClienteId}, Usuario: {request.Usuario}"
            }, cancellationToken);

            _logger.LogInformation("Validando cliente {ClienteId} con servicio externo", request.ClienteId);

            var clienteValido = await _validationService.ValidateClientAsync(request.ClienteId, cancellationToken);

            if (!clienteValido)
            {
                var msg = $"El cliente {request.ClienteId} no superó la validación externa.";
                _logger.LogWarning(msg);

                await _auditoryRepository.CreateIsolatedAsync(new LogAuditory
                {
                    Date = DateTime.UtcNow,
                    Event = "ERROR_VALIDACION",
                    Description = msg
                }, cancellationToken);

                await transaction.RollbackAsync(cancellationToken);
                throw new InvalidOperationException(msg);
            }

            var productIds = request.Items.Select(i => i.ProductoId);
            var allProductsExist = await _orderRepository.AllProductsExistAsync(productIds, cancellationToken);
            if (!allProductsExist)
            {
                var msg = "Uno o más productos no existen en la tabla Products.";
                _logger.LogWarning(msg);

                await _auditoryRepository.CreateIsolatedAsync(new LogAuditory
                {
                    Date = DateTime.UtcNow,
                    Event = "ERROR_DATOS",
                    Description = msg
                }, cancellationToken);

                await transaction.RollbackAsync(cancellationToken);
                throw new InvalidOperationException(msg);
            }

            var order = request.ToEntity();

            var created = await _orderRepository.CreateAsync(order, cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            try
            {
                await _auditoryRepository.CreateIsolatedAsync(new LogAuditory
                {
                    Date = DateTime.UtcNow,
                    Event = "EXITO",
                    Description = $"Pedido registrado exitosamente. Id: {created.Id}, Total: {created.Total}"
                }, cancellationToken);
            }
            catch (Exception auditEx)
            {
                _logger.LogError(auditEx, "No se pudo registrar log de auditoría EXITO para pedido {OrderId}", created.Id);
            }

            _logger.LogInformation("Pedido {OrderId} registrado exitosamente", created.Id);

            return created.ToResponse();
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar pedido para ClienteId: {ClienteId}", request.ClienteId);

            try
            {
                await transaction.RollbackAsync(cancellationToken);

                await _auditoryRepository.CreateIsolatedAsync(new LogAuditory
                {
                    Date = DateTime.UtcNow,
                    Event = "ERROR",
                    Description = $"Error inesperado: {ex.Message}"
                }, cancellationToken);
            }
            catch (Exception auditEx)
            {
                _logger.LogError(auditEx, "No se pudo registrar el log de auditoría del error");
            }

            throw;
        }
    }

}
