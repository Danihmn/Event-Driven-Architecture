var builder = DistributedApplication.CreateBuilder(args);

var kafka = builder
    .AddKafka("kafka")
    .WithKafkaUI();

var orderDb = builder
    .AddPostgres("order-postgres", port: 5432)
    .WithPgWeb()
    .WithDataVolume()
    .AddDatabase("orderdb");

var inventoryDb = builder
    .AddPostgres("inventory-postgres", port: 5433)
    .WithPgWeb()
    .WithDataVolume()
    .AddDatabase("inventorydb");

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