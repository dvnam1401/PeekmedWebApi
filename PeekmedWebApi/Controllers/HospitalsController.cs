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
    public HospitalsController(PeekMedDbContext db) => _db = db;

    // READ (Đã có)
    [EnableQuery]
    public IActionResult Get() => Ok(_db.Hospitals);

    [EnableQuery]
    public IActionResult Get(int key) => Ok(_db.Hospitals.FirstOrDefault(h => h.HospitalId == key));

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