using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PeekmedWebApi.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? FirstName { get; set; }

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }

    // ✅ Thay DateOnly? thành DateTime? để tương thích với Azure
    public DateTime? DateOfBirth { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string? Sex { get; set; }

    public string? Email { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // ✅ Thêm JsonIgnore để tránh circular reference và giảm payload size
    [JsonIgnore]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [JsonIgnore]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
