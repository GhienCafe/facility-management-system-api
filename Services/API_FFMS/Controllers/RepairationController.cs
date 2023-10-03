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

        // [HttpGet]
        // [SwaggerOperation("Get all repairations")]
        // public async Task<ApiResponses<RepairationDto>> GetRepairations([FromQuery] RepairationQueryDto queryDto)
        // {
        //     return await _service.GetRepairations(queryDto);
        // }
        //
        // [HttpGet("{id:guid}")]
        // [SwaggerOperation("Get detail repairation")]
        // public async Task<ApiResponse<RepairationDetailDto>> GetRepairation(Guid id)
        // {
        //     return await _service.GetRepairation(id);
        // }
        //
        // [HttpPost]
        // [SwaggerOperation("Create new repairation")]
        // public async Task<ApiResponse> Create([FromBody] RepairationCreateDto createDto)
        // {
        //     return await _service.CreateRepairation(createDto);
        // }
        //
        // [HttpPut]
        // [SwaggerOperation("Update repairation")]
        // public async Task<ApiResponse> Update(Guid id, RepairationUpdateDto updateDto)
        // {
        //     return await _service.Update(id, updateDto);
        // }
        //
        // [HttpDelete("{id}")]
        // [SwaggerOperation("Delete repairation")]
        // public async Task<ApiResponse> Delete(Guid id)
        // {
        //     return await _service.Delete(id);
        // }
    }
}
