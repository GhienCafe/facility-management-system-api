using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers;

public class RequestController : BaseController
{
    private readonly IRequestService _requestService;

    public RequestController(IRequestService requestService)
    {
        _requestService = requestService;
    }

    [HttpPost]
    [SwaggerOperation("Create new action request")]
    public async Task<ApiResponse> Create([FromBody] ActionRequestCreateDto createDto)
    {
        return await _requestService.CreateRequest(createDto);
    }

    [HttpGet]
    [SwaggerOperation("Get action requests")]
    public async Task<ApiResponses<ActionRequestDto>> GetList([FromQuery] ActionRequestQuery queryDto)
    {
        return await _requestService.GetRequests(queryDto);
    }
}