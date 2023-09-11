using System.Linq.Expressions;
using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Services;

public interface ICampusService : IBaseService
{
    Task<ApiResponses<CampusDto>> GetCampus(CampusQueryDto queryDto);
    Task<ApiResponse<CampusDetailDto>> GetCampus(Guid id);
    
    public Task<ApiResponse> Insert(CampusCreateDto addCampusDto);
    Task<ApiResponse> Update(Guid id, CampusUpdateDto campusUpdateDto);
    Task<ApiResponse> Delete(Guid id);

}
public class CampusService :BaseService,ICampusService
{
    public CampusService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public async Task<ApiResponses<CampusDto>> GetCampus(CampusQueryDto queryDto)
    {
        Expression<Func<Campus, bool>>[] conditions = new Expression<Func<Campus, bool>>[]
        {
            x => !x.DeletedAt.HasValue
        };

        if (string.IsNullOrEmpty(queryDto.CampusName)==false)
        {
            conditions = conditions.Append(x => x.CampusName.Trim().ToLower().Contains(queryDto.CampusName.Trim().ToLower())).ToArray();
        }

        var response = await MainUnitOfWork.CampusRepository.FindResultAsync<CampusDto>(conditions, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);
        return ApiResponses<CampusDto>.Success(
            response.Items,
            response.TotalCount,
            queryDto.PageSize,
            queryDto.Skip(),
            (int)Math.Ceiling(response.TotalCount / (double)queryDto.PageSize)
        );
    }

    public async Task<ApiResponse<CampusDetailDto>> GetCampus(Guid id)
    {
      var campus = await MainUnitOfWork.CampusRepository.FindOneAsync<CampusDetailDto>(
          new Expression<Func<Campus, bool>>[]
          {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id
          });

      if (campus == null)
        throw new ApiException("Not found this campus", StatusCode.NOT_FOUND);

      campus.TotalBuilding = MainUnitOfWork.BuildingsRepository.GetQuery().Count(x => !x!.DeletedAt.HasValue
          && x.CampusId == campus.Id);

      // Map CDC for the post
      campus = await _mapperRepository.MapCreator(campus);

      return ApiResponse<CampusDetailDto>.Success(campus);
    }

    public async Task<ApiResponse> Insert(CampusCreateDto campusDto)
    {
        if (!campusDto.Description.IsBetweenLength(1, 255))
        {
            return ApiResponse.Failed("Description is null or must have a length between 1 and 255 characters", StatusCode.BAD_REQUEST);
        }

        if (!campusDto.Address.IsBetweenLength(1, 255))
        {
            return ApiResponse.Failed("Address is null or must have a length between 1 and 255 characters", StatusCode.BAD_REQUEST);
        }

        if (!campusDto.CampusName.IsMinLength(3))
        {
            return ApiResponse.Failed("Campus name is null or must have a length greater than 3 characters", StatusCode.BAD_REQUEST);
        }

        bool isValid = campusDto.Telephone.IsPhoneNumberOrNonEmpty(11);

        if (!isValid)
        {
            return ApiResponse.Failed("Telephone is null or not valid", StatusCode.BAD_REQUEST);
        }

        var checkDuplication = await MainUnitOfWork.CampusRepository.GetQuery()
            .Where(x => x.CampusName == campusDto.CampusName)
            .SingleOrDefaultAsync();

        if (checkDuplication != null)
        {
            return ApiResponse.Failed("Campus name is duplicated or not valid", StatusCode.BAD_REQUEST);
        }

        var campus = campusDto.ProjectTo<CampusCreateDto, Campus>();
        bool response = await MainUnitOfWork.CampusRepository.InsertAsync(campus, AccountId, DateTime.Today);

        if (response)
        {
            return ApiResponse.Success("Campus created successfully");
        }
        else
        {
            return ApiResponse.Failed("Failed to create campus", StatusCode.SERVER_ERROR);
        }
    }

    public async Task<ApiResponse> Update(Guid id, CampusUpdateDto campusDto)
    {
        var campus = await MainUnitOfWork.CampusRepository.FindOneAsync(id);
        if (campus==null)
        {
            throw new ApiException("Not found this campus", StatusCode.NOT_FOUND);
        }
        if (!campusDto.Description.IsBetweenLength(1, 255))
        {
            throw new ApiException("Can not create campus when description is null or must length of characters 1-255", StatusCode.BAD_REQUEST);
        }
        if (!campusDto.Address.IsBetweenLength(1, 255))
        {
            throw new ApiException("Can not create campus when address is null or must length of characters 1-255", StatusCode.BAD_REQUEST);
        }
        if (!campusDto.CampusName.IsMinLength(3))
        {
            // Thực hiện xử lý khi CampusName không đạt độ dài tối thiểu
            throw new ApiException("Can not create campus when name is null or lenght must to > 3 characters");
        }
        bool isValid = campusDto.Telephone.IsPhoneNumberOrNonEmpty(11);

        if (isValid == false)
        {
            throw new ApiException("Can't not create campus when description is null or not valid", StatusCode.BAD_REQUEST);
        }

        campus.CampusName = campusDto.CampusName;
        campus.Address = campusDto.Address;
        campus.Telephone = campusDto.Telephone;
        campus.Description = campusDto.Description;
        bool response = await MainUnitOfWork.CampusRepository.UpdateAsync(campus, AccountId, DateTime.Today);

        if (response)
        {
            return ApiResponse.Success("Campus Updated successfully");
        }
        else
        {
            return ApiResponse.Failed("Failed to create campus", StatusCode.SERVER_ERROR);
        }
    }
    public async Task<ApiResponse> Delete(Guid id)
    {
        var existingCampus = await MainUnitOfWork.CampusRepository.FindOneAsync(id);
        if (existingCampus == null)
            throw new ApiException("Not found this campus", StatusCode.NOT_FOUND);
        if (!await MainUnitOfWork.CampusRepository.DeleteAsync(existingCampus, AccountId, CurrentDate))
            throw new ApiException("Can't not delete", StatusCode.SERVER_ERROR);

        return ApiResponse.Success();
    }
}