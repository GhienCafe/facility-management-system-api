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
    Task<ApiResponses<BuildingsDto>> GetBuildings(BuildingQueryDto queryDto);
    Task<ApiResponse<BuildingDetailDto>> GetBuildings(Guid id);
    
    public Task<ApiResponse> Insert(BuildingCreateDto addBuildingDto);
    public Task<ApiResponse<BuildingDetailDto>> Update(Guid id, BuildingUpdateDto buildingUpdate);
    Task<ApiResponse> Delete(Guid id);

}
public class BuildingsService : BaseService, IBuildingsService
{
    public BuildingsService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor,
        IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public async Task<ApiResponses<BuildingsDto>> GetBuildings(BuildingQueryDto queryDto)
    {
        Expression<Func<Buildings, bool>>[] conditions = new Expression<Func<Buildings, bool>>[]
        {
            x => !x.DeletedAt.HasValue
        };

        if (string.IsNullOrEmpty(queryDto.BuildingName)==false)
        {
            conditions = conditions.Append(x => x.BuildingName.Trim().ToLower() == queryDto.BuildingName.Trim().ToLower()).ToArray();
        }

        var response = await MainUnitOfWork.BuildingsRepository.FindResultAsync<BuildingsDto>(conditions, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);
        return ApiResponses<BuildingsDto>.Success(
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
          new Expression<Func<Buildings, bool>>[]
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
        var buildings = buildingsDto.ProjectTo<BuildingCreateDto, Buildings>();
        bool response = await MainUnitOfWork.BuildingsRepository.InsertAsync(buildings, AccountId);
        
        if (response)
        {
            return ApiResponse<bool>.Success(true);
        }
        else
        {
            return (ApiResponse<bool>)ApiResponse.Failed();
        }
    }
    public async Task<ApiResponse<BuildingDetailDto>> Update(Guid id, BuildingUpdateDto buildingsDto)
    {
        var buildings = await MainUnitOfWork.BuildingsRepository.FindOneAsync(id);
        if (buildings==null)
        {
            throw new ApiException("Not found this buildings", StatusCode.NOT_FOUND);
        }
        if (!buildingsDto.BuildingName.IsBetweenLength(1, 50))
        {
            throw new ApiException("Can not create buildings when description is null or must length of characters 1-255", StatusCode.BAD_REQUEST);
        }
        var buildingsUpdate = buildingsDto.ProjectTo<BuildingUpdateDto, Buildings>();
        if (!await MainUnitOfWork.BuildingsRepository.UpdateAsync(buildingsUpdate, AccountId, CurrentDate))
            throw new ApiException("Can't not update", StatusCode.SERVER_ERROR);

        return await GetBuildings(id);
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