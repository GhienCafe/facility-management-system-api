using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers;

public class RoomController : BaseController
{
    private readonly IRoomService _service;

    public RoomController(IRoomService service) 
    {
        _service = service;
    }
    
    [HttpGet]
    [SwaggerOperation("Get list room")]
    public async Task<ApiResponses<RoomDto>> GetListFloor([FromQuery]RoomQueryDto queryDto)
    {
        return await _service.GetRoom(queryDto);
    }
    [HttpPost]
    [SwaggerOperation("Create new room")]
    public async Task<ApiResponse> Insert([FromBody]RoomCreateDto insertDto)
    {
        return await _service.Insert(insertDto);
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation("Get detail room information")]
    public async Task<ApiResponse<RoomDetailDto>> Get(Guid id)
    {
        return await _service.GetRoom(id);
    }

    [HttpPut("{id:guid}")]
        
    [SwaggerOperation("Update room information")]
    public async Task<ApiResponse<RoomDetailDto>> Update(Guid id, RoomUpdateDto updateDto)
    {
        return await _service.Update(id, updateDto);
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation("Delete room")]
    public async Task<ApiResponse> Delete(Guid id)
    {
        return await _service.Delete(id);
    }
}