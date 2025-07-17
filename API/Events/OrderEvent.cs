namespace API.Events;

// Rastreia eventos relacionados a pedidos
public class OrderEvent
{
    public string OrderId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty; // "order_created", "order_paid", "order_shipped"
    public decimal TotalAmount { get; set; }
    public List<string> ProductIds { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object>? AdditionalData { get; set; }
}
