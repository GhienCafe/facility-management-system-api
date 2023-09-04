using System.Linq.Expressions;
using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace API_FFMS.Services;
public interface IFloorService : IBaseService
{
    Task<ApiResponses<FloorDto>> GetFloor(FloorQueryDto queryDto);
    Task<ApiResponse<FloorDetailDto>> GetFloor(Guid id);
    
    public Task<ApiResponse> Insert(FloorCreateDto addFloorDto);
    public Task<ApiResponse<FloorDetailDto>> Update(Guid id, FloorUpdateDto floorUpdate);
    Task<ApiResponse> Delete(Guid id);

}
public class FloorService : BaseService, IFloorService
{
    public FloorService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor,
        IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public async Task<ApiResponses<FloorDto>> GetFloor(FloorQueryDto queryDto)
    {
        Expression<Func<Floors, bool>>[] conditions = new Expression<Func<Floors, bool>>[]
        {
            x => !x.DeletedAt.HasValue
        };

        if (string.IsNullOrEmpty(queryDto.FloorNumber)==false)
        {
            conditions = conditions.Append(x => x.FloorNumber.Trim().ToLower() == queryDto.FloorNumber.Trim().ToLower()).ToArray();
        }
        if (string.IsNullOrEmpty(queryDto.BuildingName)==false)
        {
            conditions = conditions.Append(x => x.Buildings.BuildingName.Trim().ToLower().Contains(queryDto.BuildingName.Trim().ToLower())).ToArray();
        }
        var response = await MainUnitOfWork.FloorsRepository.FindResultAsync<FloorDto>(conditions, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);
        return ApiResponses<FloorDto>.Success(
            response.Items,
            response.TotalCount,
            queryDto.PageSize,
            queryDto.Skip(),
            (int)Math.Ceiling(response.TotalCount / (double)queryDto.PageSize)
        );
    }

    public async Task<ApiResponse<FloorDetailDto>> GetFloor(Guid id)
    {
        var floors = await MainUnitOfWork.FloorsRepository.FindOneAsync<FloorDetailDto>(
            new Expression<Func<Floors, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.Id == id
            });

        if (floors == null)
            throw new ApiException("Not found this floors", StatusCode.NOT_FOUND);

        // Map CDC for the post
        floors = await _mapperRepository.MapCreator(floors);

        return ApiResponse<FloorDetailDto>.Success(floors);
    }

    public async Task<ApiResponse> Insert(FloorCreateDto floorDto)
    {
        if (!floorDto.FloorNumber.IsBetweenLength(1, 255))
        {
            throw new ApiException("Can not create floors when address is null or must length of characters 1-255", StatusCode.ALREADY_EXISTS);
        }

        var existingFloor = MainUnitOfWork.FloorsRepository.GetQuery()
            .Where(x => x.FloorNumber.Trim().ToLower() == floorDto.FloorNumber.Trim().ToLower())
            .SingleOrDefault();

        if (existingFloor != null)
        {
            throw new ApiException("Floor name was used, please again!", StatusCode.BAD_REQUEST);
        }

        if (MainUnitOfWork.BuildingsRepository.GetQuery().SingleOrDefault(x => x.Id == floorDto.BuildingId)==null)
        {
            throw new ApiException("Id cannot null", StatusCode.BAD_REQUEST);
        }
        var floors = floorDto.ProjectTo<FloorCreateDto, Floors>();
        bool response = await MainUnitOfWork.FloorsRepository.InsertAsync(floors, AccountId);
        
        if (response)
        {
            return ApiResponse<bool>.Success(true);
        }
        else
        {
            return (ApiResponse<bool>)ApiResponse.Failed();
        }
    }
    public async Task<ApiResponse<FloorDetailDto>> Update(Guid id, FloorUpdateDto floorsDto)
    {
        var floors = await MainUnitOfWork.FloorsRepository.FindOneAsync(id);
        if (floors==null)
        {
            throw new ApiException("Not found this floors", StatusCode.NOT_FOUND);
        }
        if (!floorsDto.FloorNumber.IsBetweenLength(1, 50))
        {
            throw new ApiException("Can not create floors when description is null or must length of characters 1-255", StatusCode.BAD_REQUEST);
        }
        var totalAreaRoom = MainUnitOfWork.RoomsRepository.GetQuery()
            .Where(room => room.FloorId == id) // Lọc các phòng thuộc tầng cần kiểm tra
            .Sum(room => room.Area); // Tính tổng diện tích của các phòng
        if (totalAreaRoom >= floorsDto.Area)
        {
            throw new ApiException("Can not create floor when area not valid with total area of room", StatusCode.BAD_REQUEST);
        }
        var floorsUpdate = floorsDto.ProjectTo<FloorUpdateDto, Floors>();
        if (!await MainUnitOfWork.FloorsRepository.UpdateAsync(floorsUpdate, AccountId, CurrentDate))
            throw new ApiException("Can't not update", StatusCode.SERVER_ERROR);

        return await GetFloor(id);
    }
    public async Task<ApiResponse> Delete(Guid id)
    {
        var existingfloors = await MainUnitOfWork.FloorsRepository.FindOneAsync(id);
        if (existingfloors == null)
            throw new ApiException("Not found this floors", StatusCode.NOT_FOUND);
        if (!await MainUnitOfWork.FloorsRepository.DeleteAsync(existingfloors, AccountId, CurrentDate))
            throw new ApiException("Can't not delete", StatusCode.SERVER_ERROR);

        return ApiResponse.Success();
    }
}