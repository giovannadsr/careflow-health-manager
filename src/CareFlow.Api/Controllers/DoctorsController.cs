using CareFlow.Application.DTOs.Doctors;
using CareFlow.Domain.Entities;
using CareFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DoctorsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DoctorsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _context.Doctors.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var doctor = await _context.Doctors.FindAsync(id);

        if (doctor == null)
            return NotFound();

        return Ok(doctor);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateDoctorDto dto)
    {
        var doctor = new Doctor
        {
            FullName = dto.FullName,
            CRM = dto.CRM,
            Specialty = dto.Specialty
        };

        _context.Doctors.Add(doctor);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = doctor.Id },
            doctor);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateDoctorDto dto)
    {
        var doctor = await _context.Doctors.FindAsync(id);

        if (doctor == null)
            return NotFound();

        doctor.FullName = dto.FullName;
        doctor.Specialty = dto.Specialty;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var doctor = await _context.Doctors.FindAsync(id);

        if (doctor == null)
            return NotFound();

        doctor.IsActive = false;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}