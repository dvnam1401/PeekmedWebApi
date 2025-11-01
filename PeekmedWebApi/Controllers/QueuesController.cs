using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using PeekmedWebApi.Data;
// Thêm các using sau
using PeekmedWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OData.Deltas;

namespace PeekmedWebApi.Controllers;

public class QueuesController : ODataController
{
    private readonly PeekMedDbContext _db;
    private readonly ILogger<QueuesController> _logger;

    public QueuesController(PeekMedDbContext db, ILogger<QueuesController> logger)
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
            _logger.LogInformation("Getting all queues...");
            var queues = await _db.Queues.AsNoTracking().ToListAsync();
            _logger.LogInformation($"Found {queues.Count} queues");
            return Ok(queues);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting queues");
            return StatusCode(500, new { error = "Internal server error", details = ex.Message });
        }
    }

    [EnableQuery]
    public async Task<IActionResult> Get(int key)
    {
        try
        {
            var queue = await _db.Queues.AsNoTracking().FirstOrDefaultAsync(q => q.QueueId == key);
            if (queue == null)
                return NotFound();
            return Ok(queue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting queue {key}");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    // CREATE (Thêm mới)
    public async Task<IActionResult> Post([FromBody] Queue queue)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _db.Queues.Add(queue);
        await _db.SaveChangesAsync();

        return Created(queue);
    }

    // UPDATE (Cập nhật một phần - PATCH)
    public async Task<IActionResult> Patch(int key, [FromBody] Delta<Queue> patch)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var queue = await _db.Queues.FindAsync(key);
        if (queue == null)
        {
            return NotFound();
        }

        patch.Patch(queue); // Áp dụng các thay đổi

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_db.Queues.Any(q => q.QueueId == key))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return Updated(queue);
    }

    // DELETE (Xóa)
    public async Task<IActionResult> Delete(int key)
    {
        var queue = await _db.Queues.FindAsync(key);
        if (queue == null)
        {
            return NotFound();
        }

        _db.Queues.Remove(queue);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}