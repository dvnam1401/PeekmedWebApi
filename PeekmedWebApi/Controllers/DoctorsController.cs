using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using PeekmedWebApi.Data;
// Thêm các using sau
using PeekmedWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OData.Deltas;

namespace PeekmedWebApi.Controllers;

public class DoctorsController : ODataController
{
    private readonly PeekMedDbContext _db;
    private readonly ILogger<DoctorsController> _logger;

    public DoctorsController(PeekMedDbContext db, ILogger<DoctorsController> logger)
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
            _logger.LogInformation("Getting all doctors...");
            var doctors = await _db.Doctors.AsNoTracking().ToListAsync();
            _logger.LogInformation($"Found {doctors.Count} doctors");
            return Ok(doctors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting doctors");
            return StatusCode(500, new { error = "Internal server error", details = ex.Message });
        }
    }

    [EnableQuery]
    public async Task<IActionResult> Get(int key)
    {
        try
        {
            var doctor = await _db.Doctors.AsNoTracking().FirstOrDefaultAsync(d => d.DoctorId == key);
            if (doctor == null)
                return NotFound();
            return Ok(doctor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting doctor {key}");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    // CREATE (Thêm mới)
    public async Task<IActionResult> Post([FromBody] Doctor doctor)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _db.Doctors.Add(doctor);
        await _db.SaveChangesAsync();

        return Created(doctor);
    }

    // UPDATE (Cập nhật một phần - PATCH)
    public async Task<IActionResult> Patch(int key, [FromBody] Delta<Doctor> patch)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var doctor = await _db.Doctors.FindAsync(key);
        if (doctor == null)
        {
            return NotFound();
        }

        patch.Patch(doctor); // Áp dụng các thay đổi

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_db.Doctors.Any(d => d.DoctorId == key))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return Updated(doctor);
    }

    // DELETE (Xóa)
    public async Task<IActionResult> Delete(int key)
    {
        var doctor = await _db.Doctors.FindAsync(key);
        if (doctor == null)
        {
            return NotFound();
        }

        _db.Doctors.Remove(doctor);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}