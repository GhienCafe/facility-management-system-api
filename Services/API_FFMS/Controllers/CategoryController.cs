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
    public async Task<ApiResponses<CategoryDto>> GetCategories([FromQuery]CategoryQueryDto queryDto)
    {
        return await _categoryService.GetCategories(queryDto);
    }
}