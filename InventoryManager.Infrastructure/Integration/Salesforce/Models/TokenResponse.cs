using System.Text.Json.Serialization;

namespace InventoryManager.Infrastructure.Integration.Salesforce.Models;

public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = null!;

    [JsonPropertyName("instance_url")]
    public string InstanceUrl { get; set; } = null!;

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = null!;

    [JsonPropertyName("issued_at")]
    public string IssuedAt { get; set; } = null!;

    [JsonPropertyName("expires_in")] 
    public int ExpiresIn { get; set; }
}