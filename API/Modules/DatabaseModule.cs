
using API.Infra.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Modules;

public static class DatabaseModule
{
    public static IServiceCollection AddDatabaseModule(this IServiceCollection services, IConfiguration configuration)
    {
        // PostgreSQL
        services.AddDbContext<ApplicationContext>(opt =>
                 opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}
