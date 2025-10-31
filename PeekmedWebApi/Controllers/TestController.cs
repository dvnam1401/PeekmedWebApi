using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeekmedWebApi.Data;

namespace PeekmedWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly PeekMedDbContext _db;
    private readonly ILogger<TestController> _logger;

    public TestController(PeekMedDbContext db, ILogger<TestController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet("health")]
    public async Task<IActionResult> HealthCheck()
    {
        try
        {
            var count = await _db.Users.CountAsync();
            return Ok(new { 
                status = "healthy", 
                userCount = count, 
                timestamp = DateTime.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(500, new { 
                status = "unhealthy", 
                error = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }

    [HttpGet("users-simple")]
    public async Task<IActionResult> GetUsersSimple()
    {
        try
        {
            _logger.LogInformation("Getting simple users list...");
            
            var users = await _db.Users
                .Select(u => new {
                    u.UserId,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.PhoneNumber
                })
                .Take(10)
                .ToListAsync();
            
            _logger.LogInformation($"Retrieved {users.Count} users successfully");
            
            return Ok(new {
                count = users.Count,
                data = users,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting simple users");
            return StatusCode(500, new { 
                error = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }

    [HttpGet("users-full")]
    public async Task<IActionResult> GetUsersFullTest()
    {
        try
        {
            _logger.LogInformation("Getting full users list for testing...");
            
            var users = await _db.Users
                .AsNoTracking()
                .Take(5) // Giới hạn 5 records để test
                .ToListAsync();
            
            _logger.LogInformation($"Retrieved {users.Count} full users successfully");
            
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting full users");
            return StatusCode(500, new { 
                error = ex.Message,
                stackTrace = ex.StackTrace,
                timestamp = DateTime.UtcNow
            });
        }
    }

    [HttpGet("connection-test")]
    public async Task<IActionResult> TestConnection()
    {
        try
        {
            _logger.LogInformation("Testing database connection...");
            
            // Test basic connection
            var canConnect = await _db.Database.CanConnectAsync();
            
            if (!canConnect)
            {
                return StatusCode(500, new { 
                    status = "failed", 
                    message = "Cannot connect to database",
                    timestamp = DateTime.UtcNow
                });
            }

            // Test query execution
            var firstUser = await _db.Users.FirstOrDefaultAsync();
            
            return Ok(new {
                status = "success",
                canConnect = canConnect,
                hasData = firstUser != null,
                firstUserId = firstUser?.UserId,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Connection test failed");
            return StatusCode(500, new { 
                status = "failed",
                error = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }
}
