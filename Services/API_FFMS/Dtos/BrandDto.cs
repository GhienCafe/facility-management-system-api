using AppCore.Models;

namespace API_FFMS.Dtos;

public class BrandDto : BaseDto
{
    public string? BrandName { get; set; }
    public string? Description { get; set; }
}

public class BrandCreateDto
{
    public string? BrandName { get; set; }
    public string? Description { get; set; }
}

public class BrandUpdateDto
{
    public string? BrandName { get; set; }
    public string? Description { get; set; }
}

public class BrandQueryDto : BaseQueryDto
{
    public string? BrandName { get; set; }
    public string? Description { get; set; }
}