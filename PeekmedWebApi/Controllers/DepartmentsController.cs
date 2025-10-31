using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using PeekmedWebApi.Data;
// Thêm các using sau
using PeekmedWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OData.Deltas;

namespace PeekmedWebApi.Controllers;

public class DepartmentsController : ODataController
{
    private readonly PeekMedDbContext _db;
    public DepartmentsController(PeekMedDbContext db) => _db = db;

    // READ (Đã có)
    [EnableQuery]
    public IActionResult Get() => Ok(_db.Departments);

    [EnableQuery]
    public IActionResult Get(int key) => Ok(_db.Departments.FirstOrDefault(d => d.DepartmentId == key));

    // CREATE (Thêm mới)
    public async Task<IActionResult> Post([FromBody] Department department)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _db.Departments.Add(department);
        await _db.SaveChangesAsync();

        return Created(department);
    }

    // UPDATE (Cập nhật một phần - PATCH)
    public async Task<IActionResult> Patch(int key, [FromBody] Delta<Department> patch)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var department = await _db.Departments.FindAsync(key);
        if (department == null)
        {
            return NotFound();
        }

        patch.Patch(department); // Áp dụng các thay đổi

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_db.Departments.Any(d => d.DepartmentId == key))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return Updated(department);
    }

    // DELETE (Xóa)
    public async Task<IActionResult> Delete(int key)
    {
        var department = await _db.Departments.FindAsync(key);
        if (department == null)
        {
            return NotFound();
        }

        _db.Departments.Remove(department);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}