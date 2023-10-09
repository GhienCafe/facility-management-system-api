using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers;

public class RoomStatusController : BaseController
{
    private readonly IRoomStatusService _service;

    public RoomStatusController(IRoomStatusService roomStatusService) 
    {
        _service = roomStatusService;
    }
    
    [HttpGet]
    [SwaggerOperation("Get list roomStatus")]
    public async Task<ApiResponses<RoomStatusDto>> GetListRoomStatus([FromQuery]RoomStatusQueryDto queryDto)
    {
        return await _service.GetRoomStatus(queryDto);
    }
    [HttpPost]
    [SwaggerOperation("Create new roomStatus")]
    public async Task<ApiResponse> InsertEvent([FromBody]RoomStatusCreateDto roomStatusDto)
    {
        return await _service.Insert(roomStatusDto);
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation("Get detail roomStatus information")]
    public async Task<ApiResponse<RoomStatusDetailDto>> Get(Guid id)
    {
        return await _service.GetRoomStatus(id);
    }
    [HttpDelete("{id:guid}")]
    [SwaggerOperation("Delete roomStatus")]
    public async Task<ApiResponse> Delete(Guid id)
    {
        return await _service.Delete(id);
    }
}