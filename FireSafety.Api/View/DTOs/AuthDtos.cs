using System.ComponentModel.DataAnnotations;

namespace FireSafety.Api.View.DTOs;

/// <summary>Данни за регистрация на клиент или специалист.</summary>
public sealed class RegisterRequestDto
{
    [Required(ErrorMessage = "Имейлът е задължителен.")]
    [EmailAddress(ErrorMessage = "Невалиден имейл.")]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Паролата е задължителна.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Паролата трябва да е поне 8 символа.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Името е задължително.")]
    [StringLength(120)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(30)]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Ролята е задължителна.")]
    [RegularExpression("^(Client|FireSpecialist)$", ErrorMessage = "Ролята трябва да е Client или FireSpecialist.")]
    public string Role { get; set; } = string.Empty;
}

/// <summary>Данни за вход.</summary>
public sealed class LoginRequestDto
{
    [Required(ErrorMessage = "Имейлът е задължителен.")]
    [EmailAddress(ErrorMessage = "Невалиден имейл.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Паролата е задължителна.")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>Публични данни за потребителя. Телефонът и хешът на паролата не се връщат.</summary>
public sealed record UserDto(int UserId, string Email, string FullName, string Role);

/// <summary>Резултат от успешен вход.</summary>
public sealed record AuthResponseDto(string Token, DateTime ExpiresAt, UserDto User);
