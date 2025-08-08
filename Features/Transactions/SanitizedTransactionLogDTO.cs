using System.Text.Json.Serialization;

public class SanitizedTransactionLogDTO
{
    public string? partnerkey { get; set; }
    public string? partnerrefno { get; set; }
    [JsonIgnore]
    public string? partnerpassword { get; set; }
    public string? encryptedpassword { get; set; }
    public long? totalamount { get; set; }
    public List<ItemDetailDTO>? items { get; set; }
    public string? timestamp { get; set; }
    public string? sig { get; set; }

    public SanitizedTransactionLogDTO() { }
    public SanitizedTransactionLogDTO(TransactionDetailDTO source)
    {
        partnerkey = source.partnerkey;
        partnerrefno = source.partnerrefno;
        partnerpassword = source.partnerpassword;
        // encryptedpassword = EncryptHelper.Encrypt(source.partnerpassword);
        totalamount = source.totalamount;
        items = source.items;
        timestamp = source.timestamp;
        sig = source.sig;
    }
}
