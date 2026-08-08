using Confluent.Kafka;
using Contracts.Consumer;
using Contracts.Infra.Publish;
using Order.Api.Consumers;
using Order.Api.Endpoints;
using Order.Application;
using Order.Infrastructure;
using Order.Infrastructure.Data.Context;
using Shared.Implementations.Consumer;
using Shared.Implementations.Publish;

var builder = WebApplication.CreateBuilder(args);

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
}

app.UseHttpsRedirection();

app.MapOrderEndpoints();

app.Run();