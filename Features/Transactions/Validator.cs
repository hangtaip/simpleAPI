using FluentValidation;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

using TransactionApi;

// NOTE: Message '<fieldname> is required' won't actually appear because sig warning will override other mandatory field 
public class TransactionDetailDTOValidator : AbstractValidator<TransactionDetailDTO>
{
    private readonly IAppLogger _logger;

    public TransactionDetailDTOValidator(IAppLogger logger)
    {
        _logger = logger;

        var serverTime = DateTime.UtcNow;

        RuleFor(x => x.partnerkey)
            .NotNull()
            .Length(1, 50)
            .Must(x => x == "FAKEGOOGLE" || x == "FAKEPEOPLE")
            .WithMessage("Access Denied!");

        RuleFor(x => x.partnerrefno)
            .NotNull().WithMessage("parterrefno is required.")
            .Length(1, 50);

        RuleFor(x => x.partnerpassword)
            .NotNull().WithMessage("partnerpassword is required.")
            .Length(1, 50);

        RuleFor(x => x.totalamount)
            .NotNull().GreaterThanOrEqualTo(0)
            .Must((dto, totalamount) =>
            {
                if (dto.items == null || dto.items.Count == 0) return true;

                long? itemSum = dto.items.Sum(item => item.qty * item.unitprice);
                // _logger.Info($"itemSum: {itemSum}");

                // +- 0.05cent
                // _logger.Info($"diff: {itemSum - totalamount}");
                return (itemSum >= totalamount - 5) && (itemSum <= totalamount + 5);
            })
            .WithMessage("Invalid Total Amount");

        RuleFor(x => x.timestamp)
                .NotNull()
                .Must(timestamp =>
                {
                    if (!DateTime.TryParse(timestamp, null, DateTimeStyles.AdjustToUniversal, out var providedTime)) return false;

                    var timeDiff = (providedTime - serverTime).Duration();

                    // _logger.Info($"timeDiff: {timeDiff}");
                    return timeDiff <= TimeSpan.FromMinutes(5);
                })
                .WithMessage("Expired.");

        RuleFor(x => x.sig)
                .NotNull()
                .Must((dto, sig) =>
                {
                    try
                    {
                        if (!DateTime.TryParse(dto.timestamp, null, DateTimeStyles.AdjustToUniversal, out var timestamp)) return false;

                        var formattedTimestamp = timestamp.ToString("yyyyMMddHHmmss");

                        var input = $"{formattedTimestamp}{dto.partnerkey}{dto.partnerrefno}{dto.totalamount}{dto.partnerpassword}";

                        using var sha256 = SHA256.Create();

                        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                        var hexHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                        var base64Hash = Convert.ToBase64String(Encoding.UTF8.GetBytes(hexHash));

                        _logger.Info($"bashe64Hash: {base64Hash}");
                        return base64Hash == sig;
                    }
                    catch
                    {
                        return false;
                    }
                })
                .WithMessage("Access Denied!");

        When(x => x.totalamount != null && x.totalamount != 0, () =>
        {
            RuleFor(x => x.items)
                .NotNull().WithMessage("Items must be provided when totalamount specified.")
                .Must(items => items?.Count > 0).WithMessage("At least 1 item must be provided.");
        });

        When(x => x.items != null && x.items.Count > 0, () =>
        {
            RuleForEach(x => x.items).ChildRules(items =>
            {
                items.RuleFor(x => x.partneritemref).NotNull().Length(1, 50);
                items.RuleFor(x => x.name).NotNull().Length(1, 100);
                items.RuleFor(x => x.qty).NotNull().ExclusiveBetween(0, 6);
                items.RuleFor(x => x.unitprice).NotNull().GreaterThanOrEqualTo(0);
            });
        });
    }
}

