using CareFlow.Application.DTOs.Patients;
using CareFlow.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareFlow.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _service;

    public PatientsController(IPatientService service)
    {
        _service = service;
    }

    [Authorize(Roles = "Admin, Doctor, Recepcionist")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [Authorize(Roles = "Admin, Doctor, Recepcionist")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var patient = await _service.GetByIdAsync(id);

        if (patient == null)
            return NotFound();

        return Ok(patient);
    }


    [Authorize(Roles = "Admin, Recepcionist")]
    [HttpPost]
    public async Task<IActionResult> Create(CreatePatientDto dto)
    {
        var patient = await _service.CreateAsync(dto);

        return Ok(patient);
    }

    [Authorize(Roles = "Admin, Recepcionist")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdatePatientDto dto)
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