using Confluent.Kafka;
using Inventory.Application;
using Inventory.Infrastructure;
using Inventory.Infrastructure.Data.Context;

var builder = WebApplication.CreateBuilder(args);

builder.AddNpgsqlDbContext<InventoryDbContext>("inventorydb");

builder.Services.AddInfrastructure();
builder.Services.AddApplication();

builder.Services.AddSingleton<IConsumer<string, string>>(sp =>
{
    var config = new ConsumerConfig
    {
        BootstrapServers = builder.Configuration.GetConnectionString("kafka"),
        GroupId = "inventory-service",
        AutoOffsetReset = AutoOffsetReset.Earliest
    };

    return new ConsumerBuilder<string, string>(config).Build();
});

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();