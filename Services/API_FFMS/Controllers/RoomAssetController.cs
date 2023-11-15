using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers
{
    public class RoomAssetController : BaseController
    {
        private readonly IRoomAssetService _service;

        public RoomAssetController(IRoomAssetService service)
        {
            _service = service;
        }

        // [HttpGet]
        // [SwaggerOperation("Tracking asset used in room")]
        // public async Task<ApiResponses<AssetTrackingDto>> AssetUsedTracking([FromQuery] RoomTrackingQueryDto queryDto, Guid id)
        // {
        //     return await _service.AssetUsedTracking(id, queryDto);
        // }

        [HttpGet]
        [SwaggerOperation("Get room assets")]
        public async Task<ApiResponses<RoomAssetBaseDto>> GetItems([FromQuery] RoomAssetQueryDto queryDto)
        {
            return await _service.GetItems(queryDto);
        }
        
        [HttpGet("{id}")]
        [SwaggerOperation("Get room asset")]
        public async Task<ApiResponse<RoomAssetDetailDto>> GetItem(Guid id)
        {
            return await _service.GetItem(id);
        }
        
        [HttpPost]
        [SwaggerOperation("Create new room asset")]
        public async Task<ApiResponse> CreateItem([FromBody]RoomAssetCreateBaseDto createBaseDto)
        {
            return await _service.CreateRoomAsset(createBaseDto);
        }

        [HttpPost("nulti")]
        [SwaggerOperation("Create new room assets")]
        public async Task<ApiResponse> CreateItems([FromBody] RoomAssetMultiCreateBaseDto createBaseDto)
        {
            return await _service.CreateRoomAssets(createBaseDto);
        }

        [HttpPut("{id}")]
        [SwaggerOperation("Update room asset")]
        public async Task<ApiResponse> UpdateItem(Guid id, [FromBody]RoomAssetUpdateBaseDto update)
        {
            return await _service.UpdateItem(id, update);
        }
        
        [HttpDelete("{id}")]
        [SwaggerOperation("Delete room asset")]
        public async Task<ApiResponse> DeleteItem(Guid id)
        {
            return await _service.DeleteItem(id);
        }
    }
}
