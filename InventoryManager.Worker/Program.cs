using InventoryManager.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<OutboxProcessor>();

var host = builder.Build();
host.Run();