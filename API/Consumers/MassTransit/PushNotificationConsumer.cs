using API.DTO.MassTransit;
using MassTransit;

namespace API.Consumers;

public class PushNotificationConsumer : IConsumer<PushNotificationMessage>
{
    private readonly ILogger<PushNotificationConsumer> _logger;

    public PushNotificationConsumer(ILogger<PushNotificationConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PushNotificationMessage> context)
    {
        var message = context.Message;
        
        _logger.LogInformation($"📱 [PUSH] Enviando notificação para {message.CustomerId}");
        _logger.LogInformation($"📱 [PUSH] Título: {message.Title}");
        _logger.LogInformation($"�� [PUSH] Mensagem: {message.Message}");
        
        // Simular envio de push notification
        await Task.Delay(1000);
        
        _logger.LogInformation($"📱 [PUSH] ✅ Notificação push enviada para {message.CustomerId}");
    }
}
