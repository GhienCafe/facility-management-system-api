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
}