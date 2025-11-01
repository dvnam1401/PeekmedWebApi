using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using PeekmedWebApi.Data;
using PeekmedWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OData.Deltas;

namespace PeekmedWebApi.Controllers;

public class AppointmentsController : ODataController
{
    private readonly PeekMedDbContext _db;
    private readonly ILogger<AppointmentsController> _logger;

    public AppointmentsController(PeekMedDbContext db, ILogger<AppointmentsController> logger)
    {
        _db = db;
        _logger = logger;
    }

    // ✅ Cải thiện GET với error handling và async
    [EnableQuery]
    public async Task<IActionResult> Get()
    {
        try
        {
            _logger.LogInformation("Getting all appointments...");
            var appointments = await _db.Appointments.AsNoTracking().ToListAsync();
            _logger.LogInformation($"Found {appointments.Count} appointments");
            return Ok(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting appointments");
            return StatusCode(500, new { error = "Internal server error", details = ex.Message });
        }
    }

    [EnableQuery]
    public async Task<IActionResult> Get(int key)
    {
        try
        {
            var appointment = await _db.Appointments.AsNoTracking().FirstOrDefaultAsync(a => a.AppointmentId == key);
            if (appointment == null)
                return NotFound();
            return Ok(appointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting appointment {key}");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    // CREATE (Thêm mới)
    public async Task<IActionResult> Post([FromBody] Appointment appointment)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Clear navigation properties to avoid validation issues
        appointment.User = null;
        appointment.Doctor = null;
        appointment.Hospital = null;
        appointment.Department = null;
        appointment.Queue = null;

        // Set timestamps
        appointment.CreatedAt = DateTime.UtcNow;
        appointment.UpdatedAt = DateTime.UtcNow;

        _db.Appointments.Add(appointment);
        await _db.SaveChangesAsync();

        return Created(appointment);
    }

    // UPDATE (Cập nhật một phần - PATCH)
    public async Task<IActionResult> Patch(int key, [FromBody] Delta<Appointment> patch)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var appointment = await _db.Appointments.FindAsync(key);
        if (appointment == null)
        {
            return NotFound();
        }

        patch.Patch(appointment); // Áp dụng các thay đổi

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_db.Appointments.Any(a => a.AppointmentId == key))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return Updated(appointment);
    }

    // DELETE (Xóa)
    public async Task<IActionResult> Delete(int key)
    {
        var appointment = await _db.Appointments.FindAsync(key);
        if (appointment == null)
        {
            return NotFound();
        }

        _db.Appointments.Remove(appointment);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}