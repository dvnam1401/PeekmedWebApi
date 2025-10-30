using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using PeekmedWebApi.Data;

namespace PeekmedWebApi.Controllers;

public class DoctorsController : ODataController
{
    private readonly PeekMedDbContext _db;
    public DoctorsController(PeekMedDbContext db) => _db = db;

    [EnableQuery]
    public IActionResult Get() => Ok(_db.Doctors);

    [EnableQuery]
    public IActionResult Get(int key) => Ok(_db.Doctors.FirstOrDefault(d => d.DoctorId == key));
}