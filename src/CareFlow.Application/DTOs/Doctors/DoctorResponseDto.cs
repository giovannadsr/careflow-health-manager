namespace CareFlow.Application.DTOs.Doctors;

public class DoctorResponseDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string CRM { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}