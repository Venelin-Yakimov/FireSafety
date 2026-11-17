using System.Text;
using FireSafety.Api.Infrastructure.Errors;
using FireSafety.Model.Entities;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace FireSafety.Api.Infrastructure.Auth;

/// <summary>Създава подписан JWT токен след успешен вход.</summary>
public sealed class JwtTokenService(IConfiguration configuration)
{
    public (string Token, DateTime ExpiresAt) CreateToken(User user)
    {
        var keyValue = configuration["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(keyValue) || Encoding.UTF8.GetByteCount(keyValue) < 32)
        {
            throw new AppException(StatusCodes.Status500InternalServerError,
                "JWT ключът не е настроен или е по-къс от 32 байта.");
        }

        var expiresAt = DateTime.UtcNow.AddMinutes(configuration.GetValue("Jwt:ExpiresMinutes", 120));
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"],
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyValue)),
                SecurityAlgorithms.HmacSha256),
            Claims = new Dictionary<string, object>
            {
                [AuthClaims.UserId] = user.UserId.ToString(),
                [AuthClaims.Name] = user.FullName,
                [AuthClaims.Email] = user.Email,
                [AuthClaims.Role] = user.Role.ToString()
            }
        };

        var token = new JsonWebTokenHandler().CreateToken(descriptor);
        return (token, expiresAt);
    }
}
