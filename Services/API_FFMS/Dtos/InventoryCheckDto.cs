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
    public string? Notes { get; set; } // Results
    public bool? IsInternal { get; set; }
    public Guid? RoomId { get; set; }
    public Guid? AssignedToId { get; set; }

    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime? Checkin { get; set; }

    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime? Checkout { get; set; }

    public List<AssetInventoryCheck>? Assets { get; set; }
    public RoomBaseDto? Room { get; set; }
    public MediaFileDto? MediaFile { get; set; }
    public UserBaseDto? AssignedTo { get; set; }
}

public class AssetInventoryCheck
{
    public string? AssetName { get; set; }
    public string? AssetCode { get; set; }
    public AssetStatus? Status { get; set; }
    public EnumValue? StatusObj { get; set; }
    public int? Quantity { get; set; }
}

public class InventoryCheckQueryDto : BaseRequestQueryDto { }

public class InventoryCheckCreateDto
{
    public required Guid InventoryCheckConfigId { get; set; }
    public required List<Guid> AssetIds { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public bool IsInternal { get; set; }
    public Priority Priority { get; set; } = Priority.Medium;

    public required Guid AssignedTo { get; set; }

    public required Guid RoomId { get; set; }
    //public MediaFileCreateDto? RelatedFile { get; set; }
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