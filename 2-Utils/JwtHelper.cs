using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Talent;

public static class JwtHelper
{
    // Symmetric security key used for signing JWT tokens
    private static readonly SymmetricSecurityKey _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AppConfig.JwtKey));

    // JWT token handler
    private static readonly JwtSecurityTokenHandler _handler = new JwtSecurityTokenHandler();

    // Generates a new JWT token for the given user
    public static string GetNewToken(User user)
    {
        // Create a slim version of the user object to include in the token
        var slimUser = new { user.Id, user.Name, user.Email, user.RoleId, Role = user.Role?.RoleName };
        string json = JsonSerializer.Serialize(slimUser, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        // Create claims for the token
        List<Claim> claims = new List<Claim> {
            new Claim("user", json),
            new Claim(ClaimTypes.Role, user.Role?.RoleName)
        };

        // Create a security token descriptor
        SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(AppConfig.JwtKeyExpire),
            SigningCredentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha512)
        };

        // Create and write the token
        SecurityToken securityToken = _handler.CreateToken(descriptor);
        string token = _handler.WriteToken(securityToken);
        return token;
    }

    // Configures JWT bearer options
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
