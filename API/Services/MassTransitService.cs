using MassTransit;
using API.Services.Interfaces;

namespace API.Services;

public class MassTransitService : IMessageService
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public MassTransitService(IPublishEndpoint publishEndpoint, ISendEndpointProvider sendEndpointProvider)
    {
        _publishEndpoint = publishEndpoint;
        _sendEndpointProvider = sendEndpointProvider;
    }
    
    public async Task PublishMessageAsync<T>(T message) where T : class
    {
        await _publishEndpoint.Publish(message);
    }

    public async Task PublishMessageAsync<T>(T message, string queueName) where T : class
    {
        if (queueName == "stock-update")
        {
            // Para stock-update, vamos usar Publish em vez de Send
            await _publishEndpoint.Publish(message);
        }
        else
        {
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{queueName}"));
            await endpoint.Send(message);
        }
    }
}
