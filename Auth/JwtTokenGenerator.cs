using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace HumanResourceManager.Auth;

public static class JwtTokenGenerator
{
    public static string Generate(string username)
    {
        var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, username)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("a-string-secret-at-least-256-bits-long"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "localIssuer",
            audience: "localAudience",
            claims: claims,
            expires: DateTime.Now.AddDays(1.0),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}