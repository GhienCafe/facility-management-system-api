using System.Linq.Expressions;
using API_FFMS.Dtos;
using AppCore.Models;
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
        Expression<Func<Rooms, bool>>[] conditions = new Expression<Func<Rooms, bool>>[]
        {
            x => !x.DeletedAt.HasValue
        };

        if (string.IsNullOrEmpty(queryDto.FloorNumber)==false)
        {
            conditions = conditions.Append(x => x.RoomNumber.Trim().ToLower() == queryDto.FloorNumber.Trim().ToLower()).ToArray();
        }
        else
        {
            conditions = conditions.Append(x => x.Floors.FloorNumber.Trim().ToLower().Contains("floor g")).ToArray();
        }
        
        var response = await MainUnitOfWork.RoomsRepository.FindResultAsync<VirtualizeDto>(conditions, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);
        return ApiResponses<VirtualizeDto>.Success(
            response.Items,
            response.TotalCount,
            queryDto.PageSize,
            queryDto.Skip(),
            (int)Math.Ceiling(response.TotalCount / (double)queryDto.PageSize)
        );
    }
}