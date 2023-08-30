using AppCore.Models;

namespace API_FFMS.Dtos;

public class CampusDto : BaseDto
{
    public string? CampusName { get; set; }
    public string? Telephone { get; set; }
    public string? Description { get; set; }
    public string? Address { get; set; }
}

public class CampusDetailDto : BaseDto
{
    public string? CampusName { get; set; }
    public string? Telephone { get; set; }
    public string? Description { get; set; }
    public string? Address { get; set; }
}

public class CampusQueryDto : BaseQueryDto
{
    public string? CampusName { get; set; }
}