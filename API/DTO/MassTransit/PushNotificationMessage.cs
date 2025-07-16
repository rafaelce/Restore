namespace API.DTO.MassTransit;

// Mensagem para notificação push
public class PushNotificationMessage
{
    public string CustomerId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
}
