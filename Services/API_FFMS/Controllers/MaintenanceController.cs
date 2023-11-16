using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers;

public class MaintenanceController : BaseController
{
    private readonly IMaintenanceService _maintenanceService;

    public MaintenanceController(IMaintenanceService maintenanceService)
    {
        _maintenanceService = maintenanceService;
    }
    
    [HttpGet]
    [SwaggerOperation("Get list maintenances")]
    public async Task<ApiResponses<MaintenanceDto>> GetMaintenances([FromQuery]MaintenanceQueryDto queryDto)
    {
        return await _maintenanceService.GetItems(queryDto);
    }
    
    [HttpGet("{id:guid}")]
    [SwaggerOperation("Get detail maintenance")]
    public async Task<ApiResponse<MaintenanceDto>> GetMaintenance(Guid id)
    {
        return await _maintenanceService.GetItem(id);
    }
    
    [HttpPost]
    [SwaggerOperation("Create new maintenance")]
    public async Task<ApiResponse> Create([FromBody] MaintenanceCreateDto createDto)
    {
        return await _maintenanceService.CreateItem(createDto);
    }
    
    [HttpPut("{id}")]
    [SwaggerOperation("Update maintenance")]
    public async Task<ApiResponse> Update(Guid id, MaintenanceUpdateDto updateDto)
    {
        return await _maintenanceService.UpdateItem(id, updateDto);
    }

    [HttpPut("status-update/{id:guid}")]
    [SwaggerOperation("Update maintenance's status")]
    public async Task<ApiResponse> UpdateStatus(Guid id, BaseUpdateStatusDto updateDto)
    {
        return await _maintenanceService.UpdateStatus(id, updateDto);
    }

    // [HttpDelete("{id}")]
    // [SwaggerOperation("Delete maintenance")]
    // public async Task<ApiResponse> Delete(Guid id)
    // {
    //     return await _maintenanceService.DeleteMaintenance(id);
    // }

    [HttpPost("multi")]
    [SwaggerOperation("Create new list maintenance")]
    public async Task<ApiResponse> CreateItems([FromBody] List<MaintenanceCreateDto> createDtos)
    {
        return await _maintenanceService.CreateItems(createDtos);
    }
}