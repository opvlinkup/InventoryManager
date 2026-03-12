using System.Net.Http.Headers;
using System.Net.Http.Json;
using InventoryManager.Application.Abstractions.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace InventoryManager.Infrastructure.Email;

public sealed class EmailService(
    IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<EmailService> logger) : IEmailService
{
    public async Task SendConfirmationAsync(
        Guid userId,
        string email,
        string token,
        CancellationToken ct)
    {
        var apiKey = configuration["RESEND_API_KEY"]
                     ?? throw new InvalidOperationException("RESEND_API_KEY missing");

        var sender = configuration["RESEND_API_FROM"]
                     ?? throw new InvalidOperationException("RESEND_FROM missing");

        var resendUrl = configuration["RESEND_API"]
                        ?? "https://api.resend.com";

        var frontendUrl = configuration["CLIENT_BASE_URL"]
                          ?? throw new InvalidOperationException("CLIENT_BASE_URL missing");

        var encodedToken = Uri.EscapeDataString(token);

        var confirmationLink = $"{frontendUrl}/api/auth/confirm-email?userId={userId}&token={encodedToken}";

        var html = $"""
                    <h3>Confirm your email</h3>
                    <p>Please confirm your account:</p>
                    <p><a href="{confirmationLink}">Confirm Email</a></p>
                    """;

        var client = httpClientFactory.CreateClient();

        client.BaseAddress = new Uri(resendUrl);
        
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        var body = new
        {
            from = sender,
            to = new[] { email },
            subject = "Confirm your InventoryManager account",
            html
        };

        var response = await client.PostAsJsonAsync("emails", body, ct);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);

            logger.LogError("Email sending failed {Status}: {Error}", response.StatusCode, error);
        }
    }
}