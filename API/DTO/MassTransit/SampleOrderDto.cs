namespace API.DTO.MassTransit;

public class SampleOrderDto
{
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public List<SampleOrderItemDto> Items { get; set; } = new();
}
