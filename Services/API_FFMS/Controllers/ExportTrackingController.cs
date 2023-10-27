using API_FFMS.Services;
using Microsoft.AspNetCore.Mvc;

namespace API_FFMS.Controllers
{
    [ApiController]
    [Route("api/v1/export")]
    public class ExportTrackingController : BaseController
    {
        private readonly IExportService _service;
        private readonly ITaskExportService _taskExportService;

        public ExportTrackingController(IExportService service, ITaskExportService taskExportService)
        {
            _service = service;
            _taskExportService = taskExportService;
        }

        [HttpGet("room")]
        public async Task<IActionResult> Export([FromQuery] QueryRoomExportDto exportQuery)
        {
            var exportFile = await _service.Export(exportQuery);
            return File(
                exportFile.Stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"{exportFile.FileName}.xlsx"
            );
        }
        [HttpGet("asset")]
        public async Task<IActionResult> Export([FromQuery] QueryAssetExportDto exportQuery)
        {
            var exportFile = await _service.Export(exportQuery);
            return File(
                exportFile.Stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"{exportFile.FileName}.xlsx"
            );
        }

        [HttpGet]
        public async Task<IActionResult> ExportAsset()
        {
            var exportFile = await _taskExportService.ExportAsset();
            return File(
                exportFile.Stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"{exportFile.FileName}.xlsx"
            );
        }

        [HttpGet("export-ultra")]
        public async Task<IActionResult> ExportTaskUltra()
        {
            var exportFile = await _taskExportService.ExportTaskUltra();
            return File(
                exportFile.Stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"{exportFile.FileName}.xlsx"
            );
        }
    }
}