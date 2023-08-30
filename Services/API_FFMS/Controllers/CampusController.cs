using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers;

public class CampusController : BaseController
{
    private readonly ICampusService _campusService;

    public CampusController(ICampusService campusService) 
    {
        _campusService = campusService;
    }
    
    [HttpGet]
    [SwaggerOperation("Get list ...")]
    public async Task<ApiResponses<CampusDto>> GetListCampus([FromQuery]CampusQueryDto queryDto)
    {
        return await _campusService.GetCampus(queryDto);
    }
}