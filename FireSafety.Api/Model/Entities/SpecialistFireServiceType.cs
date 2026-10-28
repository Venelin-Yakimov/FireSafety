namespace FireSafety.Model.Entities;

public class SpecialistFireServiceType
{
    public int SpecialistId { get; set; }

    public User Specialist { get; set; } = null!;

    public int FireServiceTypeId { get; set; }

    public FireServiceType FireServiceType { get; set; } = null!;
}