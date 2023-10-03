
using AppCore.Models;
using MainData.Entities;
using System.ComponentModel.DataAnnotations;

namespace API_FFMS.Dtos;
public class TransportCreateDto
{
    public string RequestCode { get; set; } = null!;
    [Required(ErrorMessage = "RequestedDate is required")]
    public DateTime? RequestedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string? Description { get; set; }
    public string? Notes{ get; set; }
    public int? Quantity{ get; set; }
    [Required(ErrorMessage = "Assignee  is required")]
    public Guid AssignedTo { get; set; }
    [Required(ErrorMessage = "Asset is required")]
    public List<Guid> AssetId { get; set; }
    [Required(ErrorMessage = "Room is required")]
    public Guid ToRoomId { get; set; }
    public bool IsInternal { get; set; }
}

public class TransportUpdateDto
{
    [FutureDate(ErrorMessage = "Scheduled Date must be in the future")]
    public DateTime? RequestDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string? Description { get; set; }
    public string? Notes{ get; set; }
    public int? Quantity{ get; set; }
    public Guid? AssignedTo { get; set; }
    public List<Guid>? AssetId { get; set; }
    public Guid? ToRoomId { get; set; }
}

public class TransportRequestDto : BaseDto
{
    public string RequestCode { get; set; } = null!;
    public DateTime? RequestedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string? Description { get; set; }
    public string? Notes{ get; set; }
    public int? Quantity{ get; set; }
    public RequestType? RequestType { get; set; }
    public RequestStatus? RequestStatus { get; set; }
    public bool IsInternal { get; set; }
    public Guid? AssignedTo { get; set; }
    public AssigneeTransportDto? PersonInCharge { get; set; }
    public TransportDetailDto? TransportDetail { get; set; }
}

public class TransportDetailDto
{
    public int? Quantity { get; set; }
    public Guid? ToRoomId { get; set; }
    public List<AssetTransportDto>? Assets { get; set; }
    public RoomTransportDto? ToRoom { get; set; }
}

public class AssigneeTransportDto
{
    public string Fullname { get; set; } = null!;
}

public class RoomTransportDto
{
    public string? RoomName { get; set; }
    public string RoomCode { get; set; } = null!;
}

public class AssetTransportDto
{
    public string AssetName { get; set; } = null!;
    public string? AssetCode { get; set; }
    public AssetStatus? Status { get; set; }
    public int? ManufacturingYear { get; set; }
    public string? SerialNumber { get; set; }
    public double Quantity { get; set; }
    public string? Description { get; set; }
}

public class TransportQueryDto : BaseQueryDto
{
    public DateTime? RequestedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public Guid? AssignedTo { get; set; }
    public Guid? AssetId { get; set; }
    public Guid? ToRoomId { get; set; }
}
public class TransportUpdateStatusDto
{
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