using Strategico.Inventory.Api.Data;
using Strategico.Inventory.Api.Services;
using Microsoft.EntityFrameworkCore;
using Strategico.Inventory.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IWarehouseProvider, WarehouseProvider>();

// Program.cs (example - read connection from environment with fallback)
var conn = builder.Configuration.GetConnectionString("InventoryConnection")
           ?? Environment.GetEnvironmentVariable("InventoryConnection")
           ?? "Host=localhost;Database=inventorydb;Username=inventory;Password=inventory";

builder.Services.AddDbContext<InventoryDbContext>(opts =>
    opts.UseNpgsql(conn));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<WarehouseMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Add Somedata 
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    var attempts = 0;
    while (attempts < 5)
    {
        try
        {
            db.Database.EnsureCreated();
            SeedData.Initialize(db);    
            break;
        }
        catch (Npgsql.NpgsqlException ex) when (attempts < 4)
        {
            logger.LogWarning(ex, "DB not ready yet, retrying...");
            attempts++;
            Thread.Sleep(TimeSpan.FromSeconds(2 * attempts));
        }
    }
}

app.Run();
