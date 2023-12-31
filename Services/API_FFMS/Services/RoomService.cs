﻿using System.Linq.Expressions;
using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Services;
public interface IRoomService : IBaseService
{
    Task<ApiResponses<RoomDto>> GetRoom(RoomQueryDto queryDto);
    Task<ApiResponse<RoomDetailDto>> GetRoom(Guid id);
    public Task<ApiResponse> Insert(RoomCreateDto addRoomDto);
    public Task<ApiResponse> Update(Guid id, RoomUpdateDto floorUpdate);
    Task<ApiResponse> Delete(Guid id);
    Task<ApiResponse> DeleteRooms(DeleteMutilDto deleteDto);

}
public class RoomService : BaseService, IRoomService
{
    public RoomService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor,
        IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public async Task<ApiResponses<RoomDto>> GetRoom(RoomQueryDto queryDto)
    {
        var keyword = queryDto.Keyword?.Trim().ToLower();
        var roomQuerySet = MainUnitOfWork.RoomRepository.GetQuery().Where(x => !x!.DeletedAt.HasValue);

        if (queryDto.RoomTypeId != null)
        {
            roomQuerySet = roomQuerySet.Where(x => x!.RoomTypeId == queryDto.RoomTypeId);
        }

        if (!string.IsNullOrEmpty(keyword))
        {
            roomQuerySet = roomQuerySet.Where(x => x!.RoomCode.ToLower().Contains(keyword)
                || x.RoomName!.ToLower().Contains(keyword) || x.Description!.ToLower().Contains(keyword));
        }

        if (queryDto.StatusId != null)
        {
            roomQuerySet = roomQuerySet.Where(x => x!.StatusId == queryDto.StatusId);
        }

        if (queryDto.FromArea != null)
        {
            roomQuerySet = roomQuerySet.Where(x => x!.Area >= queryDto.FromArea);
        }

        if (queryDto.ToArea != null)
        {
            roomQuerySet = roomQuerySet.Where(x => x!.Area <= queryDto.ToArea);
        }

        if (queryDto.FromCapacity != null)
        {
            roomQuerySet = roomQuerySet.Where(x => x!.Capacity >= queryDto.FromCapacity);
        }

        if (queryDto.ToCapacity != null)
        {
            roomQuerySet = roomQuerySet.Where(x => x!.Capacity <= queryDto.ToCapacity);
        }

        if (queryDto.FloorId != null)
        {
            roomQuerySet = roomQuerySet.Where(x => x!.FloorId == queryDto.FloorId);
        }

        var response = from room in roomQuerySet
                       join status in MainUnitOfWork.RoomStatusRepository.GetQuery() on room.StatusId equals status.Id
                           into statusGroup
                       from status in statusGroup.DefaultIfEmpty()
                       join type in MainUnitOfWork.RoomTypeRepository.GetQuery() on room.RoomTypeId equals type.Id
                           into typeGroup
                       from type in typeGroup.DefaultIfEmpty()
                       join floor in MainUnitOfWork.FloorRepository.GetQuery() on room.FloorId equals floor.Id
                       select new
                       {
                           Room = room,
                           Status = status,
                           Type = type,
                           Floor = floor
                       };

        var totalCount = response.Count();

        response = response.Skip(queryDto.Skip())
            .Take(queryDto.PageSize);

        var rooms = await response.Select(
            x => new RoomDto
            {
                Area = x.Room.Area,
                Capacity = x.Room.Capacity,
                Id = x.Room.Id,
                FloorId = x.Room.FloorId,
                PathRoom = x.Room.PathRoom,
                RoomName = x.Room.RoomName,
                Description = x.Room.Description,
                RoomTypeId = x.Room.RoomTypeId,
                RoomCode = x.Room.RoomCode,
                StatusId = x.Room.StatusId,
                Status = x.Status.ProjectTo<RoomStatus, RoomStatusDto>(),
                RoomType = x.Type.ProjectTo<RoomType, RoomTypeDto>(),
                Floor = x.Floor.ProjectTo<Floor, FloorBaseDto>()
            }
            ).ToListAsync();

        //rooms = await _mapperRepository.MapCreator(rooms);

        // Return data
        return ApiResponses<RoomDto>.Success(
            rooms,
            totalCount,
            queryDto.PageSize,
            queryDto.Page,
            (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
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
            throw new ApiException("Không tìm thấy phòng", StatusCode.NOT_FOUND);

        room.Status = await MainUnitOfWork.RoomStatusRepository.FindOneAsync<RoomStatusDto>(new Expression<Func<RoomStatus, bool>>[]
        {
            x => !x.DeletedAt.HasValue,
            x => x.Id == room.StatusId
        });

        room.RoomType = await MainUnitOfWork.RoomTypeRepository.FindOneAsync<RoomTypeDto>(new Expression<Func<RoomType, bool>>[]
        {
            x => !x.DeletedAt.HasValue,
            x => x.Id == room.RoomTypeId
        });

        // Map CDC for the post
        room = await _mapperRepository.MapCreator(room);

        return ApiResponse<RoomDetailDto>.Success(room);
    }

    public async Task<ApiResponse> Insert(RoomCreateDto roomDto)
    {
        var room = roomDto.ProjectTo<RoomCreateDto, Room>();
        if (!await MainUnitOfWork.RoomRepository.InsertAsync(room, AccountId, CurrentDate))
            throw new ApiException("Thêm mới thất bại", StatusCode.SERVER_ERROR);

        return ApiResponse.Created("Thêm mới thành công");
    }
    public async Task<ApiResponse> Update(Guid id, RoomUpdateDto roomDto)
    {
        var room = await MainUnitOfWork.RoomRepository.FindOneAsync(id);
        if (room == null)
        {
            throw new ApiException("Không tìm thấy phòng", StatusCode.NOT_FOUND);
        }

        room.RoomName = roomDto.RoomName ?? room.RoomName;
        room.RoomCode = roomDto.RoomCode ?? room.RoomCode;
        room.RoomTypeId = roomDto.RoomTypeId ?? room.RoomTypeId;
        room.Area = roomDto.Area ?? room.Area;
        room.Capacity = roomDto.Capacity ?? room.Capacity;
        room.FloorId = roomDto.FloorId ?? room.FloorId;
        room.PathRoom = roomDto.PathRoom ?? room.PathRoom;
        room.StatusId = roomDto.StatusId ?? room.StatusId;
        room.Description = roomDto.Description ?? room.Description;

        if (!await MainUnitOfWork.RoomRepository.UpdateAsync(room, AccountId, CurrentDate))
            throw new ApiException("Cập nhật thất bại", StatusCode.SERVER_ERROR);

        return ApiResponse.Success("Cập nhật thành công");
    }
    public async Task<ApiResponse> Delete(Guid id)
    {
        var existingRoom = await MainUnitOfWork.RoomRepository.FindOneAsync(id);

        if (existingRoom == null)
            throw new ApiException("Không tìm thấy phòng", StatusCode.NOT_FOUND);

        if (!await MainUnitOfWork.RoomRepository.DeleteAsync(existingRoom, AccountId, CurrentDate))
            throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);

        return ApiResponse.Success("Xóa thành công");
    }

    public async Task<ApiResponse> DeleteRooms(DeleteMutilDto deleteDto)
    {
        var roomDeleteds = await MainUnitOfWork.RoomRepository.FindAsync(
            new Expression<Func<Room, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => deleteDto.ListId!.Contains(x.Id)
            }, null);

        if (!await MainUnitOfWork.RoomRepository.DeleteAsync(roomDeleteds, AccountId, CurrentDate))
        {
            throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
        }
        return ApiResponse.Success("Xóa thành công");

    }
}