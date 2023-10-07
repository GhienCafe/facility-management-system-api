using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers;

public class VirtualizeController : BaseController
{
    private readonly IVirtualizeService _service;

    public VirtualizeController(IVirtualizeService service) 
    {
        _service = service;
    }
    
    [HttpGet("{id}")]
    [SwaggerOperation("Get virtualization of floor")]
    public async Task<ApiResponse<VirtualizeFloorDto>> GetVirtualizeFloor(Guid id)
    {
        return await _service.GetVirtualizeFloor(id);
    }
    
    [HttpGet("rooms")]
    [SwaggerOperation("Get rooms in virtualization of floor")]
    public async Task<ApiResponse<IEnumerable<VirtualizeRoomDto>>> GetVirtualizeFloor([FromQuery]VirtualizeRoomQueryDto queryDto)
    {
        return await _service.GetVirtualizeRoom(queryDto);
    }
    
}