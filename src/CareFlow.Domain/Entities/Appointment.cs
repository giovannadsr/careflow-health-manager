namespace CareFlow.Domain.Entities;

using CareFlow.Domain.Enums;

public class Appointment
{
    public Guid Id { get; set; }

    public Guid PatientId { get; set; }

    public Guid DoctorId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public AppointmentStatus Status { get; set; }
    = AppointmentStatus.Scheduled;

    public string? Notes { get; set; }

    public Patient Patient { get; set; } = null!;

    public Doctor Doctor { get; set; } = null!;
}