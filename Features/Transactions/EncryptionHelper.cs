using System.Security.Cryptography;
using System.Text;

public static class EncryptHelper
{
    // private static readonly byte[] Key = Encoding.UTF8.GetBytes("");
    // private static readonly byte[] IV = Encoding.UTF8.GetBytes("");
    //
    // static EncryptHelper()
    // {
    //     Key = Convert.FromBase64String("BASE64_ENCODED_256_BIT_KEY");
    //     IV = Convert.FromBase64String("BASE64_ENCODED_16_BIT_IV");
    // }
    //
    // public static async Task<string> EncryptAsync(string plainText)
    // {
    //     if (string.IsNullOrEmpty(plainText)) return string.Empty;
    //
    //     using (Aes aes = Aes.Create())
    //     {
    //         aes.Key = Key;
    //         aes.IV = IV;
    //
    //         using (var memoryStream = new MemoryStream())
    //         {
    //             using (var cryptoStream = new CryptoStream(
    //                         memoryStream,
    //                         aes.CreateEncryptor(),
    //                         CryptoStreamMode.Write))
    //             {
    //                 var plainBytes = Encoding.UTF8.GetBytes(plainText);
    //                 await cryptoStream.WriteAsync(plainBytes, 0, plainBytes.Length);
    //
    //                 await cryptoStream.FlushFinalBlockAsync();
    //             }
    //             return Convert.ToBase64String(memoryStream.ToArray());
    //         }
    //     }
    // }
}
