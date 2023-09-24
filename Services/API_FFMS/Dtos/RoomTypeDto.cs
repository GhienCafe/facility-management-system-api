using AppCore.Models;

namespace API_FFMS.Dtos;

public class RoomTypeDto : BaseDto
{
    public string TypeName { get; set; } = null!;
    public string? Description { get; set; }
}

public class RoomTypeQueryDto : BaseQueryDto
{
    public string? TypeName { get; set; }
}