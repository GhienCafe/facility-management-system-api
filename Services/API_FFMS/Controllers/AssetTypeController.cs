using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers
{
    public class AssetTypeController : BaseController
    {
        private readonly IAssetTypeService _service;

        public AssetTypeController(IAssetTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation("Get list of asset types")]
        public async Task<ApiResponses<AssetTypeDto>> GetAssetTypes([FromQuery] AssetTypeQueryDto queryDto)
        {
            return await _service.GetAssetTypes(queryDto);
        }

        [HttpGet("asset-types")]
        [SwaggerOperation("Get list of asset types")]
        public async Task<ApiResponses<AssetTypeSheetDto>> GetAssetTypes()
        {
            return await _service.GetAssetTypes();
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation("Get asset type by ID")]
        public async Task<ApiResponse<AssetTypeDetailDto>> GetAssetType(Guid id)
        {
            return await _service.GetAssetType(id);
        }

        [HttpPost]
        [SwaggerOperation("Create a new asset type")]
        public async Task<ApiResponse> CreateAssetType([FromBody] AssetTypeCreateDto createDto)
        {
            return await _service.Create(createDto);
        }

        [HttpPut("{id:guid}")]
        [SwaggerOperation("Update type information")]
        public async Task<ApiResponse> UpdateAssetType(Guid id, AssetTypeUpdateDto updateDto)
        {
            return await _service.Update(id, updateDto);
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation("Delete asset type")]
        public async Task<ApiResponse> DeleteAssetType(Guid id)
        {
            return await _service.Delete(id);
        }

        [HttpDelete]
        [SwaggerOperation("Delete list asset type")]
        public async Task<ApiResponse> DeleteAssetTypes(List<Guid> ids)
        {
            return await _service.DeleteAssetTypes(ids);
        }
    }
}
