using CareFlow.Application.DTOs.Doctors;
using CareFlow.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CareFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly IDoctorService _service;

    public DoctorsController(IDoctorService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var doctor = await _service.GetByIdAsync(id);

        if (doctor == null)
            return NotFound();

        return Ok(doctor);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateDoctorDto dto)
    {
        var doctor = await _service.CreateAsync(dto);

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
        await _service.UpdateAsync(id, dto);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);

        return NoContent();
    }
}