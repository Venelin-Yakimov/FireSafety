namespace FireSafety.Model.Entities;

public class SafetyRequest
{
    public int SafetyRequestId { get; set; }

    public int ClientId { get; set; }

    public User Client { get; set; } = null!;

    public int FireServiceTypeId { get; set; }

    public FireServiceType FireServiceType { get; set; } = null!;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    // Точният адрес се връща само на изпълнителя на приетата оферта.
    public string ExactAddress { get; set; } = string.Empty;

    public SafetyRequestStatus Status { get; set; } = SafetyRequestStatus.Open;

    public DateTime CreatedAt { get; set; }

    public ICollection<Offer> Offers { get; set; } = new List<Offer>();

    public Review? Review { get; set; }
}