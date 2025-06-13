using API.Middleware;
using API.Modules;
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
app.UseCors("frontend");

app.Run();
