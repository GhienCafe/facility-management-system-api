using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers;

public class CampusController : BaseController
{
    private readonly ICampusService _service;

    public CampusController(ICampusService campusService) 
    {
        _service = campusService;
    }
    
    [HttpGet]
    [SwaggerOperation("Get list campus")]
    public async Task<ApiResponses<CampusDto>> GetListCampus([FromQuery]CampusQueryDto queryDto)
    {
        return await _service.GetCampus(queryDto);
    }
    [HttpPost]
    [SwaggerOperation("Create new campus")]
    public async Task<ApiResponse> InsertEvent([FromBody]CampusCreateDto campusDto)
    {
        return await _service.Insert(campusDto);
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation("Get detail campus information")]
    public async Task<ApiResponse<CampusDetailDto>> Get(Guid id)
    {
        return await _service.GetCampus(id);
    }

    [HttpPut("{id:guid}")]
        
    [SwaggerOperation("Update campus information")]
    public async Task<ApiResponse<CampusDetailDto>> Update(Guid id, CampusUpdateDto campusDto)
    {
        return await _service.Update(id, campusDto);
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation("Delete campus")]
    public async Task<ApiResponse> Delete(Guid id)
    {
        return await _service.Delete(id);
    }
}