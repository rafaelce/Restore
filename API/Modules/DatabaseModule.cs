using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API;

public static class DatabaseModule
{
    public static IServiceCollection AddDatabaseModule(this IServiceCollection services,
    IConfiguration config)
    {

        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseNpgsql(config["Storage:Db:Postgre"]!);
        });

        return services;
    }

    public static WebApplication UseDatabaseModule(this WebApplication app)
    {

        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            context.Database.Migrate();
            DbInitializer.Initialize(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "A problem occurred during migration");
        }

        return app;
    }
}
