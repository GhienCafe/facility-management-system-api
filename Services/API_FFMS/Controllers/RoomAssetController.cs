using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomAssetController : ControllerBase
    {
        private readonly IRoomAssetService _service;

        public RoomAssetController(IRoomAssetService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation("Tracking asset use in room")]
        public async Task<ApiResponses<AssetTrackingDto>> AssetUsedTracking([FromQuery] RoomTrackingQueryDto queryDto)
        {
            return await _service.AssetUsedTracking(queryDto);
        }
    }
}
