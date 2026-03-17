namespace OrderManagement.Domain.Entities;

public class LogAuditory
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Event { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
