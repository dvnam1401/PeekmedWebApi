using System;
using System.Collections.Generic;

namespace PeekmedWebApi.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int UserId { get; set; }

    public int DoctorId { get; set; }

    public int DepartmentId { get; set; }

    public int HospitalId { get; set; }

    public DateTime? AppointmentDateTime { get; set; }

    public string? ReasonForVisit { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Hospital Hospital { get; set; } = null!;

    public virtual Queue? Queue { get; set; }

    public virtual User User { get; set; } = null!;
}
