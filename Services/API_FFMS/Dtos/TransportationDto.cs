using System.ComponentModel.DataAnnotations;

namespace API_FFMS.Dtos;

public class TransportDto : BaseRequestDto
{
    public int? Quantity { get; set; }
    public Guid? FromRoomId { get; set; }
    public Guid? ToRoomId { get; set; } // For internal
    public AssetBaseDto? Asset { get; set; }
    public RoomBaseDto? ToRoom { get; set; }
    public RoomBaseDto? FromRoom { get; set; } 
}

public class TransportCreateDto : BaseRequestCreateDto
{
    public int? Quantity { get; set; }
    public Guid? ToRoomId { get; set; }
}

public class TransportationQueryDto : BaseRequestQueryDto
{
    public string? RequestCode { get; set; }
    public DateTime? RequestDate { get; set; }
    public DateTime? CompletionDate { get; set; }
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