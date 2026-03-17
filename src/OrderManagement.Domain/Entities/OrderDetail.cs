namespace OrderManagement.Domain.Entities;

public class OrderDetail
{
    public int Id { get; set; }
    public int OrderHeaderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public OrderHeader? OrderHeader { get; set; }
    public Product? Product { get; set; }
}
