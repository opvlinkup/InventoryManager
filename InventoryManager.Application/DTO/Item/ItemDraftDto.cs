namespace InventoryManager.Application.DTO.Item;

public sealed class ItemDraftDto
{
    
    public string? Text1 { get; set; }
    public string? Text2 { get; set; }
    public string? Text3 { get; set; }

    public string? LongText1 { get; set; }
    public string? LongText2 { get; set; }
    public string? LongText3 { get; set; }

    public decimal? Number1 { get; set; }
    public decimal? Number2 { get; set; }
    public decimal? Number3 { get; set; }

    public bool? Bool1 { get; set; }
    public bool? Bool2 { get; set; }
    public bool? Bool3 { get; set; }

    public string? Link1 { get; set; }
    public string? Link2 { get; set; }
    public string? Link3 { get; set; }
}