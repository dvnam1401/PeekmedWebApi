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
    public QueuesController(PeekMedDbContext db) => _db = db;

    // READ (Đã có)
    [EnableQuery]
    public IActionResult Get() => Ok(_db.Queues);

    [EnableQuery]
    public IActionResult Get(int key) => Ok(_db.Queues.FirstOrDefault(q => q.QueueId == key));

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