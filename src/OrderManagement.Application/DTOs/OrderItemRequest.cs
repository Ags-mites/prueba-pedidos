using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Application.DTOs;

public class OrderItemRequest
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "ProductoId debe ser mayor a 0.")]
    public int ProductoId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Cantidad debe ser mayor a 0.")]
    public int Cantidad { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Precio debe ser mayor a 0.")]
    public decimal Precio { get; set; }
}
