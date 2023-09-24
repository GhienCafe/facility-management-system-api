using AppCore.Models;
using MainData.Entities;
using System.ComponentModel.DataAnnotations;

namespace API_FFMS.Dtos;

public class TeamDto : BaseDto
{
    public string TeamName { get; set; } = null!;
    public string? Description { get; set; }
    public int? TotalMember { get; set; }
    //public IEnumerable<Cate>
}
public class TeamDetailDto : BaseDto
{
    public string TeamName { get; set; } = null!;
    public string? Description { get; set; }
    public int? TotalMember { get; set; }
    public IEnumerable<TeamIncludeDto>? Members { get; set; }
}

public class TeamCreateDto
{
    [Required]
    public string? TeamName { get; set; }
    public string? Description { get; set; }
}

public class TeamUpdateDto
{
    public string? TeamName { get; set; }
    public string? Description { get; set; }
}

public class TeamIncludeDto
{
    public string UserCode { get; set; } = null!;
    public string Fullname { get; set; } = null!;
    public string? Avatar { get; set; }
    public UserStatus Status { get; set; }
    public string Email { get; set; } = null!;
    public string? PersonalIdentifyNumber { get; set; }
}

public class TeamQueryDto : BaseQueryDto
{
    public string? TeamName { get; set; }
    public string? Description { get; set; }
}