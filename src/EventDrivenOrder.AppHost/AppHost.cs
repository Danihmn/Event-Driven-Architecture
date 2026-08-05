var builder = DistributedApplication.CreateBuilder(args);

var kafka = builder
    .AddKafka("kafka")
    .WithKafkaUI();

var orderDb = builder
    .AddPostgres("order-postgres", port: 5432)
    .WithDataVolume()
    .AddDatabase("order-db");

var inventoryDb = builder
    .AddPostgres("inventory-postgres", port: 5433)
    .WithDataVolume()
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