using System.Security.Claims;
using FireSafety.Api.Infrastructure.Errors;

namespace FireSafety.Api.Infrastructure.Auth;

/// <summary>Стандартизира имената на данните в JWT токена.</summary>
public static class AuthClaims
{
    public const string UserId = "sub";
    public const string Name = "name";
    public const string Email = "email";
    public const string Role = "role";
}

/// <summary>Ролите, използвани в [Authorize(Roles = ...)].</summary>
public static class AppRoles
{
    public const string Admin = "Admin";
    public const string Client = "Client";
    public const string FireSpecialist = "FireSpecialist";
}

public static class ClaimsPrincipalExtensions
{
    /// <summary>ID-то се взема от подписания токен, а не от тялото на заявката.</summary>
    public static int GetUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirst(AuthClaims.UserId)?.Value;
        return int.TryParse(value, out var id)
            ? id
            : throw new AppException(StatusCodes.Status401Unauthorized, "Невалиден токен.");
    }
}
