using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos;

public class FloorDto : BaseDto
{
    public string? FloorMap { get; set; }
    public int? FloorNumber { get; set; }
    public Guid BuildingId { get; set; }
}

public class FloorDetailDto : BaseDto
{
    public string? FloorMap { get; set; }
    public int FloorNumber { get; set; }
    public Guid BuildingId { get; set; }
}

public class FloorCreateDto
{
    public string? FloorMap { get; set; }
    public int FloorNumber { get; set; }
    public Guid BuildingId { get; set; }
}

public class FloorUpdateDto
{
    public string? FloorMap { get; set; }
    public int? FloorNumber { get; set; }
    public Guid? BuildingId { get; set; }
}
public class FloorQueryDto : BaseQueryDto
{
    public int? FloorNumber { get; set; }
}