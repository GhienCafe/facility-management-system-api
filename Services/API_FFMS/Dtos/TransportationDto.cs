
using AppCore.Models;
using MainData.Entities;
using System.ComponentModel.DataAnnotations;

namespace API_FFMS.Dtos;
public class TransportCreateDto
{
    [Required]
    public DateTime? ScheduledDate { get; set; }
    [Required]
    public DateTime? ActualDate { get; set; }
    public string? Description { get; set; }
    [Required]
    public Guid? AssignedTo { get; set; }
    [Required]
    public Guid? AssetId { get; set; }
    [Required]
    public Guid? ToRoomId { get; set; }
}

public class TransportUpdateDto
{
    public DateTime? ScheduledDate { get; set; }
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