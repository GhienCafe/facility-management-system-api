using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace API_FFMS.Controllers
{
    [ApiController]
    [Route("api/v1/export")]
    public class ExportTrackingController : BaseController
    {
        private readonly IExportService _service;

        public ExportTrackingController(IExportService service)
        {
            _service = service;
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
    }
}