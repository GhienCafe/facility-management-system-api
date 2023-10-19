﻿using AppCore.Configs;
using AppCore.Models;
using MainData.Entities;
using Newtonsoft.Json;

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
    public int? Quantity { get; set; }
    public RoomBaseDto? FromRoom { get; set; }
    public AssetBaseDto? Asset { get; set; }
}

public class TransportCreateDto
{
    public List<AssetTransportDto>? Assets { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public bool IsInternal { get; set; }
    public int? Quantity { get; set; }
    public Guid? AssignedTo { get; set; }
    //public Guid? FromRoomId { get; set; }
    public Guid? ToRoomId { get; set; }
}

public class AssetTransportDto
{
    public Guid AssetId { get; set; }
    //public string? AssetName { get; set; }
    public double? Quantity { get; set; }
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