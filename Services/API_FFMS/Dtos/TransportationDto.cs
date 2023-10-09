
using AppCore.Models;
using MainData.Entities;
using System.ComponentModel.DataAnnotations;

namespace API_FFMS.Dtos;
public class TransportCreateDto
{
    [Required(ErrorMessage = "RequestedDate is required")]
    [FutureDate(ErrorMessage = "Scheduled Date must be in the future")]
    public DateTime? RequestedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string? Description { get; set; }
    public string? Note{ get; set; }
    public int? Quantity{ get; set; }
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
    public DateTime? RequestedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string? Description { get; set; }
    public string? Note{ get; set; }
    public int? Quantity{ get; set; }
    public Guid? AssignedTo { get; set; }
    //public Guid? AssetId { get; set; }
    public Guid? ToRoomId { get; set; }
}

public class TransportDetailDto : BaseDto
{
    public DateTime RequestedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string? Description { get; set; }
    public string? Note{ get; set; }
    public int? Quantity{ get; set; }
    public TransportationStatus Status { get; set; }
    public Guid? AssignedTo { get; set; }
    public Guid? AssetId { get; set; }
    public Guid? ToRoomId { get; set; }
    public AssetDto? Asset { get; set; }
}

public class TransportQueryDto : BaseQueryDto
{
    public DateTime? RequestedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public TransportationStatus? Status { get; set; }
    public Guid? AssignedTo { get; set; }
    public Guid? AssetId { get; set; }
    public Guid? ToRoomId { get; set; }
}

public class TransportDto : BaseDto
{
    public DateTime RequestedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string? Description { get; set; }
    public string? Note { get; set; }
    public EnumValue? Status { get; set; }
    public int? Quantity { get; set; }
    public Guid? AssignedTo { get; set; }
    public Guid? AssetId { get; set; }
    public Guid? ToRoomId { get; set; }
    public UserDto? PersonInCharge { get; set; }
    public RoomDto? FromRoom { get; set; }
    public RoomDto? ToRoom { get; set; }
    public AssetDto? Asset { get; set; }
}

public class TransportUpdateStatusDto
{
    public TransportationStatus Status { get; set; }
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