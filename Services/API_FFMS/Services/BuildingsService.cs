using System.Linq.Expressions;
using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace API_FFMS.Services;
public interface IBuildingsService : IBaseService
{
    Task<ApiResponses<BuildingDto>> GetBuildings(BuildingQueryDto queryDto);
    Task<ApiResponse<BuildingDetailDto>> GetBuildings(Guid id);
    public Task<ApiResponse> Insert(BuildingCreateDto addBuildingDto);
    public Task<ApiResponse> Update(Guid id, BuildingUpdateDto buildingUpdate);
    Task<ApiResponse> Delete(Guid id);

}
public class BuildingsService : BaseService, IBuildingsService
{
    public BuildingsService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor,
        IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public async Task<ApiResponses<BuildingDto>> GetBuildings(BuildingQueryDto queryDto)
    {
        var buildings = await MainUnitOfWork.BuildingRepository.FindResultAsync<BuildingDto>(
            new Expression<Func<Building, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => string.IsNullOrEmpty(queryDto.BuildingName) ||
                     x.BuildingName!.ToLower().Contains(queryDto.BuildingName.Trim().ToLower()),
                x => string.IsNullOrEmpty(queryDto.BuildingCode) ||
                     x.BuildingCode!.ToLower().Contains(queryDto.BuildingCode.Trim().ToLower())
            }, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);

        buildings.Items = await _mapperRepository.MapCreator(buildings.Items.ToList());
        
        return ApiResponses<BuildingDto>.Success(
            buildings.Items,
            buildings.TotalCount,
            queryDto.PageSize,
            queryDto.Skip(),
            (int)Math.Ceiling(buildings.TotalCount / (double)queryDto.PageSize)
        );
    }

    public async Task<ApiResponse<BuildingDetailDto>> GetBuildings(Guid id)
    {
      var buildings = await MainUnitOfWork.BuildingRepository.FindOneAsync<BuildingDetailDto>(
          new Expression<Func<Building, bool>>[]
          {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id
          });

      if (buildings == null)
          throw new ApiException("Not found this buildings", StatusCode.NOT_FOUND);

      // Map CDC for the post
      buildings = await _mapperRepository.MapCreator(buildings);

      return ApiResponse<BuildingDetailDto>.Success(buildings);
    }

    public async Task<ApiResponse> Insert(BuildingCreateDto buildingDto)
    {
        var building = buildingDto.ProjectTo<BuildingCreateDto, Building>();

        if (!await MainUnitOfWork.BuildingRepository.InsertAsync(building, AccountId, CurrentDate))
            throw new ApiException("Insert fail", StatusCode.SERVER_ERROR);
        
        return ApiResponse.Created("Create successfully!");
    }
    public async Task<ApiResponse> Update(Guid id, BuildingUpdateDto buildingDto)
    {
        var building = await MainUnitOfWork.BuildingRepository.FindOneAsync(id);
        if (building == null)
        {
            throw new ApiException("Not found this buildings", StatusCode.NOT_FOUND);
        }
        
        building.BuildingName = buildingDto.BuildingName ?? building.BuildingName;
        building.CampusId = buildingDto.CampusId ?? building.CampusId;
        building.BuildingCode = buildingDto.BuildingCode ?? building.BuildingCode;

        if (!await MainUnitOfWork.BuildingRepository.UpdateAsync(building, AccountId, CurrentDate))
            throw new ApiException("Update failed!", StatusCode.SERVER_ERROR);

        return ApiResponse.Success("Update successfully");
    }

    public async Task<ApiResponse> Delete(Guid id)
    {
        var existingBuilding = await MainUnitOfWork.BuildingRepository.FindOneAsync(id);
        
        if (existingBuilding == null)
            throw new ApiException("Not found this buildings", StatusCode.NOT_FOUND);
        
        if (!await MainUnitOfWork.BuildingRepository.DeleteAsync(existingBuilding, AccountId, CurrentDate))
            throw new ApiException("Can't not delete", StatusCode.SERVER_ERROR);

        return ApiResponse.Success("Delete successfully!");
    }
}