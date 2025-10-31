using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using PeekmedWebApi.Data;
// Thêm các using sau
using PeekmedWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OData.Deltas;

namespace PeekmedWebApi.Controllers;

public class NotificationsController : ODataController
{
    private readonly PeekMedDbContext _db;
    public NotificationsController(PeekMedDbContext db) => _db = db;

    // READ (Đã có)
    [EnableQuery]
    public IActionResult Get() => Ok(_db.Notifications);

    [EnableQuery]
    public IActionResult Get(int key) => Ok(_db.Notifications.FirstOrDefault(n => n.NotificationId == key));

    // CREATE (Thêm mới)
    public async Task<IActionResult> Post([FromBody] Notification notification)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _db.Notifications.Add(notification);
        await _db.SaveChangesAsync();

        return Created(notification);
    }

    // UPDATE (Cập nhật một phần - PATCH)
    public async Task<IActionResult> Patch(int key, [FromBody] Delta<Notification> patch)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var notification = await _db.Notifications.FindAsync(key);
        if (notification == null)
        {
            return NotFound();
        }

        patch.Patch(notification); // Áp dụng các thay đổi

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_db.Notifications.Any(n => n.NotificationId == key))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return Updated(notification);
    }

    // DELETE (Xóa)
    public async Task<IActionResult> Delete(int key)
    {
        var notification = await _db.Notifications.FindAsync(key);
        if (notification == null)
        {
            return NotFound();
        }

        _db.Notifications.Remove(notification);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}