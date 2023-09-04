using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos;

public class FloorDto : BaseDto
{
    public double? Area { get; set; }
    public string? PathFloor { get; set; }
    public string? FloorNumber { get; set; }
    public Guid BuildingId { get; set; }
}

public class FloorDetailDto : BaseDto
{
    public double? Area { get; set; }
    public string? PathFloor { get; set; }
    public string? FloorNumber { get; set; }
    public Guid BuildingId { get; set; }
    public virtual Buildings? Building { get; set; }
    public List<Rooms> RoomList { get; }
}

public class FloorCreateDto
{
    public double? Area { get; set; }
    public string? PathFloor { get; set; }
    public string? FloorNumber { get; set; }
    public Guid BuildingId { get; set; }
}

public class FloorUpdateDto
{
    public double? Area { get; set; }
    public string? PathFloor { get; set; }
    public string? FloorNumber { get; set; }
    public Guid BuildingId { get; set; }
}
public class FloorQueryDto : BaseQueryDto
{
    public string? FloorNumber { get; }
    public string? BuildingName { get; }
}