using Confluent.Kafka;
using Contracts.Infra.Event;
using Order.Api.Endpoints;
using Order.Api.Messaging;
using Order.Application;
using Order.Infrastructure;
using Order.Infrastructure.Data.Context;

var builder = WebApplication.CreateBuilder(args);

builder.AddNpgsqlDbContext<OrderDbContext>("orderdb");

builder.Services.AddInfrastructure();
builder.Services.AddApplication();

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapOrderEndpoints();

app.Run();