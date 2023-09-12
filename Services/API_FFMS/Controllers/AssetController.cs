﻿using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetController : ControllerBase
    {
        private readonly IAssetService _assetService;

        public AssetController(IAssetService assetService)
        {
            _assetService = assetService;
        }

        [HttpGet]
        [SwaggerOperation("Get list asset")]
        public async Task<ApiResponses<AssetDto>> GetAllAssets([FromQuery] AssetQueryDto queryDto)
        {
            return await _assetService.GetAllAssets(queryDto);
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

        [HttpPut]
        [SwaggerOperation("Update an asset")]
        public async Task<ApiResponse<AssetDetailDto>> Update(Guid id, AssetUpdateDto updateDto)
        {
            return await _assetService.Update(id, updateDto);
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation("Delete asset")]
        public async Task<ApiResponse> Delete(Guid id)
        {
            return await _assetService.Delete(id);
        }
    }
}