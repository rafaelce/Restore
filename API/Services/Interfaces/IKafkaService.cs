using API.Events;

namespace API.Services.Interfaces;

public interface IKafkaService
{
    Task PublishUserEventAsync(UserEvent userEvent);
    Task PublishOrderEventAsync(OrderEvent orderEvent);
    Task PublishSearchEventAsync(SearchEvent searchEvent);
}
