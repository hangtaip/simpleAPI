using System.Text.Json;
namespace TransactionApi;

public partial class Endpoints
{
    private readonly IAppLogger _logger; private readonly PrimeCheckService _primeChecker; public Endpoints(IAppLogger logger, PrimeCheckService primeChecker)
    {
        _logger = logger;
        _primeChecker = primeChecker;
    }

    public void Map(WebApplication app)
    {
        app.MapPost("/api/submittrxmessage", PostDetails);
    }

    public async Task<IResult> PostDetails(TransactionDetailDTO transaction)
    {
        // await EncyptPassword(transaction.partnerpassword);
        _logger.Info($"Processing Data. Data: {JsonSerializer.Serialize(transaction)}");

        var (totalDiscount, finalAmount) = await GetDiscount(transaction.totalamount);

        var response = new ResponseDetailDTO
        {
            Result = 1,
            Totalamount = transaction.totalamount,
            Totaldiscount = totalDiscount,
            Finalamount = finalAmount
        };

        return TypedResults.Ok(response);
    }

    private async Task<(long? TotalDiscount, long? FinalAmount)> GetDiscount(long? totalAmount)
    {
        // base
        // total < 200,00; 0
        // total >=200,00 total <=500,00; 5
        // total >= 501,00 total <= 800,00; 7
        // total >= 801,00 total <= 1,200,00; 10
        // total > 1,200,00; 15
        //
        // additional
        // total == prime && total >= 500,00; 8
        // total[length-1] = 5 && total > 900,00; 10
        // max discount = 20

        long discount = totalAmount switch
        {
            >= 200_00 and <= 500_00 => 5,
            >= 500_01 and <= 800_00 => 7,
            >= 801_01 and <= 1_200_00 => 10,
            > 1_200_00 => 15,
            _ => 0
        };

        _logger.Info($"totalAmount 1st: {totalAmount}");
        if (totalAmount >= 500_00 && await _primeChecker.IsPrime(totalAmount)) discount += 8;

        if (totalAmount > 900_00 && totalAmount % 10 == 5) discount += 10;

        _logger.Info($"discount 1st: {discount}");
        discount = Math.Min(discount, 20);
        _logger.Info($"discount 2nd: {discount}");

        long? totalDiscount = totalAmount * discount / 100;
        long? finalAmount = totalAmount - totalDiscount;

        return (totalDiscount, finalAmount);
    }
}
