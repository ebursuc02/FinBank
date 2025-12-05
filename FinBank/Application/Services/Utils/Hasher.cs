using System.Security.Cryptography;
using System.Text;

namespace Application.Services.Utils;

internal static class Hasher
{
    public static string ComputeSha256Base64(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = SHA256.HashData(bytes);
        return Convert.ToBase64String(hashBytes);
    }
}