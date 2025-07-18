using API.Entities;
using API.Middleware;
using API.Modules;
using API.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenApi()
    .AddApiModule(builder.Configuration);

var app = builder.Build();

// using (var scope = app.Services.CreateScope())
// {
//     var kafkaTopicService = scope.ServiceProvider.GetRequiredService<KafkaTopicService>();
//     await kafkaTopicService.CreateTopicsAsync();
// }

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

//adicionados para o deploy
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors(opt =>
{
    opt.WithOrigins("https://localhost:3000", "http://localhost:3000")
       .AllowAnyHeader()
       .AllowAnyMethod()
       .AllowCredentials();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("api").MapIdentityApi<User>();
app.MapFallbackToController("Index", "Fallback");

app.MapGraphQL("/graphql");

app.Run();
