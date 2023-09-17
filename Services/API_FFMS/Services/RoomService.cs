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
        var rooms = await MainUnitOfWork.RoomRepository.FindResultAsync<RoomDto>(new Expression<Func<Room, bool>>[]
        {
            x => !x.DeletedAt.HasValue,
            x => queryDto.RoomCode == null || x.RoomCode == queryDto.RoomCode,
            x => queryDto.RoomType == null || x.RoomType == queryDto.RoomType
        }, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);

        rooms.Items = await _mapperRepository.MapCreator(rooms.Items.ToList());
        
        return ApiResponses<RoomDto>.Success(
            rooms.Items,
            rooms.TotalCount,
            queryDto.PageSize,
            queryDto.Page,
            rooms.TotalCount/queryDto.PageSize
            );
    }

    public async Task<ApiResponse<RoomDetailDto>> GetRoom(Guid id)
    {
        var room = await MainUnitOfWork.RoomRepository.FindOneAsync<RoomDetailDto>(
            new Expression<Func<Room, bool>>[]
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
       
        throw new ApiException("Not implements");
    }
    public async Task<ApiResponse<RoomDetailDto>> Update(Guid id, RoomUpdateDto roomDto)
    {
        var room = await MainUnitOfWork.RoomRepository.FindOneAsync(id);
        if (room==null)
        {
            throw new ApiException("Not found this room", StatusCode.NOT_FOUND);
        }
        if (!roomDto.RoomCode.IsBetweenLength(1, 50))
        {
            throw new ApiException("Can not create room when description is null or must length of characters 1-255", StatusCode.BAD_REQUEST);
        }
        var roomUpdate = roomDto.ProjectTo<RoomUpdateDto , Room>();
        if (!await MainUnitOfWork.RoomRepository.UpdateAsync(roomUpdate, AccountId, CurrentDate))
            throw new ApiException("Can't not update", StatusCode.SERVER_ERROR);

        return await GetRoom(id);
    }
    public async Task<ApiResponse> Delete(Guid id)
    {
        var existingroom = await MainUnitOfWork.RoomRepository.FindOneAsync(id);
        if (existingroom == null)
            throw new ApiException("Not found this room", StatusCode.NOT_FOUND);
        if (!await MainUnitOfWork.RoomRepository.DeleteAsync(existingroom, AccountId, CurrentDate))
            throw new ApiException("Can't not delete", StatusCode.SERVER_ERROR);

        return ApiResponse.Success();
    }
}