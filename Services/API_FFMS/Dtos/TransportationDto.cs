
using AppCore.Models;
using MainData.Entities;
using System.ComponentModel.DataAnnotations;

namespace API_FFMS.Dtos;
public class TransportCreateDto
{
    [Required(ErrorMessage = "Scheduled Date is required")]
    [FutureDate(ErrorMessage = "Scheduled Date must be in the future")]
    public DateTime ScheduledDate { get; set; }
    [Required(ErrorMessage = "Scheduled Date is required")]
    [FutureDate(ErrorMessage = "Actual Date must be in the future")]
    public DateTime ActualDate { get; set; }
    public string? Description { get; set; }
    [Required(ErrorMessage = "Assignedee  is required")]
    public Guid AssignedTo { get; set; }
    [Required(ErrorMessage = "Asset is required")]
    public Guid AssetId { get; set; }
    [Required(ErrorMessage = "Room is required")]
    public Guid ToRoomId { get; set; }
}

public class TransportUpdateDto
{
    [FutureDate(ErrorMessage = "Scheduled Date must be in the future")]
    public DateTime? ScheduledDate { get; set; }
    [FutureDate(ErrorMessage = "Actual Date must be in the future")]
    public DateTime? ActualDate { get; set; }
    public string? Description { get; set; }
    public Guid? AssignedTo { get; set; }
    public Guid? AssetId { get; set; }
    public Guid? ToRoomId { get; set; }
}

public class TransportDetailDto : BaseDto
{
    public DateTime ScheduledDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public string? Description { get; set; }
    public TransportationStatus Status { get; set; }
    public Guid? AssignedTo { get; set; }
    public Guid? AssetId { get; set; }
    public Guid? ToRoomId { get; set; }
}

public class TransportQueryDto : BaseQueryDto
{
    public DateTime? ScheduledDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public TransportationStatus Status { get; set; }
    public Guid? AssignedTo { get; set; }
    public Guid? AssetId { get; set; }
    public Guid? ToRoomId { get; set; }
}

public class TransportDto : BaseDto
{
    public DateTime ScheduledDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public string? Description { get; set; }
    public TransportationStatus Status { get; set; }
    public Guid? AssignedTo { get; set; }
    public Guid? AssetId { get; set; }
    public Guid? ToRoomId { get; set; }


}

public class FutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value is DateTime date)
        {
            return date > DateTime.Now;
        }
        return false;
    }
}