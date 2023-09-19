using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers;

public class FloorController : BaseController
{
    private readonly IFloorService _service;

    public FloorController(IFloorService service) 
    {
        _service = service;
    }
    
    [HttpGet]
    [SwaggerOperation("Get list floor")]
    public async Task<ApiResponses<FloorDto>> GetListFloor([FromQuery]FloorQueryDto queryDto)
    {
        return await _service.GetFloor(queryDto);
    }
    [HttpPost]
    [SwaggerOperation("Create new floor")]
    public async Task<ApiResponse> Insert([FromForm]FloorCreateDto floorDto)
    {
        return await _service.Insert(floorDto);
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation("Get detail floor information")]
    public async Task<ApiResponse<FloorDetailDto>> Get(Guid id)
    {
        return await _service.GetFloor(id);
    }

    [HttpPut("{id:guid}")]
        
    [SwaggerOperation("Update floor information")]
    public async Task<ApiResponse> Update(Guid id, [FromForm]FloorUpdateDto updateDto)
    {
        return await _service.Update(id, updateDto);
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation("Delete floor")]
    public async Task<ApiResponse> Delete(Guid id)
    {
        return await _service.Delete(id);
    }
}