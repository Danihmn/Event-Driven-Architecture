using Confluent.Kafka;
using Inventory.Api.Consumers;
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
        AutoOffsetReset = AutoOffsetReset.Earliest,
        AllowAutoCreateTopics = true
    };

    return new ConsumerBuilder<string, string>(config).Build();
});

builder.Services.AddOpenApi();

builder.Services.AddHostedService<OrderCreatedConsumer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();