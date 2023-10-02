using System.Text.Json.Serialization;
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
    public string? FloorName { get; set; }
    public int FloorNumber { get; set; }
    public string? Description { get; set; }
    public double? TotalArea { get; set; }
    public Guid BuildingId { get; set; }
    public BuildingBaseDto? Building { get; set; }
}

public class FloorBaseDto : BaseDto
{
    public string? FloorName { get; set; }
    public int? FloorNumber { get; set; }
}

public class FloorDetailDto : BaseDto
{
    public string? FloorName { get; set; }
    public string? FloorMap { get; set; }
    public int FloorNumber { get; set; }
    public string? Description { get; set; }
    public double? TotalArea { get; set; }
    public Guid BuildingId { get; set; }
    public BuildingBaseDto? Building { get; set; }
}

public class FloorCreateFormDto
{
    [JsonPropertyName("floor_name")]
    public string? FloorName { get; set; }
    [JsonPropertyName("svg_file")]
    public IFormFile? SvgFile { get; set; }
    [JsonPropertyName("floor_number")]
    public int FloorNumber { get; set; }
    [JsonPropertyName("building_id")]
    public Guid BuildingId { get; set; }
    public string? Description { get; set; }
    public double? TotalArea { get; set; }
}

public class FloorCreateDto
{
    public string? FloorName { get; set; }
    public string? FloorMap { get; set; }
    public int FloorNumber { get; set; }
    public Guid BuildingId { get; set; }
    public string? Description { get; set; }
    public double? TotalArea { get; set; }
}

public class FloorUpdateDto
{
    public string? FloorMap { get; set; }
    public string? FloorName { get; set; }
    public int? FloorNumber { get; set; }
    public string? Description { get; set; }
    public double? TotalArea { get; set; }
    public Guid? BuildingId { get; set; }
}

public class FloorQueryDto : BaseQueryDto
{
    public int? FloorNumber { get; set; }
    public Guid? BuildingId { get; set; }
}