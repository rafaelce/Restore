using API.Entities;
using API.Infra.EntityFramework;
using API.Middleware;
using API.Modules;
using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(c =>
{
    c.AddPolicy("frontend", options => options
        .AllowCredentials()
        .WithOrigins("https://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod());
});
builder.Services.AddTransient<ExceptionMiddleware>();
builder.Services.AddIdentityApiEndpoints<User>(opt =>
{
    opt.User.RequireUniqueEmail = true;
    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    opt.Lockout.MaxFailedAccessAttempts = 5;
    opt.Lockout.AllowedForNewUsers = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationContext>();

builder.Services
    .AddOpenApi()
    .AddApiModule(builder.Configuration, builder.Environment);

// Add services to the container.
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseCors(opt =>
{
    opt.WithOrigins("https://localhost:3000")
       .AllowAnyHeader()
       .AllowAnyMethod()
       .AllowCredentials();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("api").MapIdentityApi<User>();

app.Run();
