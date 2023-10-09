
using AppCore.Models;
using MainData.Entities;
using System.ComponentModel.DataAnnotations;

namespace API_FFMS.Dtos;

public class TransportDto : BaseDto
{
    public int? Quantity { get; set; }
    public Guid? AssetId { get; set; }
    public Guid? RequestId { get; set; }
    public Guid? ToRoomId { get; set; } // For internal
    public AssetBaseDto? Asset { get; set; }
    public RoomBaseDto? ToRoom { get; set; }
    public RoomBaseDto? FromRoom { get; set; } 
}

public class FutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value is DateTime date)
        {
            return date > DateTime.Now;
        }
        return false;
    }
}