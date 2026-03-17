using OrderManagement.Application.DTOs;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Mappings;

public static class OrderMappingExtensions
{
    public static OrderHeader ToEntity(this OrderRequest request)
    {
        return new OrderHeader
        {
            ClientId = request.ClienteId,
            UserName = request.Usuario,
            Date = DateTime.UtcNow,
            Total = request.Items.Sum(i => i.Cantidad * i.Precio),
            Details = request.Items.Select(i => new OrderDetail
            {
                ProductId = i.ProductoId,
                Quantity = i.Cantidad,
                Price = i.Precio
            }).ToList()
        };
    }

    public static OrderResponse ToResponse(this OrderHeader order)
    {
        return new OrderResponse
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
}
