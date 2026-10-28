namespace FireSafety.Model.Entities;

public class Offer
{
    public int OfferId { get; set; }

    public int SafetyRequestId { get; set; }

    public SafetyRequest SafetyRequest { get; set; } = null!;

    public int FireSpecialistId { get; set; }

    public User FireSpecialist { get; set; } = null!;

    public decimal Price { get; set; }

    public string? Message { get; set; }

    public OfferStatus Status { get; set; } = OfferStatus.Pending;

    public DateTime CreatedAt { get; set; }
}