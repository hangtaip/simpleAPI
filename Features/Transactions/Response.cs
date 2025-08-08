public class ResponseDetail
{
    public int Result { get; set; }
    public long? Totalamount { get; set; }
    public long? Totaldiscount { get; set; }
    public long? Finalamount { get; set; }
}

public class FailedResponseDetail
{
    public int Result { get; set; }
    public string? Resultmessage { get; set; }
}
