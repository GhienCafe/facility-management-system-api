using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetCategoryController : ControllerBase
    {
        private readonly IAssetCategoryService _service;

        public AssetCategoryController(IAssetCategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation("Get list of asset categories")]
        public async Task<ApiResponses<AssetCategoryDto>> GetAssetCategories([FromQuery] AssetCategoryQueryDto queryDto)
        {
            return await _service.GetAssetCategories(queryDto);
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation("Get asset category by ID")]
        public async Task<ApiResponse<AssetCategoryDetailDto>> GetAssetCategory(Guid id)
        {
            return await _service.GetAssetCategory(id);
        }

        [HttpPost]
        [SwaggerOperation("Create a new asset category")]
        public async Task<ApiResponse> CreateAssetCategory([FromBody] AssetCategoryCreateDto createDto)
        {
            return await _service.Create(createDto);
        }

        [HttpPut("{id:guid}")]
        [SwaggerOperation("Update asset category")]
        public async Task<ApiResponse<AssetCategoryDetailDto>> UpdateAssetCategory(Guid id, AssetCategoryUpdateDto updateDto)
        {
            return await _service.Update(id, updateDto);
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation("Delete asset category")]
        public async Task<ApiResponse> DeleteAssetCategory(Guid id)
        {
            return await _service.Delete(id);
        }
    }
}
