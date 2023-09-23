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
        public async Task<ApiResponse<AssetDetailDto>> GetAsset(Guid id)
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
        public async Task<ApiResponses<RoomAssetDto>> GetAssetsInRoom(Guid id, [FromQuery]RoomAssetQueryDto queryDto)
        {
            return await _assetService.GetAssetsInRoom(id, queryDto);
        }
    }
}
