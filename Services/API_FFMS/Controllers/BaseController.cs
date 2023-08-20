using MainData.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_FFMS.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/[Controller]")]
[Authorize]
public class BaseController : ControllerBase
{
    
}