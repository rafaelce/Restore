namespace API.DTO.MassTransit;

// Mensagem quando pedido é criado
public class OrderCreatedMessage
{
    public int OrderId { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderItemMessage> Items { get; set; } = new();
}
