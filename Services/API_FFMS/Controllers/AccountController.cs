using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers;

public class AccountController : BaseController
{
    private readonly IAccountService _userService;
    private readonly ICacheService _cacheService;

    public AccountController(IAccountService userService, ICacheService cacheService)
    {
        _userService = userService;
        _cacheService = cacheService;
    }
    [HttpGet("infor")]
    [SwaggerOperation("Get current account information")]
    public async Task<ApiResponse<AccountDto>> GetAccountInformation()
    {
        var key = "account_infor_";
        
        // check cache data
        var cacheData = _cacheService.GetData<AccountDto>(key);
        if (cacheData != null)
        {
            return ApiResponse<AccountDto>.Success(cacheData);
        }

        var response = await _userService.GetAccountInformation();
        
        // Leave it null - the default will be 5 minutes
        var expiryTime = DateTimeOffset.Now.AddMinutes(3);
        _cacheService.SetData(key, response.Data, expiryTime);
        
        return response;
    }

    [HttpPut]
    [SwaggerOperation("Update current account information")]
    public async Task<ApiResponse> Update(AccountUpdateDto updateDto)
    {
        return await _userService.AccountUpdate(updateDto);
    }
}