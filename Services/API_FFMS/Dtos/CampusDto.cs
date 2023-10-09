using AppCore.Attributes;
using AppCore.Models;

namespace API_FFMS.Dtos;

public class CampusDto : BaseDto
{
    public string? CampusName { get; set; }
    public string? CampusCode { get; set; }
    public string? Telephone { get; set; }
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? LogoUrl { get; set; }
    public string? WebsiteUrl { get; set; }
}

public class CampusDetailDto : BaseDto
{
    public string? CampusCode { get; set; }
    public string? CampusName { get; set; }
    public string? Telephone { get; set; }
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? LogoUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public int? TotalBuilding { get; set; }
}

public class CampusCreateDto
{
    [Required] public string CampusName { get; set; } = null!;
    [Required] public string CampusCode { get; set; } = null!;
    public string? Telephone { get; set; }
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? LogoUrl { get; set; }
    public string? WebsiteUrl { get; set; }
}
public class CampusUpdateDto
{
    public string? CampusName { get; set; }
    public string? CampusCode { get; set; }
    public string? Telephone { get; set; }
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? LogoUrl { get; set; }
    public string? WebsiteUrl { get; set; }
}
public class CampusQueryDto : BaseQueryDto
{
}