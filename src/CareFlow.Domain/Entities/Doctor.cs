namespace CareFlow.Domain.Entities;

public class Doctor
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string FullName { get; set; } = string.Empty;

    public string CRM { get; set; } = string.Empty;

    public string Specialty { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}