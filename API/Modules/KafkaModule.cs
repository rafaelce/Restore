using API.Consumers.Kafka;
using API.RequestHelpers;

namespace API.Modules;

public static class KafkaModule
{
    public static IServiceCollection AddKafkaModule(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<KafkaSettings>(config.GetSection("Kafka"));

        services.AddHostedService<UserEventsConsumer>();
        services.AddHostedService<OrderEventsConsumer>();
        services.AddHostedService<SearchEventsConsumer>();

        return services;
    }
}
