using System.Linq.Expressions;
using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Services;
public interface IBuildingsService : IBaseService
{
    Task<ApiResponses<BuildingDto>> GetBuildings(BuildingQueryDto queryDto);
    Task<ApiResponse<BuildingDetailDto>> GetBuilding(Guid id);
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
        var buildingDataset = MainUnitOfWork.BuildingRepository.GetQuery().Where(x => !x!.DeletedAt.HasValue);

        if (!string.IsNullOrEmpty(queryDto.BuildingName))
        {
            buildingDataset = buildingDataset.Where(x =>
                x!.BuildingName!.ToLower().Contains(queryDto.BuildingName.Trim().ToLower()));
        }
        
        if (!string.IsNullOrEmpty(queryDto.BuildingCode))
        {
            buildingDataset = buildingDataset.Where(x =>
                x!.BuildingCode!.ToLower().Contains(queryDto.BuildingCode.Trim().ToLower()));
        }

        var joinTables = from building in buildingDataset
            join campus in MainUnitOfWork.CampusRepository.GetQuery() on building.CampusId equals campus.Id into campusGroup
            from campus in campusGroup.DefaultIfEmpty()
            select new
            {
                Building = building,
                Campus = campus
            };
        
        var totalCount = joinTables.Count();

        joinTables = joinTables.Skip(queryDto.Skip()).Take(queryDto.PageSize);

        var buildings = await joinTables.Select(
            x => new BuildingDto
            {
                Id = x.Building.Id,
                CampusId = x.Building.CampusId,
                BuildingCode = x.Building.BuildingCode,
                BuildingName = x.Building.BuildingName,
                CreatedAt = x.Building.CreatedAt,
                EditedAt = x.Building.EditedAt,
                CreatorId = x.Building.CreatorId ?? Guid.Empty,
                EditorId = x.Building.EditorId ?? Guid.Empty,
                Campus = x.Campus.ProjectTo<Campus, CampusDto>()
            }).ToListAsync();

        buildings= await _mapperRepository.MapCreator(buildings);
        
        return ApiResponses<BuildingDto>.Success(
            buildings,
            totalCount,
            queryDto.PageSize,
            queryDto.Page,
            (int)Math.Ceiling(totalCount / (double)queryDto.PageSize)
        );
    }

    public async Task<ApiResponse<BuildingDetailDto>> GetBuilding(Guid id)
    {
      var building = await MainUnitOfWork.BuildingRepository.FindOneAsync<BuildingDetailDto>(
          new Expression<Func<Building, bool>>[]
          {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id
          });

      if (building == null)
          throw new ApiException("Không tìm thấy tòa", StatusCode.NOT_FOUND);

      building.Campus = await MainUnitOfWork.CampusRepository.FindOneAsync<CampusDto>(
          new Expression<Func<Campus, bool>>[]
          {
              x => !x.DeletedAt.HasValue,
              x => x.Id == building.CampusId
          });
      
      // Map CDC for the post
      building = await _mapperRepository.MapCreator(building);

      return ApiResponse<BuildingDetailDto>.Success(building);
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