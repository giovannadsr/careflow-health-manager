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
                CPF = x.CPF,
                MedicalRecordNumber = x.MedicalRecordNumber,
                Diagnosis = x.Diagnosis,
                PhoneNumber = x.PhoneNumber,
                EmergencyContact = x.EmergencyContact
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
            CPF = patient.CPF,
            MedicalRecordNumber = patient.MedicalRecordNumber,
            Diagnosis = patient.Diagnosis,
            PhoneNumber = patient.PhoneNumber,
            EmergencyContact = patient.EmergencyContact
        };
    }

    public async Task<PatientResponseDto> CreateAsync(CreatePatientDto dto)
    {
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            FullName = dto.FullName,
            BirthDate = dto.BirthDate,
            Gender = dto.Gender,
            CPF = dto.CPF,
            MedicalRecordNumber = dto.MedicalRecordNumber,
            Diagnosis = dto.Diagnosis,
            PhoneNumber = dto.PhoneNumber,
            EmergencyContact = dto.EmergencyContact
        };

        _context.Patients.Add(patient);

        await _context.SaveChangesAsync();

        return new PatientResponseDto
        {
            Id = patient.Id,
            FullName = patient.FullName,
            BirthDate = patient.BirthDate,
            Gender = patient.Gender,
            CPF = patient.CPF,
            MedicalRecordNumber = patient.MedicalRecordNumber,
            Diagnosis = patient.Diagnosis,
            PhoneNumber = patient.PhoneNumber,
            EmergencyContact = patient.EmergencyContact
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