﻿using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetTypeController : ControllerBase
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
        [SwaggerOperation("Update asset type")]
        public async Task<ApiResponse<AssetTypeDetailDto>> UpdateAssetType(Guid id, AssetTypeUpdateDto updateDto)
        {
            return await _service.Update(id, updateDto);
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation("Delete asset type")]
        public async Task<ApiResponse> DeleteAssetType(Guid id)
        {
            return await _service.Delete(id);
        }
    }
}