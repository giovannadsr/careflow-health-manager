namespace CareFlow.Application.DTOs.Appointments;

public class CreateAppointmentDto
{
    public Guid PatientId { get; set; }

    public Guid DoctorId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public string? Notes { get; set; }
}