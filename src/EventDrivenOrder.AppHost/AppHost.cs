var builder = DistributedApplication.CreateBuilder(args);

var kafka = builder
    .AddKafka("kafka")
    .WithKafkaUI();

var orderDb = builder
    .AddPostgres("order-postgres")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("order-db");

var inventoryDb = builder
    .AddPostgres("inventory-postgres")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("inventory-db");

builder.Build().Run();