using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers;

public class VirtualizeController : BaseController
{
    private readonly IVirtualizeService _service;

    public VirtualizeController(IVirtualizeService service) 
    {
        _service = service;
    }
    [HttpGet]
    [SwaggerOperation("Get virtualize")]
    public async Task<ApiResponses<VirtualizeDto>> GetVirtualize([FromQuery]VirtualizeQueryDto queryDto)
    {
        return await _service.GetVirtualize(queryDto);
    }
}