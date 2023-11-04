using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers
{
    public class AssetController : BaseController
    {
        private readonly IAssetService _assetService;

        public AssetController(IAssetService assetService)
        {
            _assetService = assetService;
        }

        [HttpGet]
        [SwaggerOperation("Get list asset")]
        public async Task<ApiResponses<AssetDto>> GetAssets([FromQuery] AssetQueryDto queryDto)
        {
            return await _assetService.GetAssets(queryDto);
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation("Get details an asset")]
        public async Task<ApiResponse<AssetDto>> GetAsset(Guid id)
        {
            return await _assetService.GetAsset(id);
        }

        [HttpPost]
        [SwaggerOperation("Create new asset")]
        public async Task<ApiResponse> Create([FromBody] AssetCreateDto createDto)
        {
            return await _assetService.Create(createDto);
        }

        [HttpPut("{id:guid}")]
        [SwaggerOperation("Update an asset")]
        public async Task<ApiResponse> Update(Guid id, AssetUpdateDto updateDto)
        {
            return await _assetService.Update(id, updateDto);
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation("Delete asset")]
        public async Task<ApiResponse> Delete(Guid id)
        {
            return await _assetService.Delete(id);
        }

        [HttpGet("room/{id:guid}")]
        [SwaggerOperation("Get assets in room")]
        public async Task<ApiResponses<RoomAssetDto>> GetAssetsInRoom(Guid id, [FromQuery] RoomAssetQueryDto queryDto)
        {
            return await _assetService.GetAssetsInRoom(id, queryDto);
        }

        [HttpPost("ids")]
        [SwaggerOperation("Delete list assets")]
        public async Task<ApiResponse> DeleteAssets(DeleteMutilDto deleteDto)
        {
            return await _assetService.DeleteAssets(deleteDto);
        }

        [HttpGet("asset-check/{id}")]
        [SwaggerOperation("Get history checking of asset")]
        public async Task<ApiResponses<AssetCheckTrackingDto>> AssetCheckTracking(Guid id, [FromQuery] AssetTaskCheckQueryDto queryDto)
        {
            return await _assetService.AssetCheckTracking(id, queryDto);
        }

        [HttpGet("maintenance/{id}")]
        [SwaggerOperation("Get history maintenance of asset")]
        public async Task<ApiResponses<AssetMaintenanceTrackingDto>> AssetMaintenanceTracking(Guid id, [FromQuery] AssetTaskCheckQueryDto queryDto)
        {
            return await _assetService.AssetMaintenanceTracking(id, queryDto);
        }

        [HttpGet("repairation/{id}")]
        [SwaggerOperation("Get history repairation of asset")]
        public async Task<ApiResponses<AssetRepairationTrackingDto>> AssetRepairationTracking(Guid id, [FromQuery] AssetTaskCheckQueryDto queryDto)
        {
            return await _assetService.AssetRepairationTracking(id, queryDto);
        }

        [HttpGet("transportation/{id}")]
        [SwaggerOperation("Get history transportation of asset")]
        public async Task<ApiResponses<AssetTransportationTrackingDto>> AssetTransportationTracking(Guid id, [FromQuery] AssetTaskCheckQueryDto queryDto)
        {
            return await _assetService.AssetTransportationTracking(id, queryDto);
        }

        [HttpGet("replacement/{id}")]
        [SwaggerOperation("Get history replacement of asset")]
        public async Task<ApiResponses<AssetReplacementTrackingDto>> AssetReplacementTracking(Guid id, [FromQuery] AssetTaskCheckQueryDto queryDto)
        {
            return await _assetService.AssetReplacementTracking(id, queryDto);
        }
    }
}
