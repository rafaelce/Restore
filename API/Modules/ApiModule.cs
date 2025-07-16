using API.Entities;
using API.Infra.EntityFramework;
using API.Middleware;
using API.RequestHelpers;
using API.Services;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using StackExchange.Redis;
using IMessageService = API.Services.Interfaces.IMessageService;


namespace API.Modules;

public static class ApiModule
{
    public static IServiceCollection AddApiModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseModule(configuration);
        services.AddMassTransitModule(configuration);
        services.AddHostedService<ApplyMigrationBackgroundService>();
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(Program).Assembly));
        services.AddCors(c =>
            {
                c.AddPolicy("frontend", options => options
                    .AllowCredentials()
                    .WithOrigins("https://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            });

        services.AddTransient<ExceptionMiddleware>();
        services.AddIdentityApiEndpoints<User>(opt =>
        {
            opt.User.RequireUniqueEmail = true;
            opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            opt.Lockout.MaxFailedAccessAttempts = 5;
            opt.Lockout.AllowedForNewUsers = false;
        })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationContext>();

        services.AddApplicationServices();

        // Add services to the container.
        services.AddControllers();

        // Adicionando Redis
        services.AddSingleton<IConnectionMultiplexer>(c =>
        {
            var RedisConfig = ConfigurationOptions.Parse(configuration.GetConnectionString("Redis")!);
            return ConnectionMultiplexer.Connect(RedisConfig);
        });

        //registra a seção "Cloudinary" do seu arquivo de configuração (ex: appsettings.json ou variáveis de ambiente)
        services.Configure<CloudinarySettings>(configuration.GetSection("Cloudinary"));

        return services;
    }

    private static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<PaymentsService>();
        services.AddScoped<ImageService>();
        services.AddScoped<IRedisCacheService, RedisCacheService>();
        services.AddScoped<IMessageService, MassTransitService>();

        return services;
    }

    
}