using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers;

public class MaintenanceConfigScheduleController : BaseController
{
    private readonly IMaintenanceScheduleService _maintenanceSchedule;

    public MaintenanceConfigScheduleController(IMaintenanceScheduleService maintenanceSchedule)
    {
        _maintenanceSchedule = maintenanceSchedule;
    }
    
    [HttpGet]
    [SwaggerOperation("Get list maintenances")]
    public async Task<ApiResponses<MaintenanceDto>> GetMaintenances([FromQuery]MaintenanceQueryDto queryDto)
    {
        return await _maintenanceService.GetItems(queryDto);
    }
}