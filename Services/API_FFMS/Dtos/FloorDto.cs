using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos;

public class BuildingBaseDto : BaseDto
{
    public string? BuildingName { get; set; }
    public string? BuildingCode { get; set; }
}

public class FloorDto : BaseDto
{
    public string? FloorMap { get; set; }
    public int? FloorNumber { get; set; }
    public Guid BuildingId { get; set; }
    public BuildingBaseDto? Building { get; set; }
}

public class FloorDetailDto : BaseDto
{
    public string? FloorMap { get; set; }
    public int FloorNumber { get; set; }
    public Guid BuildingId { get; set; }
    public BuildingBaseDto? Building { get; set; }
}

public class FloorCreateDto
{
    public IFormFile? SvgFile { get; set; }
    public int FloorNumber { get; set; }
    public Guid BuildingId { get; set; }
}

public class FloorUpdateDto
{
    public int? FloorNumber { get; set; }
    public Guid? BuildingId { get; set; }
    public IFormFile? SvgFile { get; set; }
}

public class FloorQueryDto : BaseQueryDto
{
    public int? FloorNumber { get; set; }
}