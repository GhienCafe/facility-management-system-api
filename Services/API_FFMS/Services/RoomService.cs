using System.Linq.Expressions;
using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace API_FFMS.Services;
public interface IRoomService : IBaseService
{
    Task<ApiResponses<RoomDto>> GetRoom(RoomQueryDto queryDto);
    Task<ApiResponse<RoomDetailDto>> GetRoom(Guid id);
    
    public Task<ApiResponse> Insert(RoomCreateDto addRoomDto);
    public Task<ApiResponse<RoomDetailDto>> Update(Guid id, RoomUpdateDto floorUpdate);
    Task<ApiResponse> Delete(Guid id);

}
public class RoomService : BaseService, IRoomService
{
    public RoomService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor,
        IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public async Task<ApiResponses<RoomDto>> GetRoom(RoomQueryDto queryDto)
    {
        Expression<Func<Rooms, bool>>[] conditions = new Expression<Func<Rooms, bool>>[]
        {
            x => !x.DeletedAt.HasValue
        };

        if (string.IsNullOrEmpty(queryDto.RoomNumber)==false)
        {
            conditions = conditions.Append(x => x.RoomNumber.Trim().ToLower() == queryDto.RoomNumber.Trim().ToLower()).ToArray();
        }

        var response = await MainUnitOfWork.RoomsRepository.FindResultAsync<RoomDto>(conditions, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);
        return ApiResponses<RoomDto>.Success(
            response.Items,
            response.TotalCount,
            queryDto.PageSize,
            queryDto.Skip(),
            (int)Math.Ceiling(response.TotalCount / (double)queryDto.PageSize)
        );
    }

    public async Task<ApiResponse<RoomDetailDto>> GetRoom(Guid id)
    {
        var room = await MainUnitOfWork.RoomsRepository.FindOneAsync<RoomDetailDto>(
            new Expression<Func<Rooms, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.Id == id
            });

        if (room == null)
            throw new ApiException("Not found this room", StatusCode.NOT_FOUND);

        // Map CDC for the post
        room = await _mapperRepository.MapCreator(room);

        return ApiResponse<RoomDetailDto>.Success(room);
    }

    public async Task<ApiResponse> Insert(RoomCreateDto roomDto)
    {
        if (!roomDto.RoomNumber.IsBetweenLength(1, 255))
        {
            throw new ApiException("Can not create room when address is null or must length of characters 1-255", StatusCode.ALREADY_EXISTS);
        }
        
        if (MainUnitOfWork.RoomsRepository.GetQuery()
                .Where(x => x.RoomNumber.Trim().ToLower().Contains(roomDto.RoomNumber.Trim().ToLower()))
                .SingleOrDefault() != null)
        {
            throw new ApiException("Room name was used, please again!", StatusCode.BAD_REQUEST);

        }
        
        var floor = MainUnitOfWork.FloorsRepository.GetQuery()
            .Where(x => x.Id == roomDto.FloorId)
            .SingleOrDefault();

        if (floor == null)
        {
            throw new ApiException("Invalid FloorId, cannot be null", StatusCode.BAD_REQUEST);
        }
        // Calculate the total area of all rooms on the floor
        double? totalRoomAreaOnFloor = floor.Rooms.Sum(r => r.Area);

        // Check if adding the new room will exceed the floor's total area
        if (totalRoomAreaOnFloor + roomDto.Area > floor.Area)
        {
            throw new ApiException("The total room area exceeds the floor's total area", StatusCode.BAD_REQUEST);
        }
        var room =roomDto.ProjectTo<RoomCreateDto, Rooms>();
        bool response = await MainUnitOfWork.RoomsRepository.InsertAsync(room, AccountId);
        
        if (response)
        {
            return ApiResponse<bool>.Success(true);
        }
        else
        {
            return (ApiResponse<bool>)ApiResponse.Failed();
        }
    }
    public async Task<ApiResponse<RoomDetailDto>> Update(Guid id, RoomUpdateDto roomDto)
    {
        var room = await MainUnitOfWork.RoomsRepository.FindOneAsync(id);
        if (room==null)
        {
            throw new ApiException("Not found this room", StatusCode.NOT_FOUND);
        }
        if (!roomDto.RoomNumber.IsBetweenLength(1, 50))
        {
            throw new ApiException("Can not create room when description is null or must length of characters 1-255", StatusCode.BAD_REQUEST);
        }
        var roomUpdate = roomDto.ProjectTo<RoomUpdateDto, Rooms>();
        if (!await MainUnitOfWork.RoomsRepository.UpdateAsync(roomUpdate, AccountId, CurrentDate))
            throw new ApiException("Can't not update", StatusCode.SERVER_ERROR);

        return await GetRoom(id);
    }
    public async Task<ApiResponse> Delete(Guid id)
    {
        var existingroom = await MainUnitOfWork.RoomsRepository.FindOneAsync(id);
        if (existingroom == null)
            throw new ApiException("Not found this room", StatusCode.NOT_FOUND);
        if (!await MainUnitOfWork.RoomsRepository.DeleteAsync(existingroom, AccountId, CurrentDate))
            throw new ApiException("Can't not delete", StatusCode.SERVER_ERROR);

        return ApiResponse.Success();
    }
}