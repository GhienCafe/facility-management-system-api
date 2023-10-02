using System.ComponentModel.DataAnnotations;
using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos;

public enum ActionType
{
    [Display(Name = "Bảo trì")]
    Maintenance = 1,
    [Display(Name = "Thay thế")]
    Replacement = 2,
    [Display(Name = "Sửa chửa")]
    Repairation = 3,
    [Display(Name = "Điều chuyển")]
    Transportation = 4,
}

public class TaskDataSetDto : BaseDto
{
    public DateTime RequestedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string? Description { get; set; }
    public string? Note { get; set; }
    public string? Reason { get; set; }
    public ActionType? Type { get; set; }
    public Guid? AssignedTo { get; set; }
    public Guid? AssetId { get; set; }
    public UserDto? PersonInCharge { get; set; }
    public AssetDto? Asset { get; set; } 
    
    // Replacement
    public Guid? NewAssetId { get; set; }
    public AssetDto? NewAsset { get; set; }
    
    // Transport
    public int? Quantity { get; set; }
    public Guid? ToRoomId { get; set; } 
    public RoomDto? ToRoom { get; set; }
}

public class TaskDto : BaseDto
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public DateTime? NotificationDate { get; set; }
    public DateTime RequestedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string? Description { get; set; }
    public string? Note { get; set; }
    public string? Reason { get; set; }
    public EnumValue? Status { get; set; }
    public EnumValue? Type { get; set; }
    public Guid? AssignedTo { get; set; }
    public Guid? AssetId { get; set; }
    //public UserDto? PersonInCharge { get; set; }
    //public AssetDto? Asset { get; set; } 
    
    // Replacement
   // public Guid? NewAssetId { get; set; }
    //public AssetDto? NewAsset { get; set; }
    
    // Transport
    //public int? Quantity { get; set; }
    public Guid? ToRoomId { get; set; } 
    public RoomDto? ToRoom { get; set; }
    public RoomDto? Location { get; set; }
}

public class DetailTaskDto : BaseDto
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public DateTime? NotificationDate { get; set; }
    public DateTime RequestedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string? Description { get; set; }
    public string? Note { get; set; }
    public string? Reason { get; set; }
    public EnumValue? Status { get; set; }
    public EnumValue? Type { get; set; }
    public Guid? AssignedTo { get; set; }
    public Guid? AssetId { get; set; }
    //public UserDto? PersonInCharge { get; set; }
    public AssetDto? Asset { get; set; } 
    
    // Replacement
    public Guid? NewAssetId { get; set; }
    public AssetDto? NewAsset { get; set; }
    
    // Transport
    public int? Quantity { get; set; }
    public Guid? ToRoomId { get; set; } 
    public RoomDto? ToRoom { get; set; }
    public RoomDto? Location { get; set; }
}

public class TaskQueryDto : BaseQueryDto
{
}

public class TaskCommonQueryDto : BaseDto
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public ActionType? Type { get; set; }
    public Guid? AssetId { get; set; }
    public Guid? RoomId { get; set; }
    public RoomDto? Room { get; set; }
    public DateTime? RequestedDate { get; set; }
}

public class TaskCommonDto : BaseDto
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public EnumValue? Type { get; set; }
    public EnumValue? Status { get; set; }
    public Guid? AssetId { get; set; }
    public Guid? RoomId { get; set; }
    public RoomDto? Room { get; set; }
    public DateTime? RequestedDate { get; set; }
}