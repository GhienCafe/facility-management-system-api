using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers;

public class AccountController : BaseController
{
    private readonly IAccountService _userService;

    public AccountController(IAccountService userService)
    {
        _userService = userService;
    }
    [HttpGet("infor")]
    [SwaggerOperation("Get current account information")]
    public async Task<ApiResponse<AccountDto>> GetAccountInformation()
    {
        return await _userService.GetAccountInformation();
    }
}