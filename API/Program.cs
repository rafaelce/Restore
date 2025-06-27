using API.Entities;
using API.Middleware;
using API.Modules;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenApi()
    .AddApiModule(builder.Configuration, builder.Environment);

var app = builder.Build();

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseReDoc(opt => {
        opt.DocumentTitle = "API Documentation";
        opt.RoutePrefix = "redoc";
        opt.SpecUrl = "/openapi/v1.json";
    });
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
