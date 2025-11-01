using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using PeekmedWebApi.Data;
// Thêm các using sau
using PeekmedWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OData.Deltas;

namespace PeekmedWebApi.Controllers;

public class HospitalsController : ODataController
{
    private readonly PeekMedDbContext _db;
    private readonly ILogger<HospitalsController> _logger;

    public HospitalsController(PeekMedDbContext db, ILogger<HospitalsController> logger)
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
            _logger.LogInformation("Getting all hospitals...");
            var hospitals = await _db.Hospitals.AsNoTracking().ToListAsync();
            _logger.LogInformation($"Found {hospitals.Count} hospitals");
            return Ok(hospitals);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting hospitals");
            return StatusCode(500, new { error = "Internal server error", details = ex.Message });
        }
    }

    [EnableQuery]
    public async Task<IActionResult> Get(int key)
    {
        try
        {
            var hospital = await _db.Hospitals.AsNoTracking().FirstOrDefaultAsync(h => h.HospitalId == key);
            if (hospital == null)
                return NotFound();
            return Ok(hospital);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting hospital {key}");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    // CREATE (Thêm mới)
    public async Task<IActionResult> Post([FromBody] Hospital hospital)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _db.Hospitals.Add(hospital);
        await _db.SaveChangesAsync();

        return Created(hospital);
    }

    // UPDATE (Cập nhật một phần - PATCH)
    public async Task<IActionResult> Patch(int key, [FromBody] Delta<Hospital> patch)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var hospital = await _db.Hospitals.FindAsync(key);
        if (hospital == null)
        {
            return NotFound();
        }

        patch.Patch(hospital); // Áp dụng các thay đổi

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_db.Hospitals.Any(h => h.HospitalId == key))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return Updated(hospital);
    }

    // DELETE (Xóa)
    public async Task<IActionResult> Delete(int key)
    {
        var hospital = await _db.Hospitals.FindAsync(key);
        if (hospital == null)
        {
            return NotFound();
        }

        _db.Hospitals.Remove(hospital);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}