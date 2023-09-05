using System.Linq.Expressions;
using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Services;

public interface IRoomStatusService : IBaseService
{
    Task<ApiResponses<RoomStatusDto>> GetRoomStatus(RoomStatusQueryDto queryDto);
    Task<ApiResponse<RoomStatusDetailDto>> GetRoomStatus(Guid id);
    
    public Task<ApiResponse> Insert(RoomStatusCreateDto addRoomStatusDto);
    Task<ApiResponse> Delete(Guid id);

}
public class RoomStatusService :BaseService,IRoomStatusService
{
    public RoomStatusService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public async Task<ApiResponses<RoomStatusDto>> GetRoomStatus(RoomStatusQueryDto queryDto)
    {
        Expression<Func<RoomStatus, bool>>[] conditions = new Expression<Func<RoomStatus, bool>>[]
        {
            x => !x.DeletedAt.HasValue
        };

        if (string.IsNullOrEmpty(queryDto.Color)==false)
        {
            conditions = conditions.Append(x => x.Color.Trim().ToLower() == queryDto.Color.Trim().ToLower()).ToArray();
        }

        var response = await MainUnitOfWork.RoomStatusRepository.FindResultAsync<RoomStatusDto>(conditions, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);
        return ApiResponses<RoomStatusDto>.Success(
            response.Items,
            response.TotalCount,
            queryDto.PageSize,
            queryDto.Skip(),
            (int)Math.Ceiling(response.TotalCount / (double)queryDto.PageSize)
        );
    }

    public async Task<ApiResponse<RoomStatusDetailDto>> GetRoomStatus(Guid id)
    {
      var roomStatus = await MainUnitOfWork.RoomStatusRepository.FindOneAsync<RoomStatusDetailDto>(
          new Expression<Func<RoomStatus, bool>>[]
          {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id
          });

      if (roomStatus == null)
        throw new ApiException("Not found this roomStatus", StatusCode.NOT_FOUND);

      // Map CDC for the post
      roomStatus = await _mapperRepository.MapCreator(roomStatus);

      return ApiResponse<RoomStatusDetailDto>.Success(roomStatus);
    }

    public async Task<ApiResponse> Insert(RoomStatusCreateDto roomStatusDto)
    {
        if (!roomStatusDto.Description.IsBetweenLength(1, 255))
        {
            return ApiResponse.Failed("Description is null or must have a length between 1 and 255 characters", StatusCode.BAD_REQUEST);
        }
        if (!roomStatusDto.StatusName.IsBetweenLength(1, 255))
        {
            return ApiResponse.Failed("StatusName is null or must have a length between 1 and 255 characters", StatusCode.BAD_REQUEST);
        }

        var checkDuplication = await MainUnitOfWork.RoomStatusRepository.GetQuery()
            .Where(x => x.Color == roomStatusDto.Color)
            .SingleOrDefaultAsync();

        if (checkDuplication != null)
        {
            return ApiResponse.Failed("Color is duplicated or not valid", StatusCode.BAD_REQUEST);
        }

        var roomStatus = roomStatusDto.ProjectTo<RoomStatusCreateDto, RoomStatus>();
        bool response = await MainUnitOfWork.RoomStatusRepository.InsertAsync(roomStatus, AccountId, DateTime.Today);

        if (response)
        {
            return ApiResponse.Success("RoomStatus created successfully");
        }
        else
        {
            return ApiResponse.Failed("Failed to create roomStatus", StatusCode.SERVER_ERROR);
        }
    }

    public async Task<ApiResponse> Delete(Guid id)
    {
        var existingRoomStatus = await MainUnitOfWork.RoomStatusRepository.FindOneAsync(id);
        if (existingRoomStatus == null)
            throw new ApiException("Not found this roomStatus", StatusCode.NOT_FOUND);
        if (!await MainUnitOfWork.RoomStatusRepository.DeleteAsync(existingRoomStatus, AccountId, CurrentDate))
            throw new ApiException("Can't not delete", StatusCode.SERVER_ERROR);

        return ApiResponse.Success();
    }
}