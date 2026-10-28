namespace FireSafety.Model.Entities;

public class Review
{
    public int ReviewId { get; set; }

    public int SafetyRequestId { get; set; }

    public SafetyRequest SafetyRequest { get; set; } = null!;

    // Оценка от 1 до 5.
    public byte Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }
}