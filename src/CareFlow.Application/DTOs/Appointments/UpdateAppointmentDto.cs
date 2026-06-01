namespace CareFlow.Application.DTOs.Appointments;

public class UpdateAppointmentDto
{
    public DateTime AppointmentDate { get; set; }

    public string? Notes { get; set; }
}