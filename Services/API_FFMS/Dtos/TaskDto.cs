using System.ComponentModel.DataAnnotations;
using AppCore.Models;
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
    //Maintenance, repair

    //Replace
    public Guid NewAssetId { get; set; }
    public AssetBaseDto? Asset { get; set; } //New asset
    public AssetBaseDto? NewAsset { get; set; }
    //Transport
    public Guid? ToRoomId { get; set; }
    public int? Quantity { get; set; }
    public RoomBaseDto? ToRoom { get; set; }// ToRoom
    public List<FromRoomAssetDto>? Assets { get; set; }

    //Asset check
    //public AssetBaseDto? Asset { get; set; }
    //public RoomBaseDto? Location { get; set; }
}

public class TaskQueryDto : BaseRequestQueryDto
{
    public RequestType Type { get; set; }
}