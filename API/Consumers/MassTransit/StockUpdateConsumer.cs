using API.DTO.MassTransit;
using API.Infra.EntityFramework;
using MassTransit;

namespace API.Consumers;

public class StockUpdateConsumer : IConsumer<StockUpdateMessage>
{
    private readonly ILogger<StockUpdateConsumer> _logger;
    private readonly ApplicationContext _context;

    public StockUpdateConsumer(ILogger<StockUpdateConsumer> logger, ApplicationContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task Consume(ConsumeContext<StockUpdateMessage> context)
    {
        var message = context.Message;
        
        _logger.LogInformation($"📦 [ESTOQUE] Atualizando estoque do produto {message.ProductId} (quantidade: {message.QuantitySold})");
        
        // Simular atualização de estoque
        await Task.Delay(1500);
        
        _logger.LogInformation($"📦 [ESTOQUE] ✅ Estoque atualizado para produto {message.ProductId}");
    }
}
