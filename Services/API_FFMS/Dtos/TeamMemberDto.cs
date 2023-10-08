using AppCore.Models;

namespace API_FFMS.Dtos;

public class TeamMemberDto : BaseDto
{
    public Guid MemberId { get; set; }
    public Guid TeamId { get; set; }
    public bool IsLead { get; set; }
    
    public TeamDto? TeamDto { get; set; }
    public UserBaseDto? Member { get; set; }
}

public class TeamMemberQueryDto : BaseQueryDto
{
    public Guid? MemberId { get; set; }
    public Guid? TeamId { get; set; }
    public bool? IsLead { get; set; }
}
