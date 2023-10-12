using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers;

public class AssetCheckController : BaseController
{
    private readonly IAssetCheckService _assetCheckService;

    public AssetCheckController(IAssetCheckService assetCheckService)
    {
        _assetCheckService = assetCheckService;
    }

    [HttpGet]
    [SwaggerOperation("Get list asset check")]
    public async Task<ApiResponses<AssetCheckDto>> GetAssetChecks([FromQuery] AssetCheckQueryDto queryDto)
    {
        return await _assetCheckService.GetAssetChecks(queryDto);
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation("Get detail asset check")]
    public async Task<ApiResponse<AssetCheckDto>> GetAssetCheck(Guid id)
    {
        return await _assetCheckService.GetAssetCheck(id);
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation("Delete asset check")]
    public async Task<ApiResponse> Delete(Guid id)
    {
        return await _assetCheckService.Delete(id);
    }

    [HttpDelete]
    [SwaggerOperation("Delete list asset check")]
    public async Task<ApiResponse> DeleteAssetChecks(List<Guid> ids)
    {
        return await _assetCheckService.DeleteAssetChecks(ids);
    }

    [HttpPost]
    [SwaggerOperation("Create new asset check")]
    public async Task<ApiResponse> Create([FromBody] AssetCheckCreateDto createDto)
    {
        return await _assetCheckService.Create(createDto);
    }

    [HttpPut]
    [SwaggerOperation("Update asset check")]
    public async Task<ApiResponse> Update(Guid id, AssetCheckUpdateDto updateDto)
    {
        return await _assetCheckService.Update(id, updateDto);
    }
}