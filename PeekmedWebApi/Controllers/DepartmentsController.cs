using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using PeekmedWebApi.Data;

namespace PeekmedWebApi.Controllers;

public class DepartmentsController : ODataController
{
    private readonly PeekMedDbContext _db;
    public DepartmentsController(PeekMedDbContext db) => _db = db;

    [EnableQuery]
    public IActionResult Get() => Ok(_db.Departments);

    [EnableQuery]
    public IActionResult Get(int key) => Ok(_db.Departments.FirstOrDefault(d => d.DepartmentId == key));
}