
using API.Infra.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Modules;

public static class DatabaseModule
{
    public static IServiceCollection AddDatabaseModule(this IServiceCollection services, IConfiguration configuration)
    {
        //mssqlserver
        services.AddDbContext<ApplicationContext>(opt =>
                 opt.UseSqlServer(configuration["ConnectionStrings:DatabaseConnection"]!));

        return services;
    }
}
