using System.ComponentModel.DataAnnotations;

namespace FireSafety.Api.View.DTOs;

/// <summary>Вид услуга, който се връща от API.</summary>
public sealed record FireServiceTypeDto(int FireServiceTypeId, string Name);

/// <summary>Данни за създаване или преименуване на вид услуга.</summary>
public sealed class FireServiceTypeUpsertDto
{
    [Required(ErrorMessage = "Името на услугата е задължително.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Името трябва да е между 2 и 100 символа.")]
    public string Name { get; set; } = string.Empty;
}
