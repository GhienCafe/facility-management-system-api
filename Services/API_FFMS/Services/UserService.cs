using System.Linq.Expressions;
using System.Text.RegularExpressions;
using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
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
    private async Task CheckPermission(Guid accountId)
    {
        var user = await MainUnitOfWork.UserRepository.GetQuery()
            .Where(x => x.Id == accountId)
            .SingleOrDefaultAsync();

        if (user?.Role != UserRole.Administrator)
        {
            throw new ApiException("Not valid with role", StatusCode.FORBIDDEN);
        }
    }
    public UserService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }
    public async Task<ApiResponse> Update(Guid id, UserUpdateDto updateDto)
    {
        await CheckPermission(AccountId.Value);
        var user = await MainUnitOfWork.UserRepository.FindOneAsync(new Expression<Func<User, bool>>[]
        {
            x => !x.DeletedAt.HasValue,
            x => x.Id == id
        });

        if (user == null)
            throw new ApiException("Not found this user", StatusCode.NOT_FOUND);

        var checkDepartmentId = MainUnitOfWork.DepartmentRepository.GetQuery()
            .Where(x => x.Id == updateDto.DepartmentId).SingleOrDefault();
        
        if (checkDepartmentId == null)
        {
            throw new ApiException("Department not found", StatusCode.NOT_FOUND);
        }

        if (user.Id != AccountId)
        {
            if (updateDto.Role != null)
            {
                user.Role = updateDto.Role;
            }
            else
            {
                throw new ApiException("Status must not empty", StatusCode.BAD_REQUEST);
            }
        }

        var DepartmentId = MainUnitOfWork.DepartmentRepository.GetQuery()
            .Where(x => x.Id == updateDto.DepartmentId).Select(x => x.Id).SingleOrDefault();

        user.Fullname = updateDto.Fullname ?? user.Fullname;
        user.DepartmentId = DepartmentId;
        user.Address = updateDto.Address ?? user.Address;
        
        user.Email = updateDto.Email ?? user.Email;
        if (updateDto.Status != null)
        {
            user.Status = updateDto.Status;
        }

        user.Avatar = updateDto.Avatar ?? user.Avatar;
        user.PhoneNumber = updateDto.PhoneNumber ?? user.PhoneNumber;
  
        if (!await MainUnitOfWork.UserRepository.UpdateAsync(user, AccountId, CurrentDate))
            throw new ApiException("Updated fail", StatusCode.SERVER_ERROR);

        return ApiResponse.Success();
    }
    
    
    public async Task<ApiResponse> Create(UserCreateDto createDto)
    {
        await CheckPermission(AccountId.Value);
        var existedEmail = await MainUnitOfWork.UserRepository.FindAsync(new Expression<Func<User, bool>>[]
        {
            x => !x.DeletedAt.HasValue,
            x => x.Email == createDto.Email
        }, null);

        if (existedEmail.Any())
            throw new ApiException("This email has been used", StatusCode.BAD_REQUEST);
    
        
        var user = createDto.ProjectTo<UserCreateDto, User>();
        var salt = SecurityExtension.GenerateSalt();
        user.Password = SecurityExtension.HashPassword<User>(createDto.Password, salt);
        user.Status = UserStatus.Active;
        user.Role = createDto.Role;
        user.Avatar = createDto.Avatar ?? user.Avatar;
        user.Salt = salt;
        
        
        // Get the part of the email address after the "@" symbol and limit it to 8 characters
        string email = createDto.Email.ToLower();
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ApiException("Email must not be empty.", StatusCode.BAD_REQUEST);
        }
        else
        {
            // Define a regular expression pattern to match valid email addresses
            string emailPattern = @"^(?i)[a-zA-Z0-9._%+-]+@(fpt\.edu\.vn|fe\.edu\.vn)$";

            // Use Regex.IsMatch to check if the email matches the pattern
            if (Regex.IsMatch(email, emailPattern))
            {
                user.Email = email.ToLower(); // Convert to lowercase if needed
            }
            else
            {
                throw new ApiException("Invalid email address.", StatusCode.BAD_REQUEST);
            }
        }
        int atIndex = email.IndexOf('@');
    
        if (atIndex == -1 || atIndex + 8 >= email.Length)
        {
            throw new ApiException("Email address must contain at least 8 characters after '@'.", StatusCode.BAD_REQUEST);
        }

        string userCode = email.Substring(atIndex - 8, 8);

        string UserCode=null;
        // Check if the user code already exists in the system
        if (MainUnitOfWork.UserRepository.GetQuery()
            .Where(x => x.UserCode.Trim().ToLower() == userCode.Trim().ToLower())
            .SingleOrDefault()!=null)
        {
            userCode = email.Substring(atIndex - 9, 8);
        };


        user.UserCode = userCode;
        
        string phoneNumber = createDto.PhoneNumber;
        if (user.Address.Equals(""))
        {
            throw new ApiException("Address must not be empty.", StatusCode.BAD_REQUEST);
        }
        else
        {
            user.Address = createDto.Address ?? user.Address;
        }

        if (createDto.Gender == null)
        {
            user.Gender = true;
        }
        else
        {
            user.Gender = createDto.Gender;
        }
        
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new ApiException("Phone number must not be empty.", StatusCode.BAD_REQUEST);
        }
        else
        {
            string phoneNumberPattern = @"^(?:\+84|0)[1-9]\d{8}$";

            // Use Regex.IsMatch to check if the phoneNumber matches the pattern
            if (Regex.IsMatch(phoneNumber, phoneNumberPattern))
            {
                user.PhoneNumber = phoneNumber;
            }
            else
            {
                throw new ApiException("Invalid phone number.", StatusCode.BAD_REQUEST);
            }
        }

        if (string.IsNullOrEmpty(createDto.Fullname))
        {
            throw new ApiException("Fullname must not be empty.", StatusCode.BAD_REQUEST);
        }
        else
        {
            user.Fullname = createDto.Fullname;
        }
        // Check if createDto.Dob is null or empty (assuming it's a nullable DateTime)
        if (!createDto.Dob.HasValue)
        {
            // createDto.Dob is null or empty, handle the error here
            throw new ApiException("Date of birth is required and must be a valid date.", StatusCode.BAD_REQUEST);
        }
        // Check if createDto.Dob is less than 0 (assuming it's a DateTime representing age)
        else if (createDto.Dob < DateTime.MinValue)
        {
            // createDto.Dob is less than 0, handle the error here
            throw new ApiException("Date of birth is invalid. It cannot be negative.", StatusCode.BAD_REQUEST);
        }
        else
        {
            user.Dob = createDto.Dob ?? user.Dob;
        }
        var DepartmentId = MainUnitOfWork.DepartmentRepository.GetQuery()
            .Where(x => x.Id == createDto.DepartmentId).Select(x => x.Id).SingleOrDefault(); 
        user.DepartmentId = DepartmentId;

        if (await MainUnitOfWork.UserRepository.InsertAsync(user, Guid.Empty, CurrentDate))
        {
            return ApiResponse.Success();
        }
        else throw new ApiException("Register user fail", StatusCode.SERVER_ERROR);
        return ApiResponse.Success();
    }
    
    
    public async Task<ApiResponse<UserDetailDto>> GetDetail(Guid id)
    {
        await CheckPermission(AccountId.Value);
        var existingUser = await MainUnitOfWork.UserRepository.FindOneAsync<UserDetailDto>(
            new Expression<Func<User, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.Id == id
            });

        if (existingUser == null)
            throw new ApiException("Not found this user", StatusCode.NOT_FOUND);

        existingUser = await _mapperRepository.MapCreator(existingUser);
        
        var departmentIsDeleted = await MainUnitOfWork.DepartmentRepository.FindOneAsync(
            new Expression<Func<Department, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.CreatorId == AccountId,
                x => x.Id == existingUser.DepartmentId
            }) ;
        if (departmentIsDeleted == null)
        {
            throw new ApiException("Department is delete with user");
        }
        return ApiResponse<UserDetailDto>.Success(existingUser);
    }

    public async Task<ApiResponses<UserDto>> GetList(UserQueryDto queryDto)
    {
        await CheckPermission(AccountId.Value);

        Expression<Func<User, bool>>[] conditions = new Expression<Func<User, bool>>[]
        {
            x => !x.DeletedAt.HasValue
        };

        if (string.IsNullOrEmpty(queryDto.UserCode)==false)
        {
            conditions = conditions.Append(x => x.UserCode.Trim().ToLower().Contains(queryDto.UserCode.Trim().ToLower())).ToArray();
        }
        if (string.IsNullOrEmpty(queryDto.Fullname)==false)
        {
            conditions = conditions.Append(x => x.Fullname.Trim().ToLower().Contains(queryDto.Fullname.Trim().ToLower())).ToArray();
        }
        if (string.IsNullOrEmpty(queryDto.Email)==false)
        {
            conditions = conditions.Append(x => x.Email.Trim().ToLower().Contains(queryDto.Email.Trim().ToLower())).ToArray();
        }
        var response = await MainUnitOfWork.UserRepository.FindResultAsync<UserDto>(conditions, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);
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
        if (MainUnitOfWork.UserRepository.GetQuery().Where(x=>x.Id==AccountId).SingleOrDefault().Role != UserRole.Administrator)
        {
            throw new ApiException("Not valid with role", StatusCode.FORBIDDEN);
        }
        var existingUsers = await MainUnitOfWork.UserRepository.FindOneAsync(id);
        if (existingUsers == null)
            throw new ApiException("Not found this user", StatusCode.NOT_FOUND);
        if (!await MainUnitOfWork.UserRepository.DeleteAsync(existingUsers, AccountId, CurrentDate))
            throw new ApiException("Can't not delete", StatusCode.SERVER_ERROR);

        return ApiResponse.Success();
    }
}