using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers
{
    public class RepairationController : BaseController
    {
        private readonly IRepairationService _service;

        public RepairationController(IRepairationService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation("Get list repairations")]
        public async Task<ApiResponses<RepairationDto>> GetRepairations([FromQuery] RepairationQueryDto queryDto)
        {
            return await _service.GetRepairations(queryDto);
        }

        [HttpPost]
        [SwaggerOperation("Create new repairation")]
        public async Task<ApiResponse> Create([FromBody] RepairationCreateDto createDto)
        {
            return await _service.CreateRepairation(createDto);
        }

        [HttpPost("multi")]
        [SwaggerOperation("Create new list repairations")]
        public async Task<ApiResponse> CreateRepairations([FromBody] List<RepairationCreateDto> createDtos)
        {
            return await _service.CreateRepairations(createDtos);
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation("Get a repairation")]
        public async Task<ApiResponse<RepairationDto>> GetRepairation(Guid id)
        {
            return await _service.GetRepairation(id);
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation("Delete a repairation")]
        public async Task<ApiResponse> Delete(Guid id)
        {
            return await _service.Delete(id);
        }

        [HttpPut]
        [SwaggerOperation("Update a repairation")]
        public async Task<ApiResponse> Update(Guid id, BaseRequestUpdateDto updateDto)
        {
            return await _service.Update(id, updateDto);
        }

        [HttpDelete]
        [SwaggerOperation("Delete a repairation")]
        public async Task<ApiResponse> DeleteReplairations(List<Guid> ids)
        {
            return await _service.DeleteReplairations(ids);
        }

        [HttpPut("status-update/{id:guid}")]
        [SwaggerOperation("Update repairation's status")]
        public async Task<ApiResponse> UpdateStatus(Guid id, BaseUpdateStatusDto updateDto)
        {
            return await _service.UpdateStatus(id, updateDto);
        }
    }
}
