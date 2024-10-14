namespace API;

public static class RestoreModule
{
    public static IServiceCollection AddRestoreModule(this IServiceCollection services,
    IConfiguration configuration){
        
        services.AddLogging();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddDatabaseModule(configuration);

        return services;
    }

    public static WebApplication UseRestoreModule(this WebApplication app, IConfiguration configuration){
        
        app.UseDatabaseModule(); 
        
        return app;
    }
}
