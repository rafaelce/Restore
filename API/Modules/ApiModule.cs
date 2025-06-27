using API.Entities;
using API.Infra.EntityFramework;
using API.Middleware;
using API.Services;
using Microsoft.AspNetCore.Identity;

namespace API.Modules;

public static class ApiModule
{
    public static IServiceCollection AddApiModule(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        services.AddDatabaseModule(configuration);
        services.AddHostedService<ApplyMigrationBackgroundService>();
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

        services.AddScoped<PaymentsService>();

        // Add services to the container.
        services.AddControllers();

        return services;
    }
}
