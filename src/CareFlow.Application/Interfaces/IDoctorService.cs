using CareFlow.Application.DTOs.Doctors;

namespace CareFlow.Application.Interfaces;

public interface IDoctorService
{
    Task<IEnumerable<DoctorResponseDto>> GetAllAsync();

    Task<DoctorResponseDto?> GetByIdAsync(Guid id);

    Task<DoctorResponseDto> CreateAsync(CreateDoctorDto dto);

    Task UpdateAsync(Guid id, UpdateDoctorDto dto);

    Task DeleteAsync(Guid id);
}