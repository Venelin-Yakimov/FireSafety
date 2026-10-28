namespace FireSafety.Model.Entities;

public class User
{
    public int UserId { get; set; }

    public string Email { get; set; } = string.Empty;

    // Пази се хешът на паролата, не самата парола.
    public string PasswordHash { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    // Телефонът се показва на специалиста след приемане на офертата.
    public string? Phone { get; set; }

    public UserRole Role { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<SafetyRequest> SafetyRequests { get; set; }
        = new List<SafetyRequest>();

    public ICollection<Offer> Offers { get; set; }
        = new List<Offer>();

    public ICollection<SpecialistFireServiceType> SpecialistFireServiceTypes { get; set; }
        = new List<SpecialistFireServiceType>();
}
