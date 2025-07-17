namespace API.Events;

// Rastreia buscas dos usuários
public class SearchEvent
{
    public string UserId { get; set; } = string.Empty;
    public string SearchTerm { get; set; } = string.Empty;
    public string? Category { get; set; }
    public int ResultsCount { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object>? AdditionalData { get; set; }
}
