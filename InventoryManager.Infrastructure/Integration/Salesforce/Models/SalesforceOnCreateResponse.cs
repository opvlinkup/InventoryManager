using System.Text.Json.Serialization;

namespace InventoryManager.Infrastructure.Integration.Salesforce.Models;


public class SalesforceOnCreateResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("errors")]
    public List<string> Errors { get; set; } = new();
}