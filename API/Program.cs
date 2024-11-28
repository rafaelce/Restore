using API;

var builder = WebApplication.CreateBuilder(args);

//Cors policy
builder.Services.AddCors(c => {
    c.AddPolicy("frontend", opt => opt
        .WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddRestoreModule(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors("frontend");
app.UseRestoreModule(builder.Configuration);

app.Run();
