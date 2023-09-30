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
    public Task<ApiResponse> Update(Guid id, RoomStatusUpdateDto addRoomStatusDto);
    Task<ApiResponse> Delete(Guid id);

}
public class RoomStatusService :BaseService,IRoomStatusService
{
    public RoomStatusService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public async Task<ApiResponses<RoomStatusDto>> GetRoomStatus(RoomStatusQueryDto queryDto)
    {
        var roomStatuses = await MainUnitOfWork.RoomStatusRepository.FindResultAsync<RoomStatusDto>(
            new Expression<Func<RoomStatus, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => string.IsNullOrEmpty(queryDto.StatusName) ||
                     x.StatusName.ToLower().Contains(queryDto.StatusName.Trim().ToLower())
            }, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);

        roomStatuses.Items = await _mapperRepository.MapCreator(roomStatuses.Items.ToList());
        
        return ApiResponses<RoomStatusDto>.Success(
            roomStatuses.Items,
            roomStatuses.TotalCount,
            queryDto.PageSize,
            queryDto.Page,
            (int)Math.Ceiling(roomStatuses.TotalCount / (double)queryDto.PageSize)
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
        throw new ApiException("Không tìm thấy trạng thái", StatusCode.NOT_FOUND);

      // Map CDC for the post
      roomStatus = await _mapperRepository.MapCreator(roomStatus);

      return ApiResponse<RoomStatusDetailDto>.Success(roomStatus);
    }

    public async Task<ApiResponse> Insert(RoomStatusCreateDto roomStatusDto)
    {
        var roomStatus = roomStatusDto.ProjectTo<RoomStatusCreateDto, RoomStatus>();
        if (!await MainUnitOfWork.RoomStatusRepository.InsertAsync(roomStatus, AccountId, CurrentDate))
            throw new ApiException("Thêm trạng thái thất bại", StatusCode.SERVER_ERROR);
        
        return ApiResponse.Created("Thêm trạng thái thành công");
    }

    public async Task<ApiResponse> Update(Guid id, RoomStatusUpdateDto updateDto)
    {
        var existingRoomStatus = await MainUnitOfWork.RoomStatusRepository.FindOneAsync(id);
        
        if (existingRoomStatus == null)
            throw new ApiException("Không tìm thấy trạng thái", StatusCode.NOT_FOUND);

        existingRoomStatus.StatusName = updateDto.StatusName ?? existingRoomStatus.StatusName;
        existingRoomStatus.Color = updateDto.Color ?? existingRoomStatus.Color;
        existingRoomStatus.Description = updateDto.Description ?? existingRoomStatus.Description;
        
        if (!await MainUnitOfWork.RoomStatusRepository.UpdateAsync(existingRoomStatus, AccountId, CurrentDate))
            throw new ApiException("Cập nhật trạng thái thất bại", StatusCode.SERVER_ERROR);
        
        return ApiResponse.Created("Cập nhật trạng thái thành công");
    }

    public async Task<ApiResponse> Delete(Guid id)
    {
        var existingRoomStatus = await MainUnitOfWork.RoomStatusRepository.FindOneAsync(id);
        
        if (existingRoomStatus == null)
            throw new ApiException("Không tìm thấy trạng thái", StatusCode.NOT_FOUND);
        
        if (!await MainUnitOfWork.RoomStatusRepository.DeleteAsync(existingRoomStatus, AccountId, CurrentDate))
            throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);

        return ApiResponse.Success();
    }
}