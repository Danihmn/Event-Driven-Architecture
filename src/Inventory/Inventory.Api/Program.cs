using Confluent.Kafka;
using Contracts.Infra.Publish;
using Inventory.Api.Consumers;
using Inventory.Application;
using Inventory.Infrastructure;
using Inventory.Infrastructure.Data.Context;
using Shared.Implementations.Publish;

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

builder.Services.AddSingleton<IProducer<string, string>>(sp =>
{
    var config = new ProducerConfig
    {
        BootstrapServers = builder.Configuration.GetConnectionString("kafka")
    };
    return new ProducerBuilder<string, string>(config).Build();
});

builder.Services.AddScoped<IEventPublisher, KafkaEventPublisher>();

builder.Services.AddOpenApi();

builder.Services.AddHostedService<OrderCreatedConsumer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();