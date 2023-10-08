using System.Linq.Expressions;
using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
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
    Task<ApiResponse<IEnumerable<UserDto>>> GetListBasedOnCategory(Guid categoryId);
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
            throw new ApiException("Không tìm thấy người dùng", StatusCode.NOT_FOUND);
        
        user.UserCode = updateDto.UserCode ?? user.UserCode;
        user.Fullname = updateDto.Fullname ?? user.Fullname;
        user.Address = updateDto.Address ?? user.Address;
        user.Gender = updateDto.Gender ?? user.Gender;
        user.Avatar = updateDto.Avatar ?? user.Avatar;
        user.Dob = updateDto.Dob ?? user.Dob;
        user.Role = updateDto.Role ?? user.Role;
        user.PhoneNumber = updateDto.PhoneNumber ?? user.PhoneNumber;
        user.PersonalIdentifyNumber = updateDto.PersonalIdentifyNumber ?? user.PersonalIdentifyNumber;
        user.Status = updateDto.Status ?? user.Status;

        if (!await MainUnitOfWork.UserRepository.UpdateAsync(user, AccountId, CurrentDate))
            throw new ApiException("Cập nhật thất bại", StatusCode.SERVER_ERROR);
        
        return ApiResponse.Success("Cập nhật thành công");
    }
    
    public async Task<ApiResponse> Create(UserCreateDto createDto)
    {
        var user = createDto.ProjectTo<UserCreateDto, User>();

        user.Password = "password123@";
        var salt = SecurityExtension.GenerateSalt();
        user.Password = SecurityExtension.HashPassword<User>(user.Password, salt);
        user.Salt = salt;
        user.Status = UserStatus.Active;

        if (!await MainUnitOfWork.UserRepository.InsertAsync(user, AccountId, CurrentDate))
            throw new ApiException("Thêm thất bại", StatusCode.SERVER_ERROR);
        
        return ApiResponse.Created("Thêm thành công");
    }

    public async Task<ApiResponse<IEnumerable<UserDto>>> GetListBasedOnCategory(Guid categoryId)
    {
        var category = await MainUnitOfWork.CategoryRepository.FindOneAsync(categoryId);

        if (category == null)
            throw new ApiException("Không tồn tại nhóm trang thiết bị", StatusCode.BAD_REQUEST);

        var teamMemberIds = await MainUnitOfWork.TeamMemberRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue && x.TeamId == category.TeamId).Select(x => x!.MemberId)
            .ToListAsync();

        var member = await MainUnitOfWork.UserRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue && teamMemberIds.Contains(x.Id)).ToListAsync();

        var memberDto = member!.ProjectTo<User, UserDto>();

        memberDto = await _mapperRepository.MapCreator(memberDto);
        
        return ApiResponse<IEnumerable<UserDto>>.Success(memberDto);
    }

    public async Task<ApiResponse<UserDetailDto>> GetDetail(Guid id)
    {
        var user = await MainUnitOfWork.UserRepository.GetQuery().Where(x => !x!.DeletedAt.HasValue)
            .Where(x => x!.Id == id).Select(x => new UserDetailDto
            {
                Id = x!.Id,
                Gender = x.Gender,
                Role = x.Role,
                Status = x.Status,
                RoleObj = x.Role.GetValue(),
                StatusObj = x.Status.GetValue(),
                Address = x.Address,
                Avatar = x.Avatar,
                Dob = x.Dob,
                Fullname = x.Fullname,
                PhoneNumber = x.PhoneNumber,
                Email = x.Email,
                UserCode = x.UserCode,
                PersonalIdentifyNumber = x.PersonalIdentifyNumber,
                FirstLoginAt = x.FirstLoginAt,
                LastLoginAt = x.LastLoginAt,
                CreatedAt = x.CreatedAt,
                EditedAt = x.EditedAt,
                CreatorId = x.CreatorId ?? Guid.Empty,
                EditorId = x.EditorId ?? Guid.Empty,
            }).FirstOrDefaultAsync();
        
        if (user == null)
            throw new ApiException("Không tìm thấy người dùng");
        
        user = await _mapperRepository.MapCreator(user);
        
        return ApiResponse<UserDetailDto>.Success(user);
    }

    public async Task<ApiResponses<UserDto>> GetList(UserQueryDto queryDto)
    {
        var keyword = queryDto.Keyword?.Trim().ToLower();
        
        var userDataset = MainUnitOfWork.UserRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue && x.Role != UserRole.Administrator);
        if (!string.IsNullOrEmpty(keyword))
        {
            userDataset = userDataset.Where(x => x!.UserCode.ToLower().Contains(keyword)
                                                 || x.Email.Contains(keyword)
                                                 || x.Fullname.Contains(keyword)
                                                 || x.PhoneNumber.Contains(keyword)
                                                 || x.PersonalIdentifyNumber!.Contains(keyword));
        }
        
        if (queryDto.Role != null)
        {
            userDataset = userDataset.Where(x => x!.Role == queryDto.Role);
        }
        
        if (queryDto.Gender != null)
        {
            userDataset = userDataset.Where(x => x!.Gender == queryDto.Gender);
        }
        
        if (queryDto.Status != null)
        {
            userDataset = userDataset.Where(x => x!.Status == queryDto.Status);
        }
        
        // if (queryDto.TeamId != null)
        // {
        //     userDataset = userDataset.Where(x => x! == queryDto.TeamId);
        // }
        
        if (queryDto.CreateAtFrom != null)
        {
            userDataset = userDataset.Where(x => x!.CreatedAt >= queryDto.CreateAtFrom);
        }
        
        if (queryDto.CreateAtTo != null)
        {
            userDataset = userDataset.Where(x => x!.CreatedAt <= queryDto.CreateAtTo);
        }
        
        // Order
        // var isDescending = queryDto.OrderBy.EndsWith("desc", StringComparison.OrdinalIgnoreCase);
        // var orderByColumn = queryDto.OrderBy.Split(' ')[0]; // Get the column to order by
        // userDataset = isDescending ? userDataset.OrderByDescending(user => EF.Property<object>(user!, orderByColumn)) : userDataset.OrderBy(user => EF.Property<object>(user!, orderByColumn));

        var totalCount = userDataset.Count();
        
        userDataset = userDataset.Skip(queryDto.Skip())
            .Take(queryDto.PageSize);
        
        var users = await userDataset.Select(x => new UserDto
        {
            Id = x!.Id,
            Gender = x.Gender,
            Role = x.Role,
            RoleObj = x.Role.GetValue(),
            Status = x.Status,
            StatusObj = x.Status.GetValue(),
            Address = x.Address,
            Avatar = x.Avatar,
            Dob = x.Dob,
            Fullname = x.Fullname,
            PhoneNumber = x.PhoneNumber,
            Email = x.Email,
            UserCode = x.UserCode,
            PersonalIdentifyNumber = x.PersonalIdentifyNumber,
            FirstLoginAt = x.FirstLoginAt,
            LastLoginAt = x.LastLoginAt,
            CreatedAt = x.CreatedAt,
            EditedAt = x.EditedAt,
            CreatorId = x.CreatorId ?? Guid.Empty,
            EditorId = x.EditorId ?? Guid.Empty,
        }).ToListAsync();
        
        users = await _mapperRepository.MapCreator(users);
        
        return ApiResponses<UserDto>.Success(
            users,
            totalCount,
            queryDto.PageSize,
            queryDto.Page,
            (int)Math.Ceiling(totalCount / (double)queryDto.PageSize)
        );
    }

    public async Task<ApiResponse> Delete(Guid id)
    {
        var existingUsers = await MainUnitOfWork.UserRepository.FindOneAsync(id);
        
        if (existingUsers == null)
            throw new ApiException("Không tìm thấy người dùng", StatusCode.NOT_FOUND);
        
        if (!await MainUnitOfWork.UserRepository.DeleteAsync(existingUsers, AccountId, CurrentDate))
            throw new ApiException("Xóa người dùng thất bại", StatusCode.SERVER_ERROR);

        return ApiResponse.Success();
    }
}