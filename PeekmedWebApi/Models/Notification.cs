using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PeekmedWebApi.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public string? Title { get; set; }

    public string? Message { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? CreatedAt { get; set; }

    // ✅ Thêm JsonIgnore để tránh circular reference và giảm payload size
    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}
