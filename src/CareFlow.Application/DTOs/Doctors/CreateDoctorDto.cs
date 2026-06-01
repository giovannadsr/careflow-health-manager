namespace CareFlow.Application.DTOs.Doctors;

public class CreateDoctorDto
{
    public string FullName { get; set; } = string.Empty;

    public string CRM { get; set; } = string.Empty;

    public string Specialty { get; set; } = string.Empty;
}