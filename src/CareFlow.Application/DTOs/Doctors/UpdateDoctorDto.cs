namespace CareFlow.Application.DTOs.Doctors;

public class UpdateDoctorDto
{
    public string FullName { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}