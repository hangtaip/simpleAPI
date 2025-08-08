using System.ComponentModel.DataAnnotations;

public class ItemDetailDTO
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string? partneritemref { get; set; }
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string? name { get; set; }
    [Required]
    public int? qty { get; set; }
    [Required]
    public long? unitprice { get; set; }

    public ItemDetailDTO() { }
    public ItemDetailDTO(ItemDetail itemDetail)
    {
        partneritemref = itemDetail.PartnerItemRef;
        name = itemDetail.Name;
        qty = itemDetail.Qty;
        unitprice = itemDetail.UnitPrice;
    }
}
