namespace TransactionApi;

public class PrimeCheckService
{
    private readonly IAppLogger _logger;

    public PrimeCheckService(IAppLogger logger)
    {
        _logger = logger;
    }

    public async Task<bool> IsPrime(long? number)
    {
        _logger.Debug($"Checking primality for {number}");

        if (number <= 1)
        {
            return false;
        }


        if (number == 2)
        {
            return true;
        }

        if (number % 2 == 0)
        {
            return false;
        }

        return number < 1_000_000
             ? await Task.Run(() => TrialDivisionCheck(number))
             : await Task.Run(() => MillerRabinCheck(number));
    }

    private bool TrialDivisionCheck(long? n)
    {
        _logger.Debug($"Using trial division for {n}");
        for (long i = 3; i * i <= n; i += 2)
        {
            if (n % i == 0)
            {
                _logger.Trace($"Divisible by {i}");
                return false;
            }
        }

        return true;
    }

    private bool MillerRabinCheck(long? n)
    {
        _logger.Debug($"Using Miller-Rabin for {n}");
        long[] bases = { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37 };

        long? d = n - 1;
        int s = 0;
        while (d % 2 == 0)
        {
            d /= 2;
            s++;
        }

        foreach (long a in bases)
        {
            if (a >= n) continue;

            long? x = ModPow(a, d, n);
            if (x == 1 || x == n - 1) continue;

            for (int i = 0; i < s - 1; i++)
            {
                x = ModPow(x, 2, n);
                if (x == n - 1) break;
                if (i == s - 2) return false;
            }
        }

        return true;
    }

    private long? ModPow(long? a, long? b, long? mod)
    {
        long? result = 1;
        a %= mod;
        while (b > 0)
        {
            if ((b & 1) == 1)
                result = (result * a) % mod;
            a = (a * a) % mod;
            b >>= 1;
        }

        return result;
    }
}
