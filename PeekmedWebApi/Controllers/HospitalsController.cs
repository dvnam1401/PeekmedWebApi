using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using PeekmedWebApi.Data;

namespace PeekmedWebApi.Controllers;

public class HospitalsController : ODataController
{
    private readonly PeekMedDbContext _db;
    public HospitalsController(PeekMedDbContext db) => _db = db;

    [EnableQuery]
    public IActionResult Get() => Ok(_db.Hospitals);

    [EnableQuery]
    public IActionResult Get(int key) => Ok(_db.Hospitals.FirstOrDefault(h => h.HospitalId == key));
}