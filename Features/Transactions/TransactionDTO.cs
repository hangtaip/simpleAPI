using System.ComponentModel.DataAnnotations;

public class TransactionDetailDTO
{
    [Required]
    [StringLength(50, MinimumLength = 50)]
    public string? partnerkey { get; set; }
    [Required]
    [StringLength(50, MinimumLength = 50)]
    public string? partnerrefno { get; set; }
    [Required]
    [StringLength(50, MinimumLength = 50)]
    public string? partnerpassword { get; set; }
    [Required]
    public long? totalamount { get; set; }
    public List<ItemDetailDTO>? items { get; set; }
    [Required]
    public string? timestamp { get; set; }
    [Required]
    public string? sig { get; set; }

    public TransactionDetailDTO() { }
    public TransactionDetailDTO(TransactionDetail transactionDetail)
    {
        partnerkey = transactionDetail.PartnerKey;
        partnerrefno = transactionDetail.PartnerRefNo;
        partnerpassword = transactionDetail.PartnerPassword;
        totalamount = transactionDetail.TotalAmount;
        items = transactionDetail.Items;
        timestamp = transactionDetail.Timestamp;
        sig = transactionDetail.Sig;
    }
}
