using System.ComponentModel.DataAnnotations;

namespace PeekmedWebApi.Models;

public class CreateAppointmentDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int DoctorId { get; set; }

    [Required]
    public int DepartmentId { get; set; }

    [Required]
    public int HospitalId { get; set; }

    [Required]
    public DateTime AppointmentDateTime { get; set; }

    public string? ReasonForVisit { get; set; }

    public string Status { get; set; } = "Scheduled";
}
