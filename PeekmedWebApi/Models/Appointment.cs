using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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

    // ✅ Thêm JsonIgnore để tránh circular reference và giảm payload size
    [JsonIgnore]
    public virtual Department? Department { get; set; }

    [JsonIgnore]
    public virtual Doctor? Doctor { get; set; }

    [JsonIgnore]
    public virtual Hospital? Hospital { get; set; }

    [JsonIgnore]
    public virtual Queue? Queue { get; set; }

    [JsonIgnore]
    public virtual User? User { get; set; }
}
