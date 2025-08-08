using System.ComponentModel.DataAnnotations;

public class ResponseDetailDTO
{
    [Required]
    public int Result { get; set; }
    public long? Totalamount { get; set; }
    public long? Totaldiscount { get; set; }
    public long? Finalamount { get; set; }

    public ResponseDetailDTO() { }
    public ResponseDetailDTO(ResponseDetail responseDetail)
    {
        Result = responseDetail.Result;
        Totalamount = responseDetail.Totalamount;
        Totaldiscount = responseDetail.Totaldiscount;
        Finalamount = responseDetail.Finalamount;
    }
}

public class FailedResponseDetailDTO
{
    [Required]
    public int Result { get; set; }
    public string? Resultmessage { get; set; }

    public FailedResponseDetailDTO() { }
    public FailedResponseDetailDTO(FailedResponseDetail failedResponseDetail)
    {
        Result = failedResponseDetail.Result;
        Resultmessage = failedResponseDetail.Resultmessage;
    }
}
