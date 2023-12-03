using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers;

public class InventoryCheckController : BaseController
{
    private readonly IInventoryCheckService _inventoryCheckService;

    public InventoryCheckController(IInventoryCheckService inventoryCheckService)
    {
        _inventoryCheckService = inventoryCheckService;
    }

    [HttpPost]
    [SwaggerOperation("Create new inventory check")]
    public async Task<ApiResponse> Create([FromBody] InventoryCheckCreateDto createDto)
    {
        return await _inventoryCheckService.Create(createDto);
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation("Get a inventory check")]
    public async Task<ApiResponse<InventoryCheckDto>> GetInventoryCheck(Guid id)
    {
        return await _inventoryCheckService.GetInventoryCheck(id);
    }

    [HttpGet]
    [SwaggerOperation("Get list inventory check")]
    public async Task<ApiResponses<InventoryCheckDto>> GetInventoryChecks([FromQuery] InventoryCheckQueryDto queryDto)
    {
        return await _inventoryCheckService.GetInventoryChecks(queryDto);
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation("Delete a inventory check")]
    public async Task<ApiResponse> Delete(Guid id)
    {
        return await _inventoryCheckService.Delete(id);
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation("Update a inventory check")]
    public async Task<ApiResponse> Update(Guid id, BaseRequestUpdateDto updateDto)
    {
        return await _inventoryCheckService.Update(id, updateDto);
    }

    [HttpPut("status-update/{id:guid}")]
    [SwaggerOperation("Confirm or reject inventory check request")]
    public async Task<ApiResponse> ConfirmOrReject(Guid id, BaseUpdateStatusDto confirmOrRejectDto)
    {
        return await _inventoryCheckService.ConfirmOrReject(id, confirmOrRejectDto);
    }
}