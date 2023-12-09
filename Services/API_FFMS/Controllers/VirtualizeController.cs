using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers;

public class VirtualizeController : BaseController
{
    private readonly IVirtualizeService _service;
    private readonly ICacheService _cacheService;

    public VirtualizeController(IVirtualizeService service, ICacheService cacheService)
    {
        _service = service;
        _cacheService = cacheService;
    }

    [HttpGet("{id}")]
    [SwaggerOperation("Get virtualization of floor")]
    public async Task<ApiResponse<VirtualizeFloorDto>> GetVirtualizeFloor(Guid id)
    {
        return await _service.GetVirtualizeFloor(id);
    }

    [HttpGet("rooms")]
    [SwaggerOperation("Get rooms in virtualization of floor")]
    public async Task<ApiResponse<IEnumerable<VirtualizeRoomDto>>> GetVirtualizeFloor([FromQuery] VirtualizeRoomQueryDto queryDto)
    {
        var key = "rooms_virtualization" + queryDto.FloorId;

        // check cache data
        var cacheData = _cacheService.GetData<IEnumerable<VirtualizeRoomDto>>(key);
        if (cacheData != null)
        {
            return ApiResponse<IEnumerable<VirtualizeRoomDto>>.Success(cacheData);
        }

        var response = await _service.GetVirtualizeRoom(queryDto);
        // Leave it null - the default will be 5 minutes
        var expiryTime = DateTimeOffset.Now.AddMinutes(60);
        _cacheService.SetData(key, response.Data, expiryTime);

        return response;
    }

    [HttpGet("virtualize-dashboard")]
    [SwaggerOperation("Get dashboard information")]
    public async Task<ApiResponse<VirtualDashboard>> GetVirtualizeDashboard()
    {
        return await _service.GetVirtualDashBoard();
    }

}