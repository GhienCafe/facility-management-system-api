using System.Linq.Expressions;
using System.Text;
using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Services;
public interface IFloorService : IBaseService
{
    Task<ApiResponses<FloorDto>> GetFloor(FloorQueryDto queryDto);
    Task<ApiResponse<FloorDetailDto>> GetFloor(Guid id);
    public Task<ApiResponse> Insert(FloorCreateDto addFloorDto);
    public Task<ApiResponse> Update(Guid id, FloorUpdateDto floorUpdate);
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
        var floorDataset = MainUnitOfWork.FloorRepository.GetQuery().Where(x => !x!.DeletedAt.HasValue);

        if (queryDto.FloorNumber != null)
        {
            floorDataset = floorDataset.Where(x => queryDto.FloorNumber == x!.FloorNumber);
        }

        var joinTables = from floor in floorDataset
            join building in MainUnitOfWork.BuildingRepository.GetQuery() on floor.BuildingId equals building.Id into
                buildingGroup
            from building in buildingGroup.DefaultIfEmpty()
            select new
            {
                Floor = floor,
                Building = building
            };

        var totalCount = joinTables.Count();

        var floors = await joinTables.Select(
            x => new FloorDto
            {
                FloorNumber = x.Floor.FloorNumber,
                Id = x.Floor.Id,
                FloorMap = x.Floor.FloorMap,
                BuildingId = x.Floor.BuildingId,
                CreatedAt = x.Floor.CreatedAt,
                EditedAt = x.Floor.EditedAt,
                CreatorId = x.Floor.CreatorId ?? Guid.Empty,
                EditorId = x.Floor.EditorId ?? Guid.Empty,
                Building = x.Building.ProjectTo<Building, BuildingBaseDto>()
            }).ToListAsync();

        floors = await _mapperRepository.MapCreator(floors);

        return ApiResponses<FloorDto>.Success(
            floors,
            totalCount,
            queryDto.PageSize,
            queryDto.Page,
            (int)Math.Ceiling(totalCount / (double)queryDto.PageSize)
        );
    }

    public async Task<ApiResponse<FloorDetailDto>> GetFloor(Guid id)
    {
        var floor = await MainUnitOfWork.FloorRepository.FindOneAsync<FloorDetailDto>(
            new Expression<Func<Floor, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.Id == id
            });

        if (floor == null)
            throw new ApiException("Không tìm thấy tầng", StatusCode.NOT_FOUND);

        floor.Building = await MainUnitOfWork.BuildingRepository.FindOneAsync<BuildingBaseDto>(
            new Expression<Func<Building, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.Id == floor.BuildingId
            });

        // Map CDC for the post
        floor = await _mapperRepository.MapCreator(floor);

        return ApiResponse<FloorDetailDto>.Success(floor);
    }

    public async Task<ApiResponse> Insert(FloorCreateDto floorDto)
    {
        var floor = floorDto.ProjectTo<FloorCreateDto, Floor>();
        
        if (floorDto.SvgFile != null)
        {
            var streamReader = new StreamReader(floorDto.SvgFile.OpenReadStream(), Encoding.UTF8);
            floor.FloorMap = await streamReader.ReadToEndAsync();
        }
        
        if (!await MainUnitOfWork.FloorRepository.InsertAsync(floor, AccountId, CurrentDate))
            throw new ApiException("Tạo mới thất bại", StatusCode.SERVER_ERROR);
        
        return ApiResponse.Created("Tạo mới thành công");
    }
    
    public async Task<ApiResponse> Update(Guid id, FloorUpdateDto floorDto)
    {
        var floor = await MainUnitOfWork.FloorRepository.FindOneAsync(id);
        if (floor == null )
            throw new ApiException("Không tìm thấy tầng", StatusCode.NOT_FOUND);

        var svgString = string.Empty;
        
        if (floorDto.SvgFile != null)
        {
            var streamReader = new StreamReader(floorDto.SvgFile.OpenReadStream(), Encoding.UTF8);
            svgString = await streamReader.ReadToEndAsync();
        }

        floor.FloorNumber = floorDto.FloorNumber ?? floor.FloorNumber;
        floor.FloorMap = !string.IsNullOrEmpty(svgString) ? svgString : floor.FloorMap;
        floor.BuildingId = floorDto.BuildingId ?? floor.BuildingId;
        
        if (!await MainUnitOfWork.FloorRepository.UpdateAsync(floor, AccountId, CurrentDate))
            throw new ApiException("Cập nhật thất bại", StatusCode.SERVER_ERROR);

        return ApiResponse.Success("Cập nhật thành công");
    }
    public async Task<ApiResponse> Delete(Guid id)
    {
        var existingFloor = await MainUnitOfWork.FloorRepository.FindOneAsync(id);
        if (existingFloor == null)
            throw new ApiException("Không tìm thấy tầng", StatusCode.NOT_FOUND);
        
        if (!await MainUnitOfWork.FloorRepository.DeleteAsync(existingFloor, AccountId, CurrentDate))
            throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);

        return ApiResponse.Success("Xóa thành công");
    }
}