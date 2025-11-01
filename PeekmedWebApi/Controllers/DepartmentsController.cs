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
    private readonly ILogger<DepartmentsController> _logger;

    public DepartmentsController(PeekMedDbContext db, ILogger<DepartmentsController> logger)
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
            _logger.LogInformation("Getting all departments...");
            var departments = await _db.Departments.AsNoTracking().ToListAsync();
            _logger.LogInformation($"Found {departments.Count} departments");
            return Ok(departments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting departments");
            return StatusCode(500, new { error = "Internal server error", details = ex.Message });
        }
    }

    [EnableQuery]
    public async Task<IActionResult> Get(int key)
    {
        try
        {
            var department = await _db.Departments.AsNoTracking().FirstOrDefaultAsync(d => d.DepartmentId == key);
            if (department == null)
                return NotFound();
            return Ok(department);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting department {key}");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

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