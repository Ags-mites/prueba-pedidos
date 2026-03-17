namespace OrderManagement.Application.DTOs;

public class OrderResponse
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public List<OrderDetailResponse> Items { get; set; } = new();
}

public class OrderDetailResponse
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal Precio { get; set; }
    public decimal Subtotal => Cantidad * Precio;
}
