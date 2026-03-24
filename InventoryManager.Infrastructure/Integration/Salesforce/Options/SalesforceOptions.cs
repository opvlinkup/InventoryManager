namespace InventoryManager.Infrastructure.Integration.Salesforce.Options;

public class SalesforceOptions
{
    public string ClientId { get; set; } = null!;
    public string TokenEndpoint { get; set; } = null!;
    public string Username { get; set; } = null!;
    
    public string PrivateKey { get; set; } = null!;
}