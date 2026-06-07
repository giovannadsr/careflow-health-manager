public class CreatePatientDto
{
    public string FullName { get; set; } = string.Empty;

    public DateTime BirthDate { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string CPF { get; set; } = string.Empty;

    public string MedicalRecordNumber { get; set; } = string.Empty;

    public string Diagnosis { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string EmergencyContact { get; set; } = string.Empty;
}