using AppCore.Configs;
using AppCore.Models;
using MainData.Entities;
using Newtonsoft.Json;

namespace API_FFMS.Dtos;

public class InventoryCheckDto : BaseDto
{
    public string? RequestCode { get; set; }

    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime? RequestDate { get; set; }

    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime? CompletionDate { get; set; }
    public RequestType Type { get; set; }
    public EnumValue? TypeObj { get; set; }
    public RequestStatus? Status { get; set; }
    public EnumValue? StatusObj { get; set; }
    public Priority? Priority { get; set; }
    public EnumValue? PriorityObj { get; set; }
    public string? Result { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public bool? IsInternal { get; set; }
    public Guid? AssignedTo { get; set; }

    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime? Checkin { get; set; }

    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime? Checkout { get; set; }

    public List<RoomInventoryCheckDto>? Rooms { get; set; }
    //public RoomBaseDto? Room { get; set; }
    public MediaFileDto? MediaFile { get; set; }
    public AssignedInventoryCheckDto? Staff { get; set; }
}


public class AssetInventoryCheckDto
{
    public Guid Id { get; set; }
    public string? AssetName { get; set; }
    public string? AssetCode { get; set; }
    public double? QuantityReported { get; set; }
    public double? QuantityBefore { get; set; }
    public AssetStatus? StatusBefore { get; set; }
    public EnumValue? StatusBeforeObj { get; set; }
    public AssetStatus StatusReported { get; set; }
    public EnumValue? StatusReportedObj { get; set; }
}

public class RoomInventoryCheckDto
{
    public Guid? Id { get; set; }
    public string? RoomName { get; set; }
    public double? Area { get; set; }
    public string? RoomCode { get; set; }
    public Guid FloorId { get; set; }
    public Guid StatusId { get; set; }
    public RoomStatusInvenDto? Status { get; set; }
    public List<AssetInventoryCheckDto>? Assets { get; set; }
}

public class AssignedInventoryCheckDto
{
    public Guid Id { get; set; }
    public string? UserCode { get; set; }
    public string? Fullname { get; set; }
    public string? Avatar { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? PersonalIdentifyNumber { get; set; }
    public UserRole? Role { get; set; }
    public EnumValue? RoleObj { get; set; }
}

public class InventoryCheckQueryDto : BaseRequestQueryDto { }

public class InventoryCheckCreateDto
{
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public bool IsInternal { get; set; }
    public Priority Priority { get; set; } = Priority.Medium;
    public required Guid AssignedTo { get; set; }
    public List<RoomInvenCheckCretaeDto>? Rooms { get; set; }
    public List<MediaFileCreateDto>? RelatedFiles { get; set; }
}

public class RoomInvenCheckCretaeDto
{
    public required Guid RoomId { get; set; }
}

public class InventoryCheckConfigDto : BaseDto
{
    public int? CheckPeriod { get; set; }
    public string? Description { get; set; }
}

public class InventoryCheckConfigCreateDto
{
    public int? CheckPeriod { get; set; }
    public string? Description { get; set; }
}

public class NextInventoryCheckDto
{
    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime? NextInventoryCheck { get; set; }
}

public class InventoryCheckConfigUpdateDto
{
    public int? CheckPeriod { get; set; }
    public string? Description { get; set; }
}

public class InventoryCheckConfigQueryDto : BaseQueryDto
{
    public int? CheckPeriod { get; set; }
    public string? Description { get; set; }
}