using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers;

public class CategoryController : BaseController
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [SwaggerOperation("Get list categories")]
    public async Task<ApiResponses<CategoryDto>> GetCategories([FromQuery] CategoryQueryDto queryDto)
    {
        return await _categoryService.GetCategories(queryDto);
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation("Get category")]
    public async Task<ApiResponse<CategoryDetailDto>> GetCategory(Guid id)
    {
        return await _categoryService.GetCategory(id);
    }

    [HttpPost]
    [SwaggerOperation("Create a new category")]
    public async Task<ApiResponse> Create([FromBody] CategoryCreateDto createDto)
    {
        return await _categoryService.Create(createDto);
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation("Update category information")]
    public async Task<ApiResponse> Update(Guid id, CategoryUpdateDto updateDto)
    {
        return await _categoryService.Update(id, updateDto);
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation("Delete category")]
    public async Task<ApiResponse> Delete(Guid id)
    {
        return await _categoryService.Delete(id);
    }

    [HttpDelete]
    [SwaggerOperation("Delete category")]
    public async Task<ApiResponse> DeleteCategories(DeleteMutilDto deleteDto)
    {
        return await _categoryService.DeleteCategories(deleteDto);
    }
}