using API.Modules;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(c => {
    c.AddPolicy("frontend", options => options
        .WithOrigins("https://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

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

app.UseCors("frontend");

app.Run();
