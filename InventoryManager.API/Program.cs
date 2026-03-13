using System.Text;
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

var port = Environment.GetEnvironmentVariable("PORT") ?? "7022";
builder.WebHost.UseUrls($"http://*:{port}");

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();

var authOptions = builder.Configuration.GetSection("AuthOptions");
var key = authOptions["Key"] ?? throw new InvalidOperationException("AuthOptions:Key is required");

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

    await context.Database.MigrateAsync();

    await DbInitializer.SeedRolesAndAdminAsync(services);
}

app.UseCors("DefaultCorsPolicy");

app.UseAuthentication();
app.UseMiddleware<LastActivityMiddleware>();
app.UseAuthorization();
app.UseMiddleware<TransactionMiddleware>();

app.MapControllers();
app.MapHub<DiscussionHub>("/hubs/discussion");

app.Run();