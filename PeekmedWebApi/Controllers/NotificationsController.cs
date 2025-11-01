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
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(PeekMedDbContext db, ILogger<NotificationsController> logger)
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
            _logger.LogInformation("Getting all notifications...");
            var notifications = await _db.Notifications.AsNoTracking().ToListAsync();
            _logger.LogInformation($"Found {notifications.Count} notifications");
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notifications");
            return StatusCode(500, new { error = "Internal server error", details = ex.Message });
        }
    }

    [EnableQuery]
    public async Task<IActionResult> Get(int key)
    {
        try
        {
            var notification = await _db.Notifications.AsNoTracking().FirstOrDefaultAsync(n => n.NotificationId == key);
            if (notification == null)
                return NotFound();
            return Ok(notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting notification {key}");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

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