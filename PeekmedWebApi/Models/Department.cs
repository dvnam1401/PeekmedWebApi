using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PeekmedWebApi.Models;

public partial class Department
{
    public int DepartmentId { get; set; }

    public int HospitalId { get; set; }

    public string? DepartmentName { get; set; }

    public string? Description { get; set; }

    // ✅ Thêm JsonIgnore để tránh circular reference và giảm payload size
    [JsonIgnore]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [JsonIgnore]
    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

    [JsonIgnore]
    public virtual Hospital Hospital { get; set; } = null!;
}
