using API.DTO.MassTransit;
using MassTransit;

namespace API.Consumers;

public class EmailNotificationConsumer : IConsumer<OrderCreatedMessage>
{
    private readonly ILogger<EmailNotificationConsumer> _logger;

    public EmailNotificationConsumer(ILogger<EmailNotificationConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCreatedMessage> context)
    {
        var message = context.Message;
        
        _logger.LogInformation($"📧 [EMAIL] Iniciando envio de email para {message.CustomerEmail}");
        _logger.LogInformation($"📧 [EMAIL] Pedido #{message.OrderId} - Total: ${message.TotalAmount:F2}");
        _logger.LogInformation($"📧 [EMAIL] Itens: {string.Join(", ", message.Items.Select(i => $"{i.ProductName} x{i.Quantity}"))}");
        
        // Simular envio de email
        await Task.Delay(2000); // Simula tempo de envio
        
        _logger.LogInformation($"📧 [EMAIL] ✅ Email enviado com sucesso para pedido {message.OrderId}");
    }
}
