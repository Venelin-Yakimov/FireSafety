namespace FireSafety.Model.Entities;

public sealed class FireSpecialistProfileView
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int FireServiceTypeId { get; set; }
    public string FireServiceTypeName { get; set; } = string.Empty;
    public decimal AverageRating { get; set; }
    public long ReviewCount { get; set; }
}
