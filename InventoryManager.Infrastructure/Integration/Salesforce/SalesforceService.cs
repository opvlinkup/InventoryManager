using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using InventoryManager.Application.Abstractions.Integration;
using InventoryManager.Application.DTO.Integration.Salesforce;
using InventoryManager.Infrastructure.Integration.Salesforce.Models;
using InventoryManager.Infrastructure.Integration.Salesforce.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace InventoryManager.Infrastructure.Integration.Salesforce;



public class SalesforceService(HttpClient httpClient, IOptions<SalesforceOptions> options, ILogger<SalesforceService> logger)
    : ISalesforceService
{
    private readonly SalesforceOptions _options = options.Value;

    private string? _accessToken;
    private string? _instanceUrl;
    private DateTime _tokenExpiration;

    public async Task<string> CreateCustomerAsync(SalesforceCustomerDto dto, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(_accessToken) || DateTime.UtcNow >= _tokenExpiration)
        {
            await AuthenticateAsync(ct);
        }

        var accountId = await CreateAccountAsync(dto.CompanyName, ct);
        var contactId = await CreateContactAsync(dto, accountId, ct);

        return contactId;
    }

    private async Task AuthenticateAsync(CancellationToken ct)
    {
        var jwt = GenerateJwt();

        var request = new HttpRequestMessage(HttpMethod.Post, _options.TokenEndpoint)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "urn:ietf:params:oauth:grant-type:jwt-bearer",
                ["assertion"] = jwt
            })
        };

        var response = await httpClient.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException(error);
        }

        var result = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: ct)
                     ?? throw new InvalidOperationException("Invalid token response");

        _accessToken = result.AccessToken;
        _instanceUrl = result.InstanceUrl;

        _tokenExpiration = DateTime.UtcNow.AddSeconds(result.ExpiresIn - 60);
    }

    private string GenerateJwt()
    {
        var normlizedSecretKey = _options.PrivateKey.Replace("\\n", "\n").Trim('"');
        
        var rsa = RSA.Create();
        rsa.ImportFromPem(normlizedSecretKey);

        var securityKey = new RsaSecurityKey(rsa);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);

        var now = DateTime.UtcNow;

        var token = new JwtSecurityToken(
            issuer: _options.ClientId,
            audience: _options.TokenEndpoint,
            claims:
            [
                new Claim("sub", _options.Username)
            ],
            notBefore: now,
            expires: now.AddMinutes(3),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<string> CreateAccountAsync(string companyName, CancellationToken ct)
    {
        var url = $"{_instanceUrl}/services/data/v66.0/sobjects/Account";

     
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _accessToken) },
            Content = new StringContent(
                JsonSerializer.Serialize(new { Name = companyName }), 
                System.Text.Encoding.UTF8, 
                "application/json")
        };

        var response = await httpClient.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException($"Account creation failed: {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<SalesforceOnCreateResponse>(cancellationToken: ct);

        return result?.Id ?? throw new InvalidOperationException("Account creation failed");
    }

    private async Task<string> CreateContactAsync(SalesforceCustomerDto dto, string accountId, CancellationToken ct)
    {
        var url = $"{_instanceUrl}/services/data/v66.0/sobjects/Contact";

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _accessToken) },
            Content = JsonContent.Create(new
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                AccountId = accountId
            })
        };

        var response = await httpClient.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException(error);
        }

        var result = await response.Content.ReadFromJsonAsync<SalesforceOnCreateResponse>(cancellationToken: ct);

        return result?.Id ?? throw new InvalidOperationException("Contact creation failed");
    }
}