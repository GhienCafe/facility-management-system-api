using System.ComponentModel.DataAnnotations;
using AppCore.Models;

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

public class TaskDto : BaseDto
{
    public DateTime RequestedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string? Description { get; set; }
    public string? Note { get; set; }
    public ActionType ActionType { get; set; }
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
    
    // Maintenance 
    public EnumValue? MaintenanceType { get; set; }
}