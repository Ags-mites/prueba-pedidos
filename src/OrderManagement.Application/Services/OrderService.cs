using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Interfaces;
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
            await _auditoryRepository.CreateAsync(new LogAuditory
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

                await _auditoryRepository.CreateAsync(new LogAuditory
                {
                    Date = DateTime.UtcNow,
                    Event = "ERROR_VALIDACION",
                    Description = msg
                }, cancellationToken);

                await transaction.RollbackAsync(cancellationToken);
                throw new InvalidOperationException(msg);
            }

            var total = request.Items.Sum(i => i.Cantidad * i.Precio);

            var order = new OrderHeader
            {
                ClientId = request.ClienteId,
                UserName = request.Usuario,
                Date = DateTime.UtcNow,
                Total = total,
                Details = request.Items.Select(i => new OrderDetail
                {
                    ProductId = i.ProductoId,
                    Quantity = i.Cantidad,
                    Price = i.Precio
                }).ToList()
            };

            var created = await _orderRepository.CreateAsync(order, cancellationToken);

            await _auditoryRepository.CreateAsync(new LogAuditory
            {
                Date = DateTime.UtcNow,
                Event = "EXITO",
                Description = $"Pedido registrado exitosamente. Id: {created.Id}, Total: {created.Total}"
            }, cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Pedido {OrderId} registrado exitosamente", created.Id);

            return MapToResponse(created);
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
                await _auditoryRepository.CreateAsync(new LogAuditory
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

            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static OrderResponse MapToResponse(OrderHeader order) => new()
    {
        Id = order.Id,
        ClienteId = order.ClientId,
        Usuario = order.UserName,
        Fecha = order.Date,
        Total = order.Total,
        Items = order.Details.Select(d => new OrderDetailResponse
        {
            ProductoId = d.ProductId,
            Cantidad = d.Quantity,
            Precio = d.Price
        }).ToList()
    };
}
