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
    
    [SwaggerOperation("Get asset status information")]
    [HttpGet("analysis-asset-status")]
    public async Task<ApiResponse<IEnumerable<AssetDashBoardInformation>>> GetAssetStatus()
    {
        return await _dashboardService.GetAssetStatusInformation();
    }

    [SwaggerOperation("Get base task information")]
    [HttpGet("analysis-task-infor")]
    public async Task<ApiResponse<IEnumerable<TaskDashBoardInformation>>> GetTaskInformation()
    {
        return await _dashboardService.GetBaseTaskInformation();
    }
    
    [SwaggerOperation("Get base task information base on status")]
    [HttpGet("analysis-task-status")]
    public async Task<ApiResponse<IEnumerable<TaskBasedOnStatusDashboardDto>>> GetTaskInformationBasedOnStatus()
    {
        return await _dashboardService.GetBaseTaskStatusInformation();
    }
}