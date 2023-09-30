using AppCore.Models;

namespace API_FFMS.Dtos;

public class ModelDto : BaseDto
{
    public string? ModelName { get; set; }
    public string? Description { get; set; }
}