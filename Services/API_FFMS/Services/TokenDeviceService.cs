using API_FFMS.Dtos;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;

namespace API_FFMS.Services;

public interface ITokenDeviceService : IBaseService
{
    Task<ApiResponse> CheckTokenDevice(TokenDeviceDto tokenDto);
}
public class TokenDeviceService :BaseService, ITokenDeviceService
{
    public TokenDeviceService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public async Task<ApiResponse> CheckTokenDevice(TokenDeviceDto tokenDto)
    {
        var user = MainUnitOfWork.UserRepository
            .GetQuery()
            .SingleOrDefault(x =>x != null && !x!.DeletedAt.HasValue && x.Id == AccountId);
        if (user == null)
        {
            throw new ApiException("User not found", StatusCode.BAD_REQUEST);
        }

        var existingToken = MainUnitOfWork.TokenRepository
            .GetQuery()
            .SingleOrDefault(x => !x!.DeletedAt.HasValue && x.UserId == AccountId && x.Type == TokenType.DeviceToken);

        if (existingToken != null)
        {
            existingToken.Type = TokenType.DeviceToken;
            existingToken.AccessToken = tokenDto.Token;
            existingToken.RefreshToken = tokenDto.Token;
            await MainUnitOfWork.TokenRepository.UpdateAsync(existingToken, AccountId, CurrentDate);
            // Return a success message
            return ApiResponse.Success();
        }

        // Create a new token and store it in the database
        var newToken = new Token
        {
            AccessToken = tokenDto.Token,
            RefreshToken = tokenDto.Token, 
            Status = TokenStatus.Active,
            UserId = AccountId ?? Guid.Empty,
            Type = TokenType.DeviceToken,
            AccessExpiredAt = DateTime.UtcNow.AddHours(1), 
            RefreshExpiredAt = DateTime.UtcNow.AddMonths(1), 
        };

        if (!await MainUnitOfWork.TokenRepository.InsertAsync(newToken, AccountId, CurrentDate))
        {
            return ApiResponse.Failed();
        }

        // Return a success message
        return ApiResponse.Success();
    }
}