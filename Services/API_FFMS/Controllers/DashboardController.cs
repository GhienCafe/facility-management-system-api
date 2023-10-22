using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers;

public class DashboardController : BaseController
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }
    
    [SwaggerOperation("Get base information")]
    [HttpGet]
    public async Task<ApiResponse<DashboardDto>> GetInformation()
    {
        return await _dashboardService.GetInformation();
    }
}