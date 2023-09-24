using AppCore.Models;

namespace API_FFMS.Dtos;

public class TeamDto : BaseDto
{
    public string TeamName { get; set; } = null!;
    public string? Description { get; set; }
    public int? TotalMember { get; set; }
    //public IEnumerable<Cate>
}