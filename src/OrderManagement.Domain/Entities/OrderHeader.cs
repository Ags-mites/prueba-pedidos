namespace OrderManagement.Domain.Entities;

public class OrderHeader
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public DateTime Date { get; set; }
    public decimal Total { get; set; }
    public string UserName { get; set; } = string.Empty;

    public ICollection<OrderDetail> Details { get; set; } = new List<OrderDetail>();
}
