using API.Interfaces;
using API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Services;

public class TokenService(IConfiguration cfg) : ITokenService
{
    public string CreateToken(AppUser usr)
    {
        var tokenKey = GetTokenKey();
        if (tokenKey.Length < 128) throw new Exception("Token key is too short. Please use at least 128 characters");

        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(handler.CreateToken(GetDescriptor(usr, tokenKey)));
    }

    protected SecurityTokenDescriptor GetDescriptor(AppUser usr, string tokenKey) =>
        new()
        {
            Subject = this.CreateSubject(usr),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = CreateSigningCredentials(tokenKey)
        };

    protected string GetTokenKey() =>
        cfg["TokenKey"] ?? throw new Exception("Token key not configured");

    protected SigningCredentials CreateSigningCredentials(string tokenKey) =>
        new (
            key: new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
            algorithm: SecurityAlgorithms.HmacSha512Signature
        );

    protected ClaimsIdentity CreateSubject(AppUser usr) =>
        new ([
            new(ClaimTypes.NameIdentifier, usr.UserName),
        ]);
}