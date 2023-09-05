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
        Expression<Func<Building, bool>>[] conditions = new Expression<Func<Building, bool>>[]
        {
            x => !x.DeletedAt.HasValue
        };

        if (string.IsNullOrEmpty(queryDto.BuildingName)==false)
        {
            conditions = conditions.Append(x => x.BuildingName.Trim().ToLower().Contains(queryDto.BuildingName.Trim().ToLower())).ToArray();
        }
        if (string.IsNullOrEmpty(queryDto.CampusName)==false)
        {
            conditions = conditions.Append(x => x.Campus.CampusName.Trim().ToLower().Contains(queryDto.CampusName.Trim().ToLower())).ToArray();
        }
        var response = await MainUnitOfWork.BuildingsRepository.FindResultAsync<BuildingDto>(conditions, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);
        return ApiResponses<BuildingDto>.Success(
            response.Items,
            response.TotalCount,
            queryDto.PageSize,
            queryDto.Skip(),
            (int)Math.Ceiling(response.TotalCount / (double)queryDto.PageSize)
        );
    }

    public async Task<ApiResponse<BuildingDetailDto>> GetBuildings(Guid id)
    {
      var buildings = await MainUnitOfWork.BuildingsRepository.FindOneAsync<BuildingDetailDto>(
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

    public async Task<ApiResponse> Insert(BuildingCreateDto buildingsDto)
    {
        if (!buildingsDto.BuildingName.IsBetweenLength(1, 255))
        {
            throw new ApiException("Can not create buildings when address is null or must length of characters 1-255", StatusCode.ALREADY_EXISTS);
        }

        if (MainUnitOfWork.BuildingsRepository.GetQuery()
                .Where(x => x.BuildingName.Trim().ToLower().Contains(buildingsDto.BuildingName.Trim().ToLower()))
                .SingleOrDefault() != null)
        {
            throw new ApiException("Building name was used, please again!", StatusCode.BAD_REQUEST);

        }
        if (MainUnitOfWork.CampusRepository.GetQuery().SingleOrDefault(x => x.Id == buildingsDto.CampusId)==null)
        {
            throw new ApiException("Id cannot null", StatusCode.BAD_REQUEST);
        }
        var buildings = buildingsDto.ProjectTo<BuildingCreateDto, Building>();
        bool response = await MainUnitOfWork.BuildingsRepository.InsertAsync(buildings, AccountId, CurrentDate);
        
        if (response)
        {
            return ApiResponse<bool>.Success(true);
        }
        else
        {
            return (ApiResponse<bool>)ApiResponse.Failed();
        }
    }
    public async Task<ApiResponse> Update(Guid id, BuildingUpdateDto buildingsDto)
    {
        var buildings = await MainUnitOfWork.BuildingsRepository.FindOneAsync(id);
        if (buildings == null)
        {
            throw new ApiException("Not found this buildings", StatusCode.NOT_FOUND);
        }
        if (!buildingsDto.BuildingName.IsBetweenLength(1, 50))
        {
            throw new ApiException("Can not create buildings when description is null or must length of characters 1-255", StatusCode.BAD_REQUEST);
        }

        var checkCampusExisting = MainUnitOfWork.CampusRepository.GetQuery()
            .Where(x => x.Id == buildingsDto.CampusId).SingleOrDefault();
        if (checkCampusExisting == null)
        {
            throw new ApiException("Can not create buildings when campus is not found", StatusCode.BAD_REQUEST);
        }

        // Cập nhật các thuộc tính của thực thể hiện tại
        buildings.BuildingName = buildingsDto.BuildingName;
        buildings.CampusId = buildingsDto.CampusId;

        if (!await MainUnitOfWork.BuildingsRepository.UpdateAsync(buildings, AccountId, CurrentDate))
        {
            throw new ApiException("Can't not update", StatusCode.SERVER_ERROR);
        }

        return ApiResponse<bool>.Success(true);
    }

    public async Task<ApiResponse> Delete(Guid id)
    {
        var existingbuildings = await MainUnitOfWork.BuildingsRepository.FindOneAsync(id);
        if (existingbuildings == null)
            throw new ApiException("Not found this buildings", StatusCode.NOT_FOUND);
        if (!await MainUnitOfWork.BuildingsRepository.DeleteAsync(existingbuildings, AccountId, CurrentDate))
            throw new ApiException("Can't not delete", StatusCode.SERVER_ERROR);

        return ApiResponse.Success();
    }
}