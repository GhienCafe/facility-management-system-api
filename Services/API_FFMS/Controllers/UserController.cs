using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers;

public class UserController : BaseController
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }
    [HttpGet]
    [SwaggerOperation("Get list")]
    public async Task<ApiResponses<UserDto>> GetUsers([FromQuery]UserQueryDto queryDto)
    {
        return await _service.GetList(queryDto);
    }
    
    [HttpGet("category/{id}")]
    [SwaggerOperation("Get list user based on category id")]
    public async Task<ApiResponse<IEnumerable<UserDto>>> GetUsers(Guid id)
    {
        return await _service.GetListBasedOnCategory(id);
    }
    
    [HttpGet("{id:guid}")]
    [SwaggerOperation("Get detail information")]
    public async Task<ApiResponse<UserDetailDto>> GetDetailUser(Guid id)
    {
        return await _service.GetDetail(id);
    }
    [HttpPost]
    [SwaggerOperation("Create new user")]
    public async Task<ApiResponse> Insert([FromBody]UserCreateDto userDto)
    {
        return await _service.Create(userDto);
    }
    [HttpPut("{id:guid}")]
        
    [SwaggerOperation("Update user information")]
    public async Task<ApiResponse> Update(Guid id, UserUpdateDto updateDto)
    {
        return await _service.Update(id, updateDto);
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation("Delete user")]
    public async Task<ApiResponse> Delete(Guid id)
    {
        return await _service.Delete(id);
    }

    [HttpDelete]
    [SwaggerOperation("Delete list users")]
    public async Task<ApiResponse> DeleteUsers(DeleteMutilDto deleteDto)
    {
        return await _service.DeleteUsers(deleteDto);
    }
}