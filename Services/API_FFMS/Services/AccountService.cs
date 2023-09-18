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
            throw new ApiException("Not found user", StatusCode.NOT_FOUND);
        }

        account = await _mapperRepository.MapCreator(account);
        
        return ApiResponse<AccountDto>.Success(account);
    }
}