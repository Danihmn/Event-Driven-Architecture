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

builder.AddProject<Projects.Order_Api>("order-api")
    .WithReference(orderDb)
    .WithReference(kafka)
    .WaitFor(orderDb)
    .WaitFor(kafka);

builder.AddProject<Projects.Inventory_Api>("inventory-api")
    .WithReference(inventoryDb)
    .WithReference(kafka)
    .WaitFor(inventoryDb)
    .WaitFor(kafka);

builder.Build().Run();