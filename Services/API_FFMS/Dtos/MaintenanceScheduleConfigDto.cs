using AppCore.Configs;
using AppCore.Models;
using MainData.Entities;
using Newtonsoft.Json;

namespace API_FFMS.Dtos;

public class AssetMaintenanceDto : BaseDto
{
    public string AssetName { get; set; } = null!;
    public string? AssetCode { get; set; }
    public bool IsMovable { get; set; }
    public AssetStatus Status { get; set; }
    public EnumValue? StatusObj { get; set; }
    public int? ManufacturingYear { get; set; }
    public string? SerialNumber { get; set; }
    public double? Quantity { get; set; }
    public string? Description { get; set; }
    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime? LastMaintenanceTime { get; set; }
    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime? LastCheckedDate { get; set; }
    public Guid? TypeId { get; set; }
    public Guid? ModelId { get; set; }
    public Guid? CategoryId { get; set; }
    public bool? IsRented { get; set; }
    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime? StartDateOfUse { get; set; }
    public string? ImageUrl { get; set; }
    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime? NextMaintenanceDate { get; set; }
}

public class MaintenanceScheduleConfigDto : BaseDto
{
    public string? Code { get; set; }
    public int? RepeatIntervalInMonths { get; set; } 
    public string? Description { get; set; }
}

public class MaintenanceScheduleConfigCreateDto
{
    public int RepeatIntervalInMonths { get; set; } 
    public string? Description { get; set; }
    
    public IEnumerable<GuidIds>? AssetIds { get; set; }
}

public class GuidIds
{
    public Guid Id { get; set; }
}

public class MaintenanceScheduleConfigUpdateDto
{
    public int? RepeatIntervalInMonths { get; set; } 
    public string? Description { get; set; }
    
    public IEnumerable<GuidIds>? AssetIds { get; set; }
}

public class MaintenanceScheduleConfigDetailDto : BaseDto
{
    public string? Code { get; set; }
    public int? RepeatIntervalInMonths { get; set; } 
    public string? Description { get; set; }
    
    public IEnumerable<AssetBaseDto>? Assets { get; set; }
}

public class MaintenanceScheduleConfigQueryDto : BaseQueryDto
{
}

