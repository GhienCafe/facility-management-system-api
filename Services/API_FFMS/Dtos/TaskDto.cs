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

public class TaskQueryDto : BaseRequestQueryDto
{
    public RequestType Type { get; set; }
}