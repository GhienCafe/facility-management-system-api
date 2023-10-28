﻿using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos;

public class TaskBaseDto : BaseRequestDto
{
    public RequestType Type { get; set; }
    public EnumValue? TypeObj { get; set; }
    
    // Transport
    public int? Quantity { get; set; }
    public Guid? ToRoomId { get; set; } // For internal
    
    // Replace
    //public string? NewAssetId { get; set; }
}

public class TaskDetailDto : BaseRequestDto
{
    public RequestType Type { get; set; }
    public EnumValue? TypeObj { get; set; }
    //Maintenance, repair
    //public AssetBaseDto? Asset { get; set; }


    //Replace
    public Guid? NewAssetId { get; set; }
    public AssetBaseDto? Asset { get; set; } //New asset
    public AssetBaseDto? NewAsset { get; set; }
    //Transport
    public Guid? ToRoomId { get; set; }
    //public int? Quantity { get; set; }
    public RoomBaseDto? ToRoom { get; set; } // ToRoom, //ToNewRoom
    public List<FromRoomAssetDto>? Assets { get; set; }

    //Asset check
    //public AssetBaseDto? Asset { get; set; }
    public RoomBaseDto? CurrentRoom { get; set; }
}

public class TaskQueryDto : BaseRequestQueryDto
{
    public RequestType? Type { get; set; }
}

public class ReportCreateDto
{
    public string? FileName { get; set; }
    public string? Key { get; set; }
    public string? RawUri { get; set; }
    public List<string>? Uris { get; set; }
    public string? Extensions { get; set; }
    public FileType FileType { get; set; }
    public string? Content { get; set; }
    public Guid? ItemId { get; set; }
    public RequestStatus? Status { get; set; }
    public bool? IsVerified { get; set; }
}

public class UpdateTaskStatus
{
    public RequestStatus? Status { get; set; }
    public bool? IsVerified { get; set; }
}