﻿using AppCore.Models;
using MainData.Entities;
using AppCore.Configs;
using Newtonsoft.Json;

namespace API_FFMS.Dtos;
public class AssetDto : BaseDto
{
    public string AssetName { get; set; } = null!;
    public string? AssetCode { get; set; }
    public bool IsMovable { get; set; }
    public AssetStatus? Status { get; set; }
    public EnumValue? StatusObj { get; set; }
    public RequestType? RequestStatus { get; set; }
    public EnumValue? RequestStatusObj { get; set; }
    public int? ManufacturingYear { get; set; }
    public string? SerialNumber { get; set; }
    public double? Quantity { get; set; }
    public string? Description { get; set; }
    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime? LastMaintenanceTime { get; set; }
    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime? LastCheckedDate { get; set; }
    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime? StartDateOfUse { get; set; }
    public Guid? TypeId { get; set; }
    public Guid? ModelId { get; set; }
    public bool? IsRented { get; set; }
    public string? ImageUrl { get; set; }
    public AssetTypeDto? Type { get; set; }
    public ModelDto? Model { get; set; }
    public CategoryDto? Category { get; set; }
    public RoomBaseDto? Room { get; set; }
}

public class AssetBaseDto : BaseDto
{
    public string AssetName { get; set; } = null!;
    public string? AssetCode { get; set; }
    public bool? IsMovable { get; set; }
    public AssetStatus? Status { get; set; }
    public EnumValue? StatusObj { get; set; }
    public RequestType? RequestStatus { get; set; }
    public EnumValue? RequestStatusObj { get; set; }
    public int? ManufacturingYear { get; set; }
    public string? SerialNumber { get; set; }
    public double? Quantity { get; set; }
    public string? Description { get; set; }
    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime? LastMaintenanceTime { get; set; }
    public Guid? TypeId { get; set; }
    public Guid? ModelId { get; set; }
    public string? ImageUrl { get; set; }
    public bool? IsRented { get; set; }
    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime? LastCheckedDate { get; set; }
    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime? StartDateOfUse { get; set; }
}

public class AssetDetailDto : BaseDto
{
    public string AssetName { get; set; } = null!;
    public string? AssetCode { get; set; }
    public bool? IsMovable { get; set; }
    public AssetStatus? Status { get; set; }
    public EnumValue? StatusObj { get; set; }
    public int? ManufacturingYear { get; set; }
    public string? SerialNumber { get; set; }
    public double Quantity { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public bool? IsRented { get; set; }
    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime? LastMaintenanceTime { get; set; }
    public Guid? TypeId { get; set; }
    public Guid? ModelId { get; set; }
    public AssetTypeDto? Type { get; set; }
    public ModelDto? Model { get; set; }
    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime? LastCheckedDate { get; set; }
    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime? StartDateOfUse { get; set; }
    public CategoryDto? Category { get; set; }
}

public class AssetCreateDto
{
    //[Required(ErrorMessage = "Tên trang bị không được trống")]
    public string? AssetName { get; set; } = null!;
    public string? AssetCode { get; set; }
    public bool IsMovable { get; set; }
    public int? ManufacturingYear { get; set; }
    public string? SerialNumber { get; set; }
    public double? Quantity { get; set; }
    public string? Description { get; set; }
    public Guid? TypeId { get; set; }
    public Guid? ModelId { get; set; }
    public string? ImageUrl { get; set; }
    public bool? IsRented { get; set; }
    // public AssetTypeDto? Type { get; set; }
    // public ModelDto? Model { get; set; }
}

public class AssetUpdateDto
{
    public string? AssetName { get; set; }
    public string? AssetCode { get; set; }
    public bool? IsMovable { get; set; }
    public AssetStatus? Status { get; set; }
    public RequestType? RequestStatus { get; set; }
    public int? ManufacturingYear { get; set; }
    public string? SerialNumber { get; set; }
    public double? Quantity { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public bool? IsRented { get; set; }
    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime? LastMaintenanceTime { get; set; }
    public Guid? TypeId { get; set; }
    public Guid? ModelId { get; set; }
}

public class AssetQueryDto : BaseQueryDto
{
    public RequestType? RequestStatus { get; set; }
    public AssetStatus? Status { get; set; }
    public bool? IsMovable { get; set; }
    public Guid? TypeId { get; set; }
    public Guid? ModelId { get; set; }
    public Guid? CategoryId { get; set; }
    public bool? IsRented { get; set; }
    public Guid? RoomId  { get; set; }
}

public class AssetTaskCheckQueryDto : BaseQueryDto
{
    public string? RequestCode { get; set; }
    public DateTime? RequestDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public RequestStatus? Status { get; set; }
    public bool? IsInternal { get; set; }
    public Guid? AssignedTo { get; set; }
}

public class AssetCheckTrackingDto : BaseRequestDto
{
    public UserDto? User { get; set; }
}

public class AssetMaintenanceTrackingDto : BaseRequestDto
{
    public UserDto? User { get; set; }
}

public class AssetRepairationTrackingDto : BaseRequestDto
{
    public UserDto? User { get; set; }
}

public class AssetTransportationTrackingDto : BaseRequestDto
{
    public RoomBaseDto? ToRoom { get; set; }
    public RoomBaseDto? FromRoom { get; set; }
    public UserDto? User { get; set; }
}

public class AssetReplacementTrackingDto : BaseRequestDto
{
    public UserDto? AssignTo { get; set; }
    public AssetBaseDto? ReplacedBy { get; set; }
    public AssetTypeDto? AssetType { get; set; }
    public CategoryDto? Category { get; set; }
}