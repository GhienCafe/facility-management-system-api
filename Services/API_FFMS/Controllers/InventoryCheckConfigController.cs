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
    [SwaggerOperation("Get config")]
    public async Task<ApiResponse<InventoryCheckConfigDto>> GetConfig()
    {
        return await _service.GetConfig();
    }

    [HttpPost]
    [SwaggerOperation("Create or update config")]
    public async Task<ApiResponse> CreateOrUpdateConfig(InventoryCheckConfigCreateDto dto)
    {
        return await _service.CreateOrUpdateConfig(dto);
    }
}
