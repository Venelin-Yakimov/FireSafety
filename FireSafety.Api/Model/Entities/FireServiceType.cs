namespace FireSafety.Model.Entities;

public class FireServiceType
{
    public int FireServiceTypeId { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<SpecialistFireServiceType> SpecialistFireServiceTypes { get; set; }
        = new List<SpecialistFireServiceType>();

    public ICollection<SafetyRequest> SafetyRequests { get; set; }
        = new List<SafetyRequest>();
}