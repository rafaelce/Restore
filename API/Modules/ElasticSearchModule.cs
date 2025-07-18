using API.Settings;
using Elasticsearch.Net;
using Nest;

namespace API.Modules;

public static class ElasticSearchModule
{
    public static IServiceCollection AddElasticSearchModule(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<ElasticSearchSettings>(config.GetSection("Elasticsearch"));

        services.AddSingleton<IElasticClient>(provider =>
        {
            var settings = config.GetSection("Elasticsearch").Get<ElasticSearchSettings>();

            if (string.IsNullOrEmpty(settings?.Url))
                throw new InvalidOperationException("Elasticsearch URL não configurada");
            
            var pool = new SingleNodeConnectionPool(new Uri(settings.Url));

            var connectionSettings = new ConnectionSettings(pool)
                .DefaultIndex(settings.DefaultIndex);

            if (settings.EnableDebugMode)
                connectionSettings.EnableDebugMode();
            

            if (!string.IsNullOrEmpty(settings.Username))
                connectionSettings.BasicAuthentication(settings.Username, settings.Password);
            

            return new ElasticClient(connectionSettings);
        });

        return services;
    }
}
