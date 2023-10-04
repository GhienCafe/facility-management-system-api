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
}