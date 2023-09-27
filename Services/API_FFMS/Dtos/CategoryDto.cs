using AppCore.Models;

namespace API_FFMS.Dtos;

public class CategoryDto : BaseDto
{
    public string CategoryName { get; set; } = null!;
    public string? Description { get; set; }
    public Guid? TeamId { get; set; }
    public TeamDto? Team { get; set; }
}

public class CategoryQueryDto : BaseQueryDto
{
    public string? CategoryName { get; set; }
    public string? Description { get; set; }
    public Guid? TeamId { get; set; }
}

public class CategoryCreateDto
{
    public string CategoryName { get; set; } = null!;
    public string? Description { get; set; }
    public Guid? TeamId { get; set; }
}

public class CategoryDetailDto : BaseDto
{
    public string CategoryName { get; set; } = null!;
    public string? Description { get; set; }
    public Guid? TeamId { get; set; }
}

public class CategoryUpdateDto
{
    public string CategoryName { get; set; } = null!;
    public string? Description { get; set; }
    public Guid? TeamId { get; set; }
}