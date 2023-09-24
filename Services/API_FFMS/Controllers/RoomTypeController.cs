using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers
{
    public class RoomTypeController : BaseController
    {
        private readonly IRoomTypeService _service;

        public RoomTypeController(IRoomTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation("Get list room types")]
        public async Task<ApiResponses<RoomTypeDto>> GetRoomTypes([FromQuery] RoomTypeQueryDto queryDto)
        {
            return await _service.GetRoomTypes(queryDto);
        }

        [HttpPost]
        [SwaggerOperation("Create new room type")]
        public async Task<ApiResponse> Insert([FromBody] RoomTypeCreateDto insertDto)
        {
            return await _service.Insert(insertDto);
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation("Get detail room type")]
        public async Task<ApiResponse<RoomTypeDetailDto>> GetRoomType(Guid id)
        {
            return await _service.GetRoomType(id);
        }

        [HttpPut("{id:guid}")]

        [SwaggerOperation("Update room type")]
        public async Task<ApiResponse> Update(Guid id, RoomTypeUpdateDto updateDto)
        {
            return await _service.Update(id, updateDto);
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation("Delete room type")]
        public async Task<ApiResponse> Delete(Guid id)
        {
            return await _service.Delete(id);
        }
    }
}
