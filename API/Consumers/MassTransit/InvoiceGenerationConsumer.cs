using API.DTO.MassTransit;
using MassTransit;

namespace API.Consumers;

public class InvoiceGenerationConsumer : IConsumer<InvoiceGenerationMessage>
{
    private readonly ILogger<InvoiceGenerationConsumer> _logger;

    public InvoiceGenerationConsumer(ILogger<InvoiceGenerationConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<InvoiceGenerationMessage> context)
    {
        var message = context.Message;
        
        _logger.LogInformation($"🧾 [FATURA] Gerando fatura para pedido {message.OrderId}");
        _logger.LogInformation($"🧾 [FATURA] Cliente: {message.CustomerEmail}");
        _logger.LogInformation($"�� [FATURA] Valor: ${message.TotalAmount:F2}");
        
        // Simular geração de fatura
        await Task.Delay(3000);
        
        _logger.LogInformation($"🧾 [FATURA] ✅ Fatura gerada e enviada para {message.CustomerEmail}");
    }
}
