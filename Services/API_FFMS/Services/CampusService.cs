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
        var keyword = queryDto.Keyword?.Trim().ToLower();
        var campusesQueryable = MainUnitOfWork.CampusRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);

        if (!string.IsNullOrEmpty(keyword))
        {
            campusesQueryable = campusesQueryable.Where(x => x!.CampusName!.ToLower().Contains(keyword)
                                                             || x.CampusCode!.ToLower().Contains(keyword)
                                                             || x.Address!.ToLower().Contains(keyword)
                                                             || x.Description!.ToLower().Contains(keyword)
                                                             || x.Telephone!.Contains(keyword));
        }

        var totalCount = campusesQueryable.Count();

        var campuses = await campusesQueryable.Select(x => new CampusDto
        {
            Id = x!.Id,
            Address = x.Address,
            CampusName = x.CampusName,
            Description = x.Description,
            CampusCode = x.CampusCode,
            Telephone = x.Telephone,
            LogoUrl = x.LogoUrl,
            WebsiteUrl = x.WebsiteUrl,
            CreatedAt = x.CreatedAt,
            EditedAt = x.EditedAt,
            CreatorId = x.CreatorId ?? Guid.Empty,
            EditorId = x.EditorId ?? Guid.Empty
        }).ToListAsync();

        campuses = await _mapperRepository.MapCreator(campuses);
        
        return ApiResponses<CampusDto>.Success(
            campuses,
            totalCount,
            queryDto.PageSize,
            queryDto.Page,
            (int)Math.Ceiling(totalCount / (double)queryDto.PageSize)
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
        throw new ApiException("Không tìm thấy campus", StatusCode.NOT_FOUND);

      campus.TotalBuilding = MainUnitOfWork.BuildingRepository.GetQuery().Count(x => !x!.DeletedAt.HasValue
          && x.CampusId == campus.Id);

      // Map CDC for the post
      campus = await _mapperRepository.MapCreator(campus);

      return ApiResponse<CampusDetailDto>.Success(campus);
    }

    public async Task<ApiResponse> Insert(CampusCreateDto campusDto)
    {
        var campus = campusDto.ProjectTo<CampusCreateDto, Campus>();

        if (!await MainUnitOfWork.CampusRepository.InsertAsync(campus, AccountId, CurrentDate))
            throw new ApiException("Thêm thất bại", StatusCode.SERVER_ERROR);
        
        return ApiResponse.Created("Thêm thành công");
    }

    public async Task<ApiResponse> Update(Guid id, CampusUpdateDto campusDto)
    {
        var campus = await MainUnitOfWork.CampusRepository.FindOneAsync(id);

        if (campus == null)
            throw new ApiException("Không tìm thấy campus", StatusCode.NOT_FOUND);
        
        campus.CampusName = campusDto.CampusName ?? campus.CampusName;
        campus.CampusCode = campusDto.CampusCode ?? campus.CampusCode;
        campus.Address = campusDto.Address ?? campus.Address;
        campus.Telephone = campusDto.Telephone ?? campus.Address;
        campus.Description = campusDto.Description ?? campus.Description;

        if (!await MainUnitOfWork.CampusRepository.UpdateAsync(campus, AccountId, CurrentDate))
            throw new ApiException("Thêm thất bại", StatusCode.SERVER_ERROR);
        
        return ApiResponse.Success("Cập nhật thất bại");
    }
    public async Task<ApiResponse> Delete(Guid id)
    {
        var existingCampus = await MainUnitOfWork.CampusRepository.FindOneAsync(id);
        if (existingCampus == null)
            throw new ApiException("Không tìm thấy campus", StatusCode.NOT_FOUND);
        if (!await MainUnitOfWork.CampusRepository.DeleteAsync(existingCampus, AccountId, CurrentDate))
            throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);

        return ApiResponse.Success("Xóa thành công");
    }
}