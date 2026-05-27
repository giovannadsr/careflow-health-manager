namespace CareFlow.Application.DTOs.Patients;

public class CreatePatientDto
{
    public string FullName { get; set; } = string.Empty;

    public DateTime BirthDate { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string MedicalRecordNumber { get; set; } = string.Empty;

    public string Diagnosis { get; set; } = string.Empty;
}