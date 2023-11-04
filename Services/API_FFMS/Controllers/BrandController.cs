using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers;

public class BrandController : BaseController
{
    private readonly IBrandService _brandService;

    public BrandController(IBrandService brandService)
    {
        _brandService = brandService;
    }

    [HttpGet]
    [SwaggerOperation("Get list models")]
    public async Task<ApiResponses<BrandDto>> GetBrands([FromQuery] BrandQueryDto queryDto)
    {
        return await _brandService.GetBrands(queryDto);
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation("Get details an brand")]
    public async Task<ApiResponse<BrandDto>> GetBrand(Guid id)
    {
        return await _brandService.GetBrand(id);
    }

    [HttpPost]
    [SwaggerOperation("Create new brand")]
    public async Task<ApiResponse> Create([FromBody] BrandCreateDto createDto)
    {
        return await _brandService.Create(createDto);
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation("Update a brand")]
    public async Task<ApiResponse> Update(Guid id, BrandUpdateDto updateDto)
    {
        return await _brandService.Update(id, updateDto);
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation("Delete brand")]
    public async Task<ApiResponse> Delete(Guid id)
    {
        return await _brandService.Delete(id);
    }

    [HttpDelete]
    [SwaggerOperation("Delete list brand")]
    public async Task<ApiResponse> DeleteBrands(DeleteMutilDto deleteDto)
    {
        return await _brandService.DeleteBrands(deleteDto);
    }
}