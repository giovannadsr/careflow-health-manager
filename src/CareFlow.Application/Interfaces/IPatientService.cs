using CareFlow.Application.DTOs.Patients;

namespace CareFlow.Application.Interfaces;

public interface IPatientService
{
    Task<IEnumerable<PatientResponseDto>> GetAllAsync();

    Task<PatientResponseDto?> GetByIdAsync(Guid id);

    Task<PatientResponseDto> CreateAsync(CreatePatientDto dto);

    Task UpdateAsync(Guid id, UpdatePatientDto dto);

    Task DeleteAsync(Guid id);
}