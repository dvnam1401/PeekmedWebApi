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
    public DoctorsController(PeekMedDbContext db) => _db = db;

    // READ (Đã có)
    [EnableQuery]
    public IActionResult Get() => Ok(_db.Doctors);

    [EnableQuery]
    public IActionResult Get(int key) => Ok(_db.Doctors.FirstOrDefault(d => d.DoctorId == key));

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