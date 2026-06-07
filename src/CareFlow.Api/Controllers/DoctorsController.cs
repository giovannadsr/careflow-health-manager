using CareFlow.Application.DTOs.Doctors;
using CareFlow.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CareFlow.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly IDoctorService _service;

    public DoctorsController(IDoctorService service)
    {
        _service = service;
    }

    [Authorize(Roles = "Admin, Recepcionist")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [Authorize(Roles = "Admin, Recepcionist")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var doctor = await _service.GetByIdAsync(id);

        if (doctor == null)
            return NotFound();

        return Ok(doctor);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateDoctorDto dto)
    {
        var doctor = await _service.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = doctor.Id },
            doctor);
    }


    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateDoctorDto dto)
    {
        await _service.UpdateAsync(id, dto);

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);

        return NoContent();
    }
}