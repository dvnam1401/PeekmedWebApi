using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using PeekmedWebApi.Data;

namespace PeekmedWebApi.Controllers;

public class QueuesController : ODataController
{
    private readonly PeekMedDbContext _db;
    public QueuesController(PeekMedDbContext db) => _db = db;

    [EnableQuery]
    public IActionResult Get() => Ok(_db.Queues);

    [EnableQuery]
    public IActionResult Get(int key) => Ok(_db.Queues.FirstOrDefault(q => q.QueueId == key));
}