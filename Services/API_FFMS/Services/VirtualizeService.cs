using System.Linq.Expressions;
using API_FFMS.Dtos;
using AppCore.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using MainData;
using MainData.Entities;
using MainData.Repositories;

namespace API_FFMS.Services;
public interface IVirtualizeService : IBaseService
{
    Task<ApiResponses<VirtualizeDto>> GetVirtualize(VirtualizeQueryDto queryDto);
}
public class VirtualizeService : BaseService, IVirtualizeService
{
    public VirtualizeService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor,
        IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public async Task<ApiResponses<VirtualizeDto>> GetVirtualize(VirtualizeQueryDto queryDto)
    {
        Expression<Func<Room, bool>>[] conditions = new Expression<Func<Room, bool>>[]
        {
            x => !x.DeletedAt.HasValue
        };

        var floorNumber = MainUnitOfWork.FloorsRepository.GetQuery()
            .Where(x => x.FloorNumber.Trim().ToLower() == queryDto.FloorNumber.Trim().ToLower()).SingleOrDefault();
        if (!string.IsNullOrEmpty(queryDto.FloorNumber))
        {
            conditions = conditions.Append(x => x.Floors.FloorNumber.Trim().ToLower() == queryDto.FloorNumber.Trim().ToLower()).ToArray();
        }
        else
        {
            conditions = conditions.Append(x => x.Floors.FloorNumber.Trim().ToLower() == "floor g").ToArray();
        }

        var response = await MainUnitOfWork.RoomRepository.FindResultAsync<VirtualizeDto>(conditions, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);
        return ApiResponses<VirtualizeDto>.Success(
            response.Items,
            response.TotalCount,
            queryDto.PageSize,
            queryDto.Skip(),
            (int)Math.Ceiling(response.TotalCount / (double)queryDto.PageSize)
        );
    }

}