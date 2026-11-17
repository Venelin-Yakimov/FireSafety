namespace FireSafety.Api.Infrastructure.Errors;

/// <summary>Бизнес грешка, която се връща към клиента с избран HTTP статус.</summary>
public sealed class AppException(int statusCode, string message) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
