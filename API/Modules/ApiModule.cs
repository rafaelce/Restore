using API.Infra.EntityFramework;

namespace API.Modules;

public static class ApiModule
{
    public static IServiceCollection AddApiModule(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        services.AddDatabaseModule(configuration);
        services.AddHostedService<ApplyMigrationBackgroundService>();

        return services;
    }
}
