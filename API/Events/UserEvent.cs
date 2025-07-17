namespace API.Events;

// Rastreia comportamento do usuário (cliques, navegação)
public class UserEvent
{
    public string UserId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty; // "page_view", "product_click", "search", etc.
    public string ProductId { get; set; } = string.Empty;
    public string PageUrl { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object>? AdditionalData { get; set; }
}
