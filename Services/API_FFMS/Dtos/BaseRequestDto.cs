using AppCore.Configs;
using AppCore.Models;
using MainData.Entities;
using Newtonsoft.Json;

namespace API_FFMS.Dtos;

public class BaseRequestDto : BaseDto
{
    public Guid? AssetId { get; set; }
    public string? RequestCode { get; set; }
    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime? RequestDate { get; set; }
    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime? CompletionDate { get; set; }
    public RequestStatus? Status { get; set; }
    public EnumValue? StatusObj { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public string? Result { get; set; }
    public bool IsInternal { get; set; }
    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime? Checkin { get; set; }
    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime? Checkout { get; set; }
    public Guid? AssignedTo { get; set; }    
    public Guid? AssetTypeId { get; set; }
    public Guid? CategoryId { get; set; }
    
}

public class TransportDetailBaseDto : BaseDto
{
    public Guid? AssetId { get; set; }
    public Guid? TransportationId { get; set; }
    //public DateTime? RequestDate { get; set; }
    public int? Quantity { get; set; }
    
    public AssetBaseDto? Asset { get; set; }
}

public class BaseRequestCreateDto
{
    public Guid AssetId { get; set; }
    //public DateTime? RequestDate { get; set; }
    public DateTime? CompletionDate { get; set; }

    public string? Description { get; set; }
    public string? Notes { get; set; } // Results
    public bool IsInternal { get; set; }
    public Guid? AssignedTo { get; set; }    
    public Guid? AssetTypeId { get; set; }
    public Guid? CategoryId { get; set; }
}

public class BaseRequestQueryDto : BaseQueryDto
{
    public Guid? AssetId { get; set; }
    public RequestStatus? Status { get; set; }
    public bool? IsInternal { get; set; }
    public Guid? AssignedTo { get; set; }    
}

public class BaseRequestUpdateDto
{
    public DateTime? RequestDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    //public RequestStatus? Status { get; set; }
    public string? Description { get; set; }
    public bool? IsInternal { get; set; }
    public string? Notes { get; set; } // Results
    public Guid? AssignedTo { get; set; }
    public Guid? AssetTypeId { get; set; }
    public Guid? CategoryId { get; set;}
}

public class BaseTransportCreateDto
{
    public List<Guid>? AssetId { get; set; }
    public string RequestCode { get; set; } = null!;
    public DateTime? RequestDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public RequestStatus? Status { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; } // Results
    public bool IsInternal { get; set; }
    public Guid? AssignedTo { get; set; }
}

public class BaseUpdateStatusDto
{
    public RequestStatus? Status { get; set; }
}