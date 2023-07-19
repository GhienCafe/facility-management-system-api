using Microsoft.AspNetCore.Mvc;

namespace API_SystemManager.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[Controller]")]
public class BaseController : ControllerBase
{
}