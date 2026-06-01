using CareFlow.Domain.Enums;
namespace CareFlow.Application.DTOs.Appointments;

public class AppointmentResponseDto
{
    public Guid Id { get; set; }

    public Guid PatientId { get; set; }

    public Guid DoctorId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public AppointmentStatus Status { get; set; }

    public string? Notes { get; set; }
}