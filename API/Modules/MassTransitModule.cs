using API.Consumers;
using API.RequestHelpers;
using MassTransit;
using Microsoft.Extensions.Options;

namespace API.Modules;

public static class MassTransitModule
{
    public static IServiceCollection AddMassTransitModule(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<MassTransitSettings>(config.GetSection("MassTransit"));

        services.AddMassTransit(x =>
        {
            // Registrar todos os consumers
            x.AddConsumer<EmailNotificationConsumer>();
            x.AddConsumer<StockUpdateConsumer>();
            x.AddConsumer<InvoiceGenerationConsumer>();
            x.AddConsumer<PushNotificationConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var settings = context.GetRequiredService<IOptions<MassTransitSettings>>().Value;

                cfg.Host(settings.HostName, settings.VirtualHost, h =>
                {
                    h.Username(settings.UserName);
                    h.Password(settings.Password);
                });

                // Configurar filas específicas
                cfg.ReceiveEndpoint("order-created", e =>
                {
                    e.ConfigureConsumer<EmailNotificationConsumer>(context);
                });

                cfg.ReceiveEndpoint("stock-update", e =>
                {
                    e.ConfigureConsumer<StockUpdateConsumer>(context);
                });

                cfg.ReceiveEndpoint("invoice-generation", e =>
                {
                    e.ConfigureConsumer<InvoiceGenerationConsumer>(context);
                });

                cfg.ReceiveEndpoint("push-notification", e =>
                {
                    e.ConfigureConsumer<PushNotificationConsumer>(context);
                });
            });
        });

        return services;
    }
}
