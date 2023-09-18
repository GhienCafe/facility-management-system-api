using System.Linq.Expressions;
using System.Text.RegularExpressions;
using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Services;

public interface IUserService : IBaseService
{
    Task<ApiResponse> Update(Guid id, UserUpdateDto updateDto);
    Task<ApiResponse> Create(UserCreateDto createDto);
    Task<ApiResponse> Delete(Guid id);
    Task<ApiResponses<UserDto>> GetList(UserQueryDto queryDto);
    Task<ApiResponse<UserDetailDto>> GetDetail(Guid id);
}
public class UserService : BaseService, IUserService
{
    public UserService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }
    public async Task<ApiResponse> Update(Guid id, UserUpdateDto updateDto)
    {
        var user = await MainUnitOfWork.UserRepository.FindOneAsync(id);

        if (user == null)
            throw new ApiException("Not found", StatusCode.NOT_FOUND);

        user.UserCode = updateDto.UserCode ?? user.UserCode;
        user.Fullname = updateDto.Fullname ?? user.Fullname;
        user.Address = updateDto.Address ?? user.Address;
        user.Email = updateDto.Email ?? user.Email;
        user.Gender = updateDto.Gender ?? user.Gender;
        user.Avatar = updateDto.Avatar ?? user.Avatar;
        user.Dob = updateDto.Dob ?? user.Dob;
        user.Role = updateDto.Role ?? user.Role;
        user.PhoneNumber = updateDto.PhoneNumber ?? user.PhoneNumber;
        user.PersonalIdentifyNumber = updateDto.PersonalIdentifyNumber ?? user.PersonalIdentifyNumber;

        if (!await MainUnitOfWork.UserRepository.UpdateAsync(user, AccountId, CurrentDate))
            throw new ApiException("Update fail", StatusCode.SERVER_ERROR);

        return ApiResponse.Success();
    }
    
    
    public async Task<ApiResponse> Create(UserCreateDto createDto)
    {
        var user = createDto.ProjectTo<UserCreateDto, User>();
        
        var salt = SecurityExtension.GenerateSalt();
        user.Password = SecurityExtension.HashPassword<User>(user.Password!, salt);
        user.Salt = salt;
        user.Status = UserStatus.Active;

        if (!await MainUnitOfWork.UserRepository.InsertAsync(user, AccountId, CurrentDate))
            throw new ApiException("Insert fail!", StatusCode.SERVER_ERROR);
        
        return ApiResponse.Created("Created successfully!");
    }
    
    
    public async Task<ApiResponse<UserDetailDto>> GetDetail(Guid id)
    {
        var existingUser = await MainUnitOfWork.UserRepository.FindOneAsync<UserDetailDto>(
            new Expression<Func<User, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.Id == id
            });
        
        if (existingUser == null)
            throw new ApiException("Not found this user", StatusCode.NOT_FOUND);
        
        existingUser = await _mapperRepository.MapCreator(existingUser);
        
        return ApiResponse<UserDetailDto>.Success(existingUser);
    }

    public async Task<ApiResponses<UserDto>> GetList(UserQueryDto queryDto)
    {
        var response = await MainUnitOfWork.UserRepository.FindResultAsync<UserDto>(new Expression<Func<User, bool>>[]
        {
            x => !x.DeletedAt.HasValue,
            x => string.IsNullOrEmpty(queryDto.Fullname) || x.Fullname.ToLower().Contains(queryDto.Fullname.Trim().ToLower()),
            x => string.IsNullOrEmpty(queryDto.UserCode) || x.UserCode.ToLower().Equals(queryDto.UserCode.Trim().ToLower()),
            x => string.IsNullOrEmpty(queryDto.Email) || x.Email.ToLower().Contains(queryDto.Email.Trim().ToLower())
        }, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);

        response.Items = await _mapperRepository.MapCreator(response.Items.ToList());
        
        return ApiResponses<UserDto>.Success(
            response.Items,
            response.TotalCount,
            queryDto.PageSize,
            queryDto.Skip(),
            (int)Math.Ceiling(response.TotalCount / (double)queryDto.PageSize)
        );
    }


    public async Task<ApiResponse> Delete(Guid id)
    {
        var existingUsers = await MainUnitOfWork.UserRepository.FindOneAsync(id);
        
        if (existingUsers == null)
            throw new ApiException("Not found this user", StatusCode.NOT_FOUND);
        
        if (!await MainUnitOfWork.UserRepository.DeleteAsync(existingUsers, AccountId, CurrentDate))
            throw new ApiException("Can't not delete", StatusCode.SERVER_ERROR);

        return ApiResponse.Success();
    }
}