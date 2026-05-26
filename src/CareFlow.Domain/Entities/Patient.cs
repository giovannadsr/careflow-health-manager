namespace CareFlow.Domain.Entities;

public class Patient
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime BirthDate { get; set; }

    public string CPF { get; set; } = string.Empty;

    public string BloodType { get; set; } = string.Empty;

    public string Allergies { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string EmergencyContact { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}