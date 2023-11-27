using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using System.Linq.Expressions;
using API_FFMS.Repositories;

namespace API_FFMS.Services;
public interface IInventoryCheckConfigService : IBaseService
{
    Task<ApiResponse> CreateOrUpdateConfig(InventoryCheckConfigCreateDto createDto);
    Task<ApiResponse<InventoryCheckConfigDto>> GetConfig();
}

public class InventoryCheckConfigService : BaseService, IInventoryCheckConfigService
{
    private readonly IInventoryConfigRepository _configRepository;
    public InventoryCheckConfigService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository, IInventoryConfigRepository configRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
        _configRepository = configRepository;
    }

    public async Task<ApiResponse> CreateOrUpdateConfig(InventoryCheckConfigCreateDto createDto)
    {
        if (createDto.Id == null)
        {
            var count = await MainUnitOfWork.InventoryCheckConfigRepository.CountAsync(null);

            if (count > 0)
                throw new ApiException("Đã tồn tại cấu hình", StatusCode.BAD_REQUEST);
        }

        var inventories = createDto.ProjectTo<InventoryCheckConfigCreateDto, InventoryCheckConfig>();
        
        var checkDates = createDto.CheckDates?.ToList().ProjectTo<InventoryCheckDatesDto, InventoryDetailConfig>();

        if (!await _configRepository.UpdateInsertInventoryConfig(inventories, checkDates, AccountId, CurrentDate))
            throw new ApiException("Thao tác thất bại", StatusCode.SERVER_ERROR);
        
        return ApiResponse.Created("Cài đặt thành công");
    }

    public async Task<ApiResponse<InventoryCheckConfigDto>> GetConfig()
    {
        var inventory = (await MainUnitOfWork.InventoryCheckConfigRepository.FindOneAsync(null))?
            .ProjectTo<InventoryCheckConfig, InventoryCheckConfigDto>();

        if (inventory == null) return ApiResponse<InventoryCheckConfigDto>.Success(null);
        
        var dates = await MainUnitOfWork.InventoryDetailConfigRepository.FindAsync(
            new Expression<Func<InventoryDetailConfig, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.InventoryConfigId == inventory.Id
            }, null);
            
        inventory.CheckDates = dates.ToList()!.ProjectTo<InventoryDetailConfig, CheckDatesDto>();

        return ApiResponse<InventoryCheckConfigDto>.Success(inventory);
    }
}
