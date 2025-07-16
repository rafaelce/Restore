namespace API.Services.Interfaces;

public interface IMessageService
{
    Task PublishMessageAsync<T>(T message) where T : class;
    Task PublishMessageAsync<T>(T message, string queueName) where T : class;
}
