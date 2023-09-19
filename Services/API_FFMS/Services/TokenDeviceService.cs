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
        // Check if the user is valid
        // Check if the user is valid (not deleted)
        var user = MainUnitOfWork.UserRepository
            .GetQuery()
            .SingleOrDefault(x =>x != null && !x.DeletedAt.HasValue && x.Id == AccountId);
        if (user == null)
        {
            throw new ApiException("User not found", StatusCode.BAD_REQUEST);
        }

        // Check if a token with Type as DeviceToken and corresponding UserId already exists
        var existingToken = MainUnitOfWork.TokenRepository
            .GetQuery()
            .SingleOrDefault(x => x != null && x.UserId == AccountId && x.Type == TokenType.DeviceToken);

        if (existingToken != null)
        {
            throw new ApiException("Token with Type DeviceToken already exists", StatusCode.ALREADY_EXISTS);
        }

        // Create a new token and store it in the database
        var newToken = new Token
        {
            AccessToken = tokenDto.Token, // Replace with your logic for generating an access token
            RefreshToken = tokenDto.Token, // Replace with your logic for generating a refresh token
            Status = TokenStatus.Active,
            UserId = AccountId ?? Guid.Empty,
            Type = TokenType.DeviceToken,
            AccessExpiredAt = DateTime.UtcNow.AddHours(1), // Token expiration time
            RefreshExpiredAt = DateTime.UtcNow.AddMonths(1), // Refresh token expiration time
        };

        if (!await MainUnitOfWork.TokenRepository.InsertAsync(newToken, AccountId, CurrentDate))
        {
            return ApiResponse.Failed();
        }

        // Return a success message
        return ApiResponse.Success();
    }
}