using System.Collections;
using System.Text;
using System.Text.Json.Serialization;
using InventoryManager.Application;
using InventoryManager.Infrastructure;
using InventoryManager.Infrastructure.Hubs;
using InventoryManager.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using DotNetEnv;
using InventoryManager.Infrastructure.Database;
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


var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var port = builder.Configuration["PORT"] ?? throw new InvalidOperationException("PORT environment variable is required");

bool isLocal = builder.Environment.IsDevelopment();

if (isLocal)
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenLocalhost(port: int.Parse(port), listenOptions =>
        {
            listenOptions.UseHttps();
        });
    });
    Console.WriteLine($"Local host runs on port {port}");
    
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("LocalCorsPolicy", policy =>
            policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
    });
}
else
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
    Console.WriteLine($"Prod runs on port {port}");
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("ProdCorsPolicy", policy =>
            policy.SetIsOriginAllowed(_ => true)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
    });
}

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var key = builder.Configuration["JWT_SEC_KEY"] ?? throw new InvalidOperationException("JWT key is required");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var keyBytes = Encoding.UTF8.GetBytes(key);

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["ISSUER"],
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("accessToken"))
            {
                context.Token = context.Request.Cookies["accessToken"];
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<InventoryManagerDbContext>();

    var retries = 2;
    while (retries > 0)
    {
        try
        {
            await context.Database.MigrateAsync();
            await DbInitializer.SeedRolesAndAdminAsync(scope.ServiceProvider, builder.Configuration);
            break;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Migration failed: {ex.Message}, retries left: {retries}");
            retries--;
            await Task.Delay(3000);
        }
    }
}

app.UseCors(isLocal ? "LocalCorsPolicy" : "ProdCorsPolicy");
app.UseRouting();
app.UseAuthentication();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<LastActivityMiddleware>();
app.UseAuthorization();
app.UseMiddleware<TransactionMiddleware>();

app.MapControllers();
app.MapHub<DiscussionHub>("/hubs/discussion");

app.Run();