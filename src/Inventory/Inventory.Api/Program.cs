using Confluent.Kafka;
using Contracts.EventConsumer;
using Contracts.Infra.EventPublisher;
using Inventory.Api.Consumers;
using Inventory.Api.Endpoints;
using Inventory.Application;
using Inventory.Infrastructure;
using Inventory.Infrastructure.Data.Context;
using Scalar.AspNetCore;
using Serilog;
using Shared.Implementations.EventConsumer;
using Shared.Implementations.EventPublisher;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services));

builder.AddNpgsqlDbContext<InventoryDbContext>("inventorydb");

builder.Services.AddInfrastructure();
builder.Services.AddApplication();

builder.Services.AddSingleton<IKafkaConsumerFactory, KafkaConsumerFactory>();

builder.Services.AddSingleton<IProducer<string, string>>(_ =>
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
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapProductsEndpoints();

app.Run();