using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers;

public class MaintenanceScheduleController : BaseController
{
    private readonly IMaintenanceScheduleService _maintenanceScheduleService;

    public MaintenanceScheduleController(IMaintenanceScheduleService maintenanceSchedule)
    {
        _maintenanceScheduleService = maintenanceSchedule;
    }
    
    [HttpGet]
    [SwaggerOperation("Get list maintenance schedules")]
    public async Task<ApiResponses<MaintenanceScheduleConfigDto>> GetItems([FromQuery]MaintenanceScheduleConfigQueryDto queryDto)
    {
        return await _maintenanceScheduleService.GetItems(queryDto);
    }
    
    [HttpGet("assets-in-time")]
    [SwaggerOperation("Get list assets in time for maintenance")]
    public async Task<ApiResponses<AssetMaintenanceDto>> GetAssetsInTime([FromQuery]AssetQueryDto queryDto)
    {
        return await _maintenanceScheduleService.GetMaintenanceItems(queryDto);
    }
    
    [HttpGet("{id}")]
    [SwaggerOperation("Get maintenance schedule")]
    public async Task<ApiResponse<MaintenanceScheduleConfigDetailDto>> GetItem(Guid id)
    {
        return await _maintenanceScheduleService.GetItem(id);
    }

    [HttpPost]
    [SwaggerOperation("Create maintenance schedule")]
    public async Task<ApiResponse> CreateItem(MaintenanceScheduleConfigCreateDto createDto)
    {
        return await _maintenanceScheduleService.CreateMaintenanceSchedule(createDto);
    }
    
    [HttpPut("{id}")]
    [SwaggerOperation("Update maintenance schedule")]
    public async Task<ApiResponse> UpdateItem(Guid id, [FromBody]MaintenanceScheduleConfigUpdateDto updateDto)
    {
        return await _maintenanceScheduleService.UpdateItem(id, updateDto);
    }
    
    [HttpDelete("{id}")]
    [SwaggerOperation("Delete maintenance schedule")]
    public async Task<ApiResponse> DeleteItem(Guid id)
    {
        return await _maintenanceScheduleService.DeleteItem(id);
    }
    
    [HttpDelete]
    [SwaggerOperation("Delete maintenance schedules")]
    public async Task<ApiResponse> DeleteItem(List<Guid> ids)
    {
        return await _maintenanceScheduleService.DeleteItems(ids);
    }
}