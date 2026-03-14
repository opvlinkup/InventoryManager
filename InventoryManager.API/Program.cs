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

var port = Environment.GetEnvironmentVariable("PORT") ?? builder.Configuration["PORT"];

if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

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

    await context.Database.MigrateAsync();

    await DbInitializer.SeedRolesAndAdminAsync(services, builder.Configuration);
}

app.UseCors("DefaultCorsPolicy");
app.UseRouting();
app.UseAuthentication();
app.UseMiddleware<LastActivityMiddleware>();
app.UseAuthorization();
app.UseMiddleware<TransactionMiddleware>();

app.MapControllers();
app.MapHub<DiscussionHub>("/hubs/discussion");

app.Run();