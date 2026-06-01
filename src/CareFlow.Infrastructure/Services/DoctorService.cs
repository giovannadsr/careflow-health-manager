using CareFlow.Application.DTOs.Doctors;
using CareFlow.Application.Interfaces;
using CareFlow.Domain.Entities;
using CareFlow.Domain.Exceptions;
using CareFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareFlow.Infrastructure.Services;

public class DoctorService : IDoctorService
{
    private readonly AppDbContext _context;

    public DoctorService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DoctorResponseDto>> GetAllAsync()
    {
        return await _context.Doctors
            .Select(d => new DoctorResponseDto
            {
                Id = d.Id,
                FullName = d.FullName,
                CRM = d.CRM,
                Specialty = d.Specialty,
                IsActive = d.IsActive
            })
            .ToListAsync();
    }

    public async Task<DoctorResponseDto?> GetByIdAsync(Guid id)
    {
        var doctor = await _context.Doctors.FindAsync(id);

        if (doctor == null)
            return null;

        return new DoctorResponseDto
        {
            Id = doctor.Id,
            FullName = doctor.FullName,
            CRM = doctor.CRM,
            Specialty = doctor.Specialty,
            IsActive = doctor.IsActive
        };
    }

    public async Task<DoctorResponseDto> CreateAsync(CreateDoctorDto dto)
    {
        var doctor = new Doctor
        {
            Id = Guid.NewGuid(),
            FullName = dto.FullName,
            CRM = dto.CRM,
            Specialty = dto.Specialty,
            IsActive = true
        };

        _context.Doctors.Add(doctor);

        await _context.SaveChangesAsync();

        return new DoctorResponseDto
        {
            Id = doctor.Id,
            FullName = doctor.FullName,
            CRM = doctor.CRM,
            Specialty = doctor.Specialty,
            IsActive = doctor.IsActive
        };
    }

    public async Task UpdateAsync(Guid id, UpdateDoctorDto dto)
    {
        var doctor = await _context.Doctors.FindAsync(id);

        if (doctor == null)
            throw new NotFoundException("Médico não encontrado.");

        doctor.FullName = dto.FullName;
        doctor.Specialty = dto.Specialty;
        doctor.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var doctor = await _context.Doctors.FindAsync(id);

        if (doctor == null)
            throw new NotFoundException("Médico não encontrado.");

        _context.Doctors.Remove(doctor);

        await _context.SaveChangesAsync();
    }
}