using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers;

public class InventoryCheckConfigController : BaseController
{
    private readonly IInventoryCheckConfigService _service;

    public InventoryCheckConfigController(IInventoryCheckConfigService service)
    {
        _service = service;
    }

    [HttpGet]
    [SwaggerOperation("Get list configs")]
    public async Task<ApiResponses<InventoryCheckConfigDto>> GetBrands([FromQuery] InventoryCheckConfigQueryDto queryDto)
    {
        return await _service.GetConfigs(queryDto);
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation("Get details an config")]
    public async Task<ApiResponse<InventoryCheckConfigDto>> GetBrand(Guid id)
    {
        return await _service.GetConfig(id);
    }

    [HttpPost]
    [SwaggerOperation("Create new config")]
    public async Task<ApiResponse> Create([FromBody] InventoryCheckConfigCreateDto createDto)
    {
        return await _service.Create(createDto);
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation("Update a config")]
    public async Task<ApiResponse> Update(Guid id, InventoryCheckConfigUpdateDto updateDto)
    {
        return await _service.Update(id, updateDto);
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation("Delete config")]
    public async Task<ApiResponse> Delete(Guid id)
    {
        return await _service.Delete(id);
    }

    [HttpDelete]
    [SwaggerOperation("Delete list config")]
    public async Task<ApiResponse> DeleteConfigs(DeleteMutilDto deleteDto)
    {
        return await _service.DeleteConfigs(deleteDto);
    }
}
