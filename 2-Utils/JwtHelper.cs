using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Talent;

public static class JwtHelper
{
    private static readonly SymmetricSecurityKey _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AppConfig.JwtKey));
    private static readonly JwtSecurityTokenHandler _handler = new JwtSecurityTokenHandler();

    public static string GetNewToken(User user)
    {
        var slimUser = new { user.Id, user.Name, user.Email, user.RoleId, Role = user.Role?.RoleName };
        string json = JsonSerializer.Serialize(slimUser, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        List<Claim> claims = new List<Claim> {
            new Claim(ClaimTypes.Actor, json),
            new Claim(ClaimTypes.Role, user.Role?.RoleName)
        };

        SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(AppConfig.JwtKeyExpire),
            SigningCredentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha512)
        };

        SecurityToken securityToken = _handler.CreateToken(descriptor);
        string token = _handler.WriteToken(securityToken);
        return token;
    }

    public static void SetBearerOptions(JwtBearerOptions options)
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _symmetricSecurityKey
        };
    }
}
