using System.Linq.Expressions;
using System.Text;
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
        var floors = await MainUnitOfWork.FloorRepository.FindResultAsync<FloorDto>(new Expression<Func<Floor, bool>>[]
        {
             x => !x.DeletedAt.HasValue,
             x => queryDto.FloorNumber == null || queryDto.FloorNumber == x.FloorNumber
        }, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);

        floors.Items = await _mapperRepository.MapCreator(floors.Items.ToList());

        return ApiResponses<FloorDto>.Success(
            floors.Items,
            floors.TotalCount,
            queryDto.PageSize,
            queryDto.Skip(),
            (int)Math.Ceiling(floors.TotalCount / (double)queryDto.PageSize)
        );
    }

    public async Task<ApiResponse<FloorDetailDto>> GetFloor(Guid id)
    {
        var floors = await MainUnitOfWork.FloorRepository.FindOneAsync<FloorDetailDto>(
            new Expression<Func<Floor, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.Id == id
            });

        if (floors == null)
            throw new ApiException("Not found", StatusCode.NOT_FOUND);

        // Map CDC for the post
        floors = await _mapperRepository.MapCreator(floors);

        return ApiResponse<FloorDetailDto>.Success(floors);
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
            throw new ApiException("Insert fail!", StatusCode.SERVER_ERROR);
        
        return ApiResponse.Success();
    }
    
    public async Task<ApiResponse> Update(Guid id, FloorUpdateDto floorDto)
    {
        var floor = await MainUnitOfWork.FloorRepository.FindOneAsync(id);
        if (floor == null )
            throw new ApiException("Not found this floors", StatusCode.NOT_FOUND);

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
            throw new ApiException("Can't not update", StatusCode.SERVER_ERROR);

        return ApiResponse.Success();
    }
    public async Task<ApiResponse> Delete(Guid id)
    {
        var existingFloor = await MainUnitOfWork.FloorRepository.FindOneAsync(id);
        if (existingFloor == null)
            throw new ApiException("Not found this floors", StatusCode.NOT_FOUND);
        
        if (!await MainUnitOfWork.FloorRepository.DeleteAsync(existingFloor, AccountId, CurrentDate))
            throw new ApiException("Can't not delete", StatusCode.SERVER_ERROR);

        return ApiResponse.Success();
    }
}