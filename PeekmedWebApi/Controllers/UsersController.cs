using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using PeekmedWebApi.Data;
using PeekmedWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OData.Deltas;

namespace PeekmedWebApi.Controllers;

public class UsersController : ODataController
{
    private readonly PeekMedDbContext _db;
    private readonly ILogger<UsersController> _logger;

    public UsersController(PeekMedDbContext db, ILogger<UsersController> logger)
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
            _logger.LogInformation("Getting all users...");
            var users = await _db.Users.AsNoTracking().ToListAsync();
            _logger.LogInformation($"Found {users.Count} users");
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            return StatusCode(500, new { error = "Internal server error", details = ex.Message });
        }
    }

    [EnableQuery]
    public async Task<IActionResult> Get(int key)
    {
        try
        {
            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == key);
            if (user == null)
                return NotFound();
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting user {key}");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    // CREATE (Thêm mới)
    public async Task<IActionResult> Post([FromBody] User user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Created(user);
    }

    // UPDATE (Cập nhật một phần - PATCH)
    public async Task<IActionResult> Patch(int key, [FromBody] Delta<User> patch)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _db.Users.FindAsync(key);
        if (user == null)
        {
            return NotFound();
        }

        patch.Patch(user); // Áp dụng các thay đổi

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_db.Users.Any(u => u.UserId == key))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return Updated(user);
    }

    // DELETE (Xóa)
    public async Task<IActionResult> Delete(int key)
    {
        var user = await _db.Users.FindAsync(key);
        if (user == null)
        {
            return NotFound();
        }

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}