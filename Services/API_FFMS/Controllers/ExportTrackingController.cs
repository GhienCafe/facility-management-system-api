using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers;
public class ExportTrackingController : BaseController
{
    private readonly IExportService _service;

    public ExportTrackingController(IExportService service)
    {
        _service = service;
    }

    [HttpGet]
    [SwaggerOperation("Get export tracking asset in room")]
    public async Task<ApiResponse<Stream>> ExportTracking([FromQuery]ExportQueryTrackingDto queryDto)
    {
        return await _service.ExportCombinedData(queryDto);
    }
}