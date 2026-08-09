using Confluent.Kafka;
using Contracts.EventConsumer;
using Contracts.Infra.EventPublisher;
using Order.Api.Consumers;
using Order.Api.Endpoints;
using Order.Application;
using Order.Infrastructure;
using Order.Infrastructure.Data.Context;
using Scalar.AspNetCore;
using Serilog;
using Shared.Implementations.EventConsumer;
using Shared.Implementations.EventPublisher;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services));

builder.AddNpgsqlDbContext<OrderDbContext>("orderdb");

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

builder.Services.AddHostedService<InventoryReserveFailedConsumer>();
builder.Services.AddHostedService<InventoryReservedConsumer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapOrderEndpoints();

app.Run();