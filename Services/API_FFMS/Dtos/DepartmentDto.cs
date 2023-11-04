using AppCore.Models;

namespace API_FFMS.Dtos;

public class DepartmentDto : BaseDto
{
    public Guid? CampusId { get; set; }
    public string? DepartmentCode { get; set; } = null!;
    public string? DepartmentName { get; set; } = null!;
}