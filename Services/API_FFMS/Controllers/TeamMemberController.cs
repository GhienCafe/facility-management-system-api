using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers;

public class TeamMemberController : BaseController
{
    private readonly ITeamMemberService _teamMemberService;

    public TeamMemberController(ITeamMemberService teamMemberService)
    {
        _teamMemberService = teamMemberService;
    }
    
    [HttpGet]
    [SwaggerOperation("Get list team members")]
    public async Task<ApiResponses<TeamMemberDto>> GetListTeamMember([FromQuery]TeamMemberQueryDto queryDto)
    {
        return await _teamMemberService.GetTeamsMember(queryDto);
    }
}