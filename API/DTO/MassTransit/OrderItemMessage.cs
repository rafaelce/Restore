namespace API.DTO.MassTransit;

// DTO para itens do pedido
public class OrderItemMessage
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
