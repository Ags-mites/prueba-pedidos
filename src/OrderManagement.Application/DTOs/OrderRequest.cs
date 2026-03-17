using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Application.DTOs;

public class OrderRequest
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "ClienteId debe ser mayor a 0.")]
    public int ClienteId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Usuario { get; set; } = string.Empty;

    [Required]
    [MinLength(1, ErrorMessage = "Debe incluir al menos un item.")]
    public List<OrderItemRequest> Items { get; set; } = new();
}
