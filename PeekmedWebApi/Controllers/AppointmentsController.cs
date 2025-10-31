using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using PeekmedWebApi.Data;
// Thêm các using sau
using PeekmedWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OData.Deltas;

namespace PeekmedWebApi.Controllers;

public class AppointmentsController : ODataController
{
    private readonly PeekMedDbContext _db;
    public AppointmentsController(PeekMedDbContext db) => _db = db;

    // READ (Đã có)
    [EnableQuery]
    public IActionResult Get() => Ok(_db.Appointments);

    [EnableQuery]
    public IActionResult Get(int key) => Ok(_db.Appointments.FirstOrDefault(a => a.AppointmentId == key));

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