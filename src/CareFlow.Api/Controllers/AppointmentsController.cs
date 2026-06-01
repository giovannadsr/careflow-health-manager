using CareFlow.Application.DTOs.Appointments;
using CareFlow.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CareFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _service;

    public AppointmentsController(IAppointmentService service)
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
        var appointment = await _service.GetByIdAsync(id);

        if (appointment == null)
            return NotFound();

        return Ok(appointment);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAppointmentDto dto)
    {
        var appointment = await _service.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = appointment.Id },
            appointment);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateAppointmentDto dto)
    {
        await _service.UpdateAsync(id, dto);

        return NoContent();
    }

    [HttpPatch("{id}/confirm")]
    public async Task<IActionResult> Confirm(Guid id)
    {
        await _service.ConfirmAsync(id);

        return NoContent();
    }

    [HttpPatch("{id}/start")]
    public async Task<IActionResult> Start(Guid id)
    {
        await _service.StartAsync(id);

        return NoContent();
    }

    [HttpPatch("{id}/complete")]
    public async Task<IActionResult> Complete(Guid id)
    {
        await _service.CompleteAsync(id);

        return NoContent();
    }

    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        await _service.CancelAsync(id);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);

        return NoContent();
    }
}