using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos;

public class FloorsDto
{
    public double? Area { get; set; }
    public string? PathFloor { get; set; }
    public string? FloorNumber { get; set; }
    public Guid BuildingId { get; set; }
    public List<Rooms> RoomsList { get; }
}

public class FloorDetailDto : BaseDto
{
    public double? Area { get; set; }
    public string? PathFloor { get; set; }
    public string? FloorNumber { get; set; }
    public Guid BuildingId { get; set; }
    public virtual Buildings? Buildings { get; set; }
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
public class FloorsQueryDto : BaseQueryDto
{
    public string? FloorNumber { get; }
    public Guid? BuildingId { get; }
}