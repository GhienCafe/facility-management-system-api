using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers
{
    public class RepairController : BaseController
    {
        private readonly IRepairService _service;

        public RepairController(IRepairService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation("Get list repairations")]
        public async Task<ApiResponses<RepairDto>> GetRepairs([FromQuery] RepairQueryDto queryDto)
        {
            return await _service.GetRepairs(queryDto);
        }

        [HttpPost]
        [SwaggerOperation("Create new repair")]
        public async Task<ApiResponse> Create([FromBody] RepairCreateDto createDto)
        {
            return await _service.CreateRepair(createDto);
        }

        [HttpPost("multi")]
        [SwaggerOperation("Create new list repair")]
        public async Task<ApiResponse> CreateMulti([FromBody] List<RepairCreateDto> createDtos)
        {
            return await _service.CreateMulti(createDtos);
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation("Get a repair")]
        public async Task<ApiResponse<RepairDto>> GetRepair(Guid id)
        {
            return await _service.GetRepair(id);
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation("Delete a repair")]
        public async Task<ApiResponse> Delete(Guid id)
        {
            return await _service.Delete(id);
        }

        [HttpPut("{id:guid}")]
        [SwaggerOperation("Update a repair")]
        public async Task<ApiResponse> Update(Guid id, BaseRequestUpdateDto updateDto)
        {
            return await _service.Update(id, updateDto);
        }

        [HttpDelete]
        [SwaggerOperation("Delete list repair requests")]
        public async Task<ApiResponse> DeleteMulti(DeleteMutilDto deleteDto)
        {
            return await _service.DeleteMulti(deleteDto);
        }

        [HttpPut("status-update/{id:guid}")]
        [SwaggerOperation("Confirm or reject repair request")]
        public async Task<ApiResponse> ConfirmOrReject(Guid id, BaseUpdateStatusDto confirmOrRejectDto)
        {
            return await _service.ConfirmOrReject(id, confirmOrRejectDto);
        }
    }
}
