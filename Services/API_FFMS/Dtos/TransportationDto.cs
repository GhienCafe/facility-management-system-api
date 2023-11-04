using AppCore.Configs;
using AppCore.Models;
using MainData.Entities;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace API_FFMS.Dtos;

public class TransportDto : BaseDto
{
    public string RequestCode { get; set; } = null!;
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
    public int? Quantity { get; set; }
    public DateTime? Checkin { get; set; }
    public DateTime? Checkout { get; set; }
    public Guid? FromRoomId { get; set; }
    public Guid? ToRoomId { get; set; } // For internal
    public Guid? AssignedTo { get; set; }
    public List<FromRoomAssetDto>? Assets { get; set; }
    public RoomBaseDto? ToRoom { get; set; }
    public MediaFileDto? MediaFile { get; set; }
    public UserBaseDto? AssignTo { get; set; }
}

public class FromRoomAssetDto
{
    public int? Quantity { get; set; }
    public RoomBaseDto? FromRoom { get; set; }
    public AssetBaseDto? Asset { get; set; }
}

public class TransportCreateDto
{
    public required List<AssetTransportDto> Assets { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public bool IsInternal { get; set; }
    public Priority Priority { get; set; } = Priority.Medium;

    [Required(ErrorMessage = "Vui lòng người phụ trách")]
    public required Guid AssignedTo { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn phòng muốn vận chuyển đến")]
    public required Guid ToRoomId { get; set; }
}

public class AssetTransportDto
{
    [Required(ErrorMessage = "Yêu cầu điền thiết bị")]
    public required Guid AssetId { get; set; }
    public string? AssetName { get; set; }
    public string? AssetCode { get; set; }
    public string? AssetType { get; set; }
    public double? Quantity { get; set; }
    public string? FromRoom { get; set; }
}

public class TransportationQueryDto : BaseRequestQueryDto { }