﻿using System.Linq.Expressions;
using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;

namespace API_FFMS.Services;
public interface IRoomService : IBaseService
{
    Task<ApiResponses<RoomDto>> GetRoom(RoomQueryDto queryDto);
    Task<ApiResponse<RoomDetailDto>> GetRoom(Guid id);
    public Task<ApiResponse> Insert(RoomCreateDto addRoomDto);
    public Task<ApiResponse> Update(Guid id, RoomUpdateDto floorUpdate);
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
        var roomQuerySet = MainUnitOfWork.RoomRepository.GetQuery().Where(x => !x!.DeletedAt.HasValue);

        if (queryDto.RoomTypeId != null)
        {
            roomQuerySet = roomQuerySet.Where(x => x!.RoomTypeId == queryDto.RoomTypeId);
        }

        if (!string.IsNullOrEmpty(queryDto.RoomCode))
        {
            roomQuerySet = roomQuerySet.Where(x => x!.RoomCode.Equals(queryDto.RoomCode));
        }

        if (!string.IsNullOrEmpty(queryDto.Status))
        {
            var statusId = MainUnitOfWork.RoomStatusRepository.GetQuery().Where(
                    x => !x!.DeletedAt.HasValue && x.StatusName.ToLower().Equals(queryDto.Status.Trim().ToLower()))
                .Select(x => x!.Id);

            roomQuerySet = roomQuerySet.Where(x => x!.StatusId.Equals(statusId));
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

        if (!string.IsNullOrEmpty(queryDto.RoomName))
        {
            roomQuerySet = roomQuerySet.Where(x => x!.RoomName!.ToLower().Contains(queryDto.RoomName.Trim().ToLower()));
        }

        if (queryDto.FloorId != null)
        {
            roomQuerySet = roomQuerySet.Where(x => x!.FloorId == queryDto.FloorId);
        }

        var response = from room in roomQuerySet
                       join status in MainUnitOfWork.RoomStatusRepository.GetQuery() on room.StatusId equals status.Id
                       join type in MainUnitOfWork.RoomTypeRepository.GetQuery() on room.RoomTypeId equals type.Id
                       select new
                       {
                           Room = room,
                           Status = status,
                           Type = type
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
                RoomTypeId = x.Room.RoomTypeId,
                CreatedAt = x.Room.CreatedAt,
                EditedAt = x.Room.EditedAt,
                RoomCode = x.Room.RoomCode,
                StatusId = x.Room.StatusId,
                CreatorId = x.Room.CreatorId ?? Guid.Empty,
                EditorId = x.Room.EditorId ?? Guid.Empty,
                Status = x.Status.ProjectTo<RoomStatus, RoomStatusDto>(),
                RoomType = x.Type.ProjectTo<RoomType, RoomTypeDto>()
            }
            ).ToListAsync();

        rooms = await _mapperRepository.MapCreator(rooms);

        // Return data
        return ApiResponses<RoomDto>.Success(
            rooms,
            totalCount,
            queryDto.PageSize,
            queryDto.Skip(),
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
            throw new ApiException("Not found this room", StatusCode.NOT_FOUND);

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
            throw new ApiException("Insert fail", StatusCode.SERVER_ERROR);

        return ApiResponse.Created("Created successfully");
    }
    public async Task<ApiResponse> Update(Guid id, RoomUpdateDto roomDto)
    {
        var room = await MainUnitOfWork.RoomRepository.FindOneAsync(id);
        if (room == null)
        {
            throw new ApiException("Not found this room", StatusCode.NOT_FOUND);
        }

        room.RoomName = roomDto.RoomName ?? room.RoomName;
        room.RoomCode = roomDto.RoomCode ?? room.RoomCode;
        room.RoomTypeId = roomDto.RoomTypeId ?? room.RoomTypeId;
        room.Area = roomDto.Area ?? room.Area;
        room.Capacity = roomDto.Capacity ?? room.Capacity;
        room.FloorId = roomDto.FloorId ?? room.FloorId;
        room.PathRoom = roomDto.PathRoom ?? room.PathRoom;
        room.StatusId = roomDto.StatusId ?? room.StatusId;

        if (!await MainUnitOfWork.RoomRepository.UpdateAsync(room, AccountId, CurrentDate))
            throw new ApiException("Update fail", StatusCode.SERVER_ERROR);

        return ApiResponse.Success();
    }
    public async Task<ApiResponse> Delete(Guid id)
    {
        var existingRoom = await MainUnitOfWork.RoomRepository.FindOneAsync(id);

        if (existingRoom == null)
            throw new ApiException("Not found this room", StatusCode.NOT_FOUND);

        if (!await MainUnitOfWork.RoomRepository.DeleteAsync(existingRoom, AccountId, CurrentDate))
            throw new ApiException("Delete fail", StatusCode.SERVER_ERROR);

        return ApiResponse.Success();
    }
}