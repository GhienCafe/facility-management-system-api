using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos;

public class TransportDto : BaseDto
{
    public string RequestCode { get; set; } = null!;
    public DateTime? RequestDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public RequestStatus? Status { get; set; }
    public EnumValue? StatusObj { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; } // Results
    public bool IsInternal { get; set; }
    public int? Quantity { get; set; }
    public Guid? FromRoomId { get; set; }
    public Guid? ToRoomId { get; set; } // For internal
    public Guid? AssignedTo { get; set; }
    public List<FromRoomAssetDto>? Assets { get; set; }
    public RoomBaseDto? ToRoom { get; set; }
    
}

public class FromRoomAssetDto
{
    public RoomBaseDto? FromRoom { get; set; }
    public AssetBaseDto? Asset { get; set; }
}

public class TransportCreateDto
{
    public List<Guid>? AssetId { get; set; }
    public string RequestCode { get; set; } = null!;
    //public DateTime? RequestDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    //public RequestStatus? Status { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; } // Results
    public bool IsInternal { get; set; }
    public int? Quantity { get; set; }
    public Guid? AssignedTo { get; set; }
    public Guid? ToRoomId { get; set; }
}

public class TransportationQueryDto : BaseQueryDto
{
    public string? RequestCode { get; set; }
    public DateTime? RequestDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public RequestStatus? Status { get; set; }
    public bool? IsInternal { get; set; }
    public Guid? AssignedTo { get; set; }
}

public class TransportUpdateStatusDto
{
    public RequestStatus? Status { get; set; }
}