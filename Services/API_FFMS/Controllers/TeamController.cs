using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using MainData.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers
{
    public class TeamController : BaseController
    {
        private readonly ITeamService _service;

        public TeamController(ITeamService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation("Get list teams")]
        public async Task<ApiResponses<TeamDto>> GetTeams([FromQuery] TeamQueryDto queryDto)
        {
            return await _service.GetTeams(queryDto);
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation("Get detail room type")]
        public async Task<ApiResponse<TeamDetailDto>> GetTeam(Guid id)
        {
            return await _service.GetTeam(id);
        }

        [HttpPost]
        [SwaggerOperation("Create new team")]
        public async Task<ApiResponse> Insert([FromBody] TeamCreateDto insertDto)
        {
            return await _service.Insert(insertDto);
        }

        [HttpPut("{id:guid}")]
        [SwaggerOperation("Update team")]
        public async Task<ApiResponse> Update(Guid id, TeamUpdateDto updateDto)
        {
            return await _service.Update(id, updateDto);
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation("Delete team")]
        public async Task<ApiResponse> Delete(Guid id)
        {
            return await _service.Delete(id);
        }

        [HttpDelete]
        [SwaggerOperation("Delete list team")]
        public async Task<ApiResponse> DeleteTeams(DeleteMutilDto deleteDto)
        {
            return await _service.DeleteTeams(deleteDto);
        }
    }
}

