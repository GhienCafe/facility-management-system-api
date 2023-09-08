using MainData.Entities;
using MainData.Middlewares;
using Microsoft.AspNetCore.Mvc;

namespace API_FFMS.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/[Controller]")]
[Authorize(new[] { UserRole.CampusManagers , UserRole.FacilitiesManager, UserRole.GlobalManager, UserRole.TechnicalSpecialist})]
public class BaseController : ControllerBase
{
    
}