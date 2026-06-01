using CareFlow.Application.DTOs.Appointments;
using CareFlow.Application.Interfaces;
using CareFlow.Domain.Entities;
using CareFlow.Domain.Exceptions;
using CareFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using CareFlow.Domain.Enums;

namespace CareFlow.Infrastructure.Services;

public class AppointmentService : IAppointmentService
{
    private readonly AppDbContext _context;

    public AppointmentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AppointmentResponseDto>> GetAllAsync()
    {
        return await _context.Appointments
            .Select(a => new AppointmentResponseDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                DoctorId = a.DoctorId,
                AppointmentDate = a.AppointmentDate,
                Status = a.Status,
                Notes = a.Notes
            })
            .ToListAsync();
    }

    public async Task<AppointmentResponseDto?> GetByIdAsync(Guid id)
    {
        var appointment = await _context.Appointments.FindAsync(id);

        if (appointment == null)
            return null;

        return new AppointmentResponseDto
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            DoctorId = appointment.DoctorId,
            AppointmentDate = appointment.AppointmentDate,
            Status = appointment.Status,
            Notes = appointment.Notes
        };
    }

    public async Task<AppointmentResponseDto> CreateAsync(CreateAppointmentDto dto)
    {
        var patientExists = await _context.Patients
            .AnyAsync(p => p.Id == dto.PatientId);

        if (!patientExists)
            throw new NotFoundException("Paciente não encontrado.");

        var doctorExists = await _context.Doctors
            .AnyAsync(d => d.Id == dto.DoctorId);

        if (!doctorExists)
            throw new NotFoundException("Médico não encontrado.");

        var conflict = await _context.Appointments.AnyAsync(a =>
            a.DoctorId == dto.DoctorId &&
            a.Status != AppointmentStatus.Cancelled &&
            a.AppointmentDate >= dto.AppointmentDate.AddMinutes(-30) &&
            a.AppointmentDate <= dto.AppointmentDate.AddMinutes(30));

        if (conflict)
            throw new Exception(
                "Já existe uma consulta agendada para este médico neste horário.");

        if (dto.AppointmentDate < DateTime.UtcNow.AddHours(1))
            throw new Exception(
                "Consultas devem ser agendadas com pelo menos 1 hora de antecedência.");

        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            PatientId = dto.PatientId,
            DoctorId = dto.DoctorId,
            AppointmentDate = dto.AppointmentDate,
            Status = AppointmentStatus.Scheduled,
            Notes = dto.Notes
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        return new AppointmentResponseDto
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            DoctorId = appointment.DoctorId,
            AppointmentDate = appointment.AppointmentDate,
            Status = appointment.Status,
            Notes = appointment.Notes
        };
    }

    public async Task UpdateAsync(Guid id, UpdateAppointmentDto dto)
    {
        var appointment = await _context.Appointments.FindAsync(id);

        if (appointment == null)
            throw new NotFoundException("Consulta não encontrada.");

        appointment.AppointmentDate = dto.AppointmentDate;
        appointment.Notes = dto.Notes;

        await _context.SaveChangesAsync();
    }

    public async Task ConfirmAsync(Guid id)
    {
        var appointment = await _context.Appointments.FindAsync(id);

        if (appointment == null)
            throw new NotFoundException("Consulta não encontrada");

        if (appointment.Status != AppointmentStatus.Scheduled)
            throw new Exception("Somente consultas agendadas podem ser confirmadas");

        appointment.Status = AppointmentStatus.Confirmed;

        await _context.SaveChangesAsync();
    }

    public async Task StartAsync(Guid id)
    {
        var appointment = await _context.Appointments.FindAsync(id);

        if (appointment.Status != AppointmentStatus.Confirmed)
            throw new Exception("Consulta precisa estar confirmada");

        appointment.Status = AppointmentStatus.InProgress;

        await _context.SaveChangesAsync();
    }

    public async Task CompleteAsync(Guid id)
    {
        var appointment = await _context.Appointments.FindAsync(id);

        if (appointment.Status != AppointmentStatus.InProgress)
            throw new Exception("Consulta precisa estar em andamento");

        appointment.Status = AppointmentStatus.Completed;

        await _context.SaveChangesAsync();
    }

    public async Task CancelAsync(Guid id)
    {
        var appointment = await _context.Appointments.FindAsync(id);

        if (appointment.AppointmentDate < DateTime.UtcNow.AddHours(2))
        {
            throw new Exception(
                "Cancelamento deve ocorrer com pelo menos 2 horas de antecedência.");
        }

        appointment.Status = AppointmentStatus.Cancelled;

        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(Guid id)
    {
        var appointment = await _context.Appointments.FindAsync(id);

        if (appointment == null)
            throw new NotFoundException("Consulta não encontrada.");

        _context.Appointments.Remove(appointment);

        await _context.SaveChangesAsync();
    }
}