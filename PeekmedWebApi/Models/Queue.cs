using System;
using System.Collections.Generic;

namespace PeekmedWebApi.Models;

public partial class Queue
{
    public int QueueId { get; set; }

    public int AppointmentId { get; set; }

    public int? QueueNumber { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;
}
