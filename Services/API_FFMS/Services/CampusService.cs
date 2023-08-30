using System.Linq.Expressions;
using API_FFMS.Dtos;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;

namespace API_FFMS.Services;

public interface ICampusService : IBaseService
{
    Task<ApiResponses<CampusDto>> GetCampus(CampusQueryDto queryDto);
}
public class CampusService :BaseService,ICampusService
{
    public CampusService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public async Task<ApiResponses<CampusDto>> GetCampus(CampusQueryDto queryDto)
    {
        var response = await MainUnitOfWork.CampusRepository.FindResultAsync<CampusDto>(new Expression<Func<Campus, bool>>[]
        {
            x=>!x.DeletedAt.HasValue,
            x=>string.IsNullOrEmpty(queryDto.CampusName) || x.CampusName.Trim().ToLower()== queryDto.CampusName.Trim().ToLower()
        },queryDto.OrderBy,queryDto.Skip(),queryDto.PageSize);
        return ApiResponses<CampusDto>.Success(
            response.Items,
            response.TotalCount,
            queryDto.PageSize,
            queryDto.Skip(),
            (int)Math.Ceiling(response.TotalCount / (double)queryDto.PageSize)
        );
    }
}