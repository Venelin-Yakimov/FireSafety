namespace FireSafety.Model.Entities;

public sealed class OpenSafetyRequestView
{
    public int SafetyRequestId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int FireServiceTypeId { get; set; }
    public string FireServiceTypeName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
