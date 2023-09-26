using System.Linq.Expressions;
using API_FFMS.Dtos;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Services;

public interface IAccountService : IBaseService
{
    Task<ApiResponse<AccountDto>> GetAccountInformation();
    Task<ApiResponse> AccountUpdate(AccountUpdateDto updateDto);
}
public class AccountService : BaseService, IAccountService
{
    public AccountService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }
    public async Task<ApiResponse<AccountDto>> GetAccountInformation()
    {
        var account = await MainUnitOfWork.UserRepository.FindOneAsync<AccountDto>(new Expression<Func<User, bool>>[]
        {
            x => !x.DeletedAt.HasValue,
            x => x.Id == AccountId
        });
        
        if (account == null)
        {
            throw new ApiException("Không tìm thấy tài khoản này ", StatusCode.NOT_FOUND);
        }

        account = await _mapperRepository.MapCreator(account);
        
        return ApiResponse<AccountDto>.Success(account);
    }
    public async Task<ApiResponse>AccountUpdate(AccountUpdateDto updateDto)
    {
        var account = await MainUnitOfWork.UserRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue && x.Id == AccountId).SingleOrDefaultAsync();
        if (account==null)
        {
            throw new ApiException("Không tìm thấy tài khoản này", StatusCode.NOT_FOUND);
        }

        account.Address = updateDto.Address;
        if (updateDto.Avatar!.Trim().ToLower()
            .Contains(("https://firebasestorage.googleapis.com/v0/b/facility-management-system-fb.appspot.com/o/")
                .Trim().ToLower()))
        {
            account.Avatar = updateDto.Avatar;
        }
        else throw new ApiException("Hình ảnh không hợp lệ", StatusCode.BAD_REQUEST);
        account.Dob = updateDto.Dob;
        account.Fullname = updateDto.Fullname;
        if (!await MainUnitOfWork.UserRepository.UpdateAsync(account, AccountId, CurrentDate))
        {
            throw new ApiException("Cập nhật thất bại");
        }
        return ApiResponse.Success();
    }
}