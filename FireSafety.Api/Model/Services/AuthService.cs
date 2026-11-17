using FireSafety.Api.Infrastructure.Auth;
using FireSafety.Api.Infrastructure.Errors;
using FireSafety.Api.View.DTOs;
using FireSafety.Model.Data;
using FireSafety.Model.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FireSafety.Model.Services;

/// <summary>Бизнес логика за регистрация, вход и данни за текущия потребител.</summary>
public sealed class AuthService(
    AppDbContext db,
    IPasswordHasher<User> passwordHasher,
    JwtTokenService jwtTokenService)
{
    private const string InvalidCredentials = "Грешен имейл или парола.";

    public async Task<UserDto> RegisterAsync(RegisterRequestDto dto)
    {
        var role = dto.Role switch
        {
            AppRoles.Client => UserRole.Client,
            AppRoles.FireSpecialist => UserRole.FireSpecialist,
            _ => throw new AppException(StatusCodes.Status400BadRequest,
                "Ролята трябва да е Client или FireSpecialist.")
        };

        var email = NormalizeEmail(dto.Email);
        if (await db.Users.AnyAsync(user => user.Email == email))
        {
            throw new AppException(StatusCodes.Status409Conflict,
                "Има регистриран потребител с този имейл.");
        }

        var user = new User
        {
            Email = email,
            FullName = dto.FullName.Trim(),
            Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim(),
            Role = role
        };

        user.PasswordHash = passwordHasher.HashPassword(user, dto.Password);
        db.Users.Add(user);
        await db.SaveChangesAsync();

        return ToDto(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var email = NormalizeEmail(dto.Email);
        var user = await db.Users.SingleOrDefaultAsync(candidate => candidate.Email == email);

        if (user is null || await VerifyPasswordAsync(user, dto.Password) == PasswordVerificationResult.Failed)
        {
            throw new AppException(StatusCodes.Status401Unauthorized, InvalidCredentials);
        }

        var (token, expiresAt) = jwtTokenService.CreateToken(user);
        return new AuthResponseDto(token, expiresAt, ToDto(user));
    }

    public async Task<UserDto> GetCurrentUserAsync(int userId)
    {
        var user = await db.Users.SingleOrDefaultAsync(candidate => candidate.UserId == userId)
            ?? throw new AppException(StatusCodes.Status401Unauthorized, "Потребителят не съществува.");

        return ToDto(user);
    }

    private async Task<PasswordVerificationResult> VerifyPasswordAsync(User user, string password)
    {
        try
        {
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = passwordHasher.HashPassword(user, password);
                await db.SaveChangesAsync();
            }

            return result;
        }
        catch (FormatException)
        {
            return PasswordVerificationResult.Failed;
        }
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private static UserDto ToDto(User user) =>
        new(user.UserId, user.Email, user.FullName, user.Role.ToString());
}
