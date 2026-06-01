using CareFlow.Application.DTOs.Appointments;

namespace CareFlow.Application.Interfaces;

public interface IAppointmentService
{
    Task<IEnumerable<AppointmentResponseDto>> GetAllAsync();
    Task<AppointmentResponseDto?> GetByIdAsync(Guid id);
    Task<AppointmentResponseDto> CreateAsync(CreateAppointmentDto dto);
    Task UpdateAsync(Guid id, UpdateAppointmentDto dto);
    Task DeleteAsync(Guid id);
    Task ConfirmAsync(Guid id);
    Task CompleteAsync(Guid id);
    Task StartAsync(Guid id);
    Task CancelAsync(Guid id);
}