namespace DotnetDemo.Security;

public interface IPasswordHasher
{
    PasswordHashResult HashPassword(string password, int iterations);
    bool Verify(string password, string saltBase64, string hashBase64, int iterations);
}

public record PasswordHashResult(string Hash, string Salt);

