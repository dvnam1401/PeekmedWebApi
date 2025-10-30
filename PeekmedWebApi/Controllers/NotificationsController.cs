using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using PeekmedWebApi.Data;

namespace PeekmedWebApi.Controllers;

public class NotificationsController : ODataController
{
    private readonly PeekMedDbContext _db;
    public NotificationsController(PeekMedDbContext db) => _db = db;

    [EnableQuery]
    public IActionResult Get() => Ok(_db.Notifications);

    [EnableQuery]
    public IActionResult Get(int key) => Ok(_db.Notifications.FirstOrDefault(n => n.NotificationId == key));
}