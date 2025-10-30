using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using PeekmedWebApi.Data;

namespace PeekmedWebApi.Controllers;

public class AppointmentsController : ODataController
{
    private readonly PeekMedDbContext _db;
    public AppointmentsController(PeekMedDbContext db) => _db = db;

    [EnableQuery]
    public IActionResult Get() => Ok(_db.Appointments);

    [EnableQuery]
    public IActionResult Get(int key) => Ok(_db.Appointments.FirstOrDefault(a => a.AppointmentId == key));
}