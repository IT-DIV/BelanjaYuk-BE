using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BelanjaYukDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5042",
            "http://localhost:5000",
            "http://localhost:5001",
            "https://localhost:5042",
            "https://localhost:5000",
            "https://localhost:5001",
            "https://dev.drian.my.id",
            "https://prod.drian.my.id"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS
app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.MapGet("/", () => new
{
    status = "OK",
    message = "BelanjaYuk API is running",
    environment = app.Environment.EnvironmentName,
    timestamp = DateTime.UtcNow
});

app.MapControllers();
app.Run();