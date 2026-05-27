using CareFlow.Application.DTOs.Patients;
using CareFlow.Application.Interfaces;
using CareFlow.Domain.Entities;
using CareFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareFlow.Infrastructure.Services;

public class PatientService : IPatientService
{
    private readonly AppDbContext _context;

    public PatientService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PatientResponseDto>> GetAllAsync()
    {
        return await _context.Patients
            .Select(x => new PatientResponseDto
            {
                Id = x.Id,
                FullName = x.FullName,
                BirthDate = x.BirthDate,
                Gender = x.Gender,
                MedicalRecordNumber = x.MedicalRecordNumber,
                Diagnosis = x.Diagnosis
            })
            .ToListAsync();
    }

    public async Task<PatientResponseDto?> GetByIdAsync(Guid id)
    {
        var patient = await _context.Patients.FindAsync(id);

        if (patient == null)
            return null;

        return new PatientResponseDto
        {
            Id = patient.Id,
            FullName = patient.FullName,
            BirthDate = patient.BirthDate,
            Gender = patient.Gender,
            MedicalRecordNumber = patient.MedicalRecordNumber,
            Diagnosis = patient.Diagnosis
        };
    }

    public async Task<PatientResponseDto> CreateAsync(CreatePatientDto dto)
    {
        var patient = new Patient
        {
            FullName = dto.FullName,
            BirthDate = dto.BirthDate,
            Gender = dto.Gender,
            MedicalRecordNumber = dto.MedicalRecordNumber,
            Diagnosis = dto.Diagnosis
        };

        _context.Patients.Add(patient);

        await _context.SaveChangesAsync();

        return new PatientResponseDto
        {
            Id = patient.Id,
            FullName = patient.FullName,
            BirthDate = patient.BirthDate,
            Gender = patient.Gender,
            MedicalRecordNumber = patient.MedicalRecordNumber,
            Diagnosis = patient.Diagnosis
        };
    }

    public async Task UpdateAsync(Guid id, UpdatePatientDto dto)
    {
        var patient = await _context.Patients.FindAsync(id);

        if (patient == null)
            throw new Exception("Paciente não encontrado.");

        patient.FullName = dto.FullName;
        patient.Diagnosis = dto.Diagnosis;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var patient = await _context.Patients.FindAsync(id);

        if (patient == null)
            throw new Exception("Paciente não encontrado.");

        _context.Patients.Remove(patient);

        await _context.SaveChangesAsync();
    }
}