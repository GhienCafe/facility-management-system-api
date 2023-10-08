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
    
    [HttpGet("{id}")]
    [SwaggerOperation("Get team member")]
    public async Task<ApiResponse<TeamMemberDetailDto>> GetListTeamMember(Guid id)
    {
        return await _teamMemberService.GetTeamMember(id);
    }
    
    [HttpPost]
    [SwaggerOperation("Create new team member")]
    public async Task<ApiResponse> GetListTeamMember([FromBody]TeamMemberCreateDto createDto)
    {
        return await _teamMemberService.CreateTeamMember(createDto);
    }
    
    [HttpPut("{id}")]
    [SwaggerOperation("Update team member")]
    public async Task<ApiResponse> GetListTeamMember(Guid id, [FromBody]TeamMemberUpdateDto updateDto)
    {
        return await _teamMemberService.UpdateTeamMember(id, updateDto);
    }
    
    [HttpDelete("{id}")]
    [SwaggerOperation("Delete team member")]
    public async Task<ApiResponse> DeleteTeamMember(Guid id)
    {
        return await _teamMemberService.DeleteTeamMember(id);
    }
    
}