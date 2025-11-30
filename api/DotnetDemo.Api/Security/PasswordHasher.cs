using System;
using System.Security.Cryptography;

namespace DotnetDemo.Security;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16; // 128 bits
    private const int KeySize = 32;  // 256 bits

    public PasswordHashResult HashPassword(string password, int iterations)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password is required", nameof(password));
        }

        if (iterations <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(iterations));
        }

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, KeySize);

        return new PasswordHashResult(
            Convert.ToBase64String(hash),
            Convert.ToBase64String(salt));
    }

    public bool Verify(string password, string saltBase64, string hashBase64, int iterations)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(saltBase64) || string.IsNullOrWhiteSpace(hashBase64))
        {
            return false;
        }

        if (!TryDecodeBase64(saltBase64, out var salt) || !TryDecodeBase64(hashBase64, out var expectedHash))
        {
            return false;
        }

        var computedHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expectedHash.Length);
        return CryptographicOperations.FixedTimeEquals(computedHash, expectedHash);
    }

    private static bool TryDecodeBase64(string input, out byte[] data)
    {
        try
        {
            data = Convert.FromBase64String(input);
            return true;
        }
        catch (FormatException)
        {
            data = Array.Empty<byte>();
            return false;
        }
    }
}

