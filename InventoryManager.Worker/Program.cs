using InventoryManager.Infrastructure;
using InventoryManager.Worker;
using DotNetEnv;
using InventoryManager.Application.Abstractions.Email;
using InventoryManager.Infrastructure.Database;
using InventoryManager.Infrastructure.Email;
using Microsoft.EntityFrameworkCore;


var root = Directory.GetCurrentDirectory();
while (!File.Exists(Path.Combine(root, ".env")))
{
    var parent = Directory.GetParent(root);
    if (parent == null)
        break;
    
    root = parent.FullName;
}

var envFile = Path.Combine(root, ".env");

if (File.Exists(envFile))
{
    Env.Load(envFile);
}

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddDbContext<InventoryManagerDbContext>(options =>
    options.UseNpgsql(builder.Configuration["DB_CONNECTION_LOCAL"]));

builder.Services.AddHttpClient();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddHostedService<OutboxProcessor>();

var host = builder.Build();
host.Run();