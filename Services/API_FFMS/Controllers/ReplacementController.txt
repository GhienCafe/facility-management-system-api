using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Task = DocumentFormat.OpenXml.Office2021.DocumentTasks.Task;

namespace API_FFMS.Controllers;

public class ReplacementController : BaseController
{
    private readonly IReplacementService _replacementService;

    public ReplacementController(IReplacementService replacementService)
    {
        _replacementService = replacementService;
    }
    
    // [HttpPost]
    // [SwaggerOperation("Create new replacement")]
    // public async Task<ApiResponse> Create([FromBody] CreateReplacementDto createDto)
    // {
    //     return await _replacementService.Create(createDto);
    // }
    //
    // [HttpPut("{id}")]
    // [SwaggerOperation("Update replacement")]
    // public async Task<ApiResponse> Update(Guid id, UpdateReplacementDto updateDto)
    // {
    //     return await _replacementService.Update(id, updateDto);
    // }
    //
    // [HttpGet("{id:guid}")]
    // [SwaggerOperation("Replace detail information")]
    // public async Task<ApiResponse<DetailReplacementDto>> GetDetail(Guid id)
    // {
    //     return await _replacementService.GetDetail(id);
    // }
    //
    // [HttpGet]
    // [SwaggerOperation("Get all replacement")]
    // public async Task<ApiResponses<ReplacementDto>> GetList([FromQuery] QueryReplacementDto queryDto)
    // {
    //     return await _replacementService.GetList(queryDto);
    // }
    // [HttpDelete("{id}")]
    // [SwaggerOperation("Delete replacement")]
    // public async Task<ApiResponse> Delete(Guid id)
    // {
    //     return await _replacementService.Delete(id);
    // }
}