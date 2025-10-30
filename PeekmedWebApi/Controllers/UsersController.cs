using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using PeekmedWebApi.Data;

namespace PeekmedWebApi.Controllers;

public class UsersController : ODataController
{
    private readonly PeekMedDbContext _db;
    public UsersController(PeekMedDbContext db) => _db = db;

    [EnableQuery]
    public IActionResult Get() => Ok(_db.Users);

    [EnableQuery]
    public IActionResult Get(int key) => Ok(_db.Users.FirstOrDefault(u => u.UserId == key));
}