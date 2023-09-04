using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers;

public class BuildingController : BaseController
{
    private readonly IBuildingsService _service;

    public BuildingController(IBuildingsService service) 
    {
        _service = service;
    }
    
    [HttpGet]
    [SwaggerOperation("Get list building")]
    public async Task<ApiResponses<BuildingDto>> GetListCampus([FromQuery]BuildingQueryDto queryDto)
    {
        return await _service.GetBuildings(queryDto);
    }
    [HttpPost]
    [SwaggerOperation("Create new building")]
    public async Task<ApiResponse> InsertFloor([FromBody]BuildingCreateDto buildingDto)
    {
        return await _service.Insert(buildingDto);
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation("Get detail building information")]
    public async Task<ApiResponse<BuildingDetailDto>> Get(Guid id)
    {
        return await _service.GetBuildings(id);
    }

    [HttpPut("{id:guid}")]
        
    [SwaggerOperation("Update building information")]
    public async Task<ApiResponse<BuildingDetailDto>> Update(Guid id, BuildingUpdateDto buildingDto)
    {
        return await _service.Update(id, buildingDto);
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation("Delete building")]
    public async Task<ApiResponse> Delete(Guid id)
    {
        return await _service.Delete(id);
    }
}