using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using PeekmedWebApi.Data;
// Thêm các using sau
using PeekmedWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OData.Deltas;

namespace PeekmedWebApi.Controllers;

public class UsersController : ODataController
{
    private readonly PeekMedDbContext _db;
    public UsersController(PeekMedDbContext db) => _db = db;

    // READ (Đã có)
    [EnableQuery]
    public IActionResult Get() => Ok(_db.Users);

    [EnableQuery]
    public IActionResult Get(int key) => Ok(_db.Users.FirstOrDefault(u => u.UserId == key));

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