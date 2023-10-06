using API_FFMS.Dtos;
using API_FFMS.Repositories;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;

namespace API_FFMS.Services;

public interface IMaintenanceService : IBaseService
{
    Task<ApiResponses<MaintenanceDto>> GetItems(MaintenanceQueryDto queryDto);
    Task<ApiResponse> CreateItem(MaintenanceCreateDto createDto);
}

public class MaintenanceService : BaseService, IMaintenanceService
{
    private readonly IMaintenanceRepository _maintenanceRepository;
    public MaintenanceService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository, IMaintenanceRepository maintenanceRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
        _maintenanceRepository = maintenanceRepository;
    }


    public Task<ApiResponses<MaintenanceDto>> GetItems(MaintenanceQueryDto queryDto)
    {
        throw new NotImplementedException();
    }

    public async Task<ApiResponse> CreateItem(MaintenanceCreateDto createDto)
    {
        var asset = await MainUnitOfWork.AssetRepository.FindOneAsync(createDto.AssetId);

        if (asset == null)
            throw new ApiException("Không cần tồn tại trang thiết bị", StatusCode.NOT_FOUND);
        
        if (asset.Status != AssetStatus.Operational)
            throw new ApiException("Trang thiết bị đang trong một yêu cầu khác", StatusCode.BAD_REQUEST);

        var maintenance = createDto.ProjectTo<MaintenanceCreateDto, Maintenance>();

        if (!await _maintenanceRepository.InsertMaintenance(maintenance, AccountId, CurrentDate))
            throw new ApiException("Tạo yêu cầu thất bại", StatusCode.SERVER_ERROR);

        return ApiResponse.Created("Gửi yêu cầu thành công");
    }
}