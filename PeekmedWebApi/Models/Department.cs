using System;
using System.Collections.Generic;

namespace PeekmedWebApi.Models;

public partial class Department
{
    public int DepartmentId { get; set; }

    public int HospitalId { get; set; }

    public string? DepartmentName { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

    public virtual Hospital Hospital { get; set; } = null!;
}
