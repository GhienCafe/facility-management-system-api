using API_FFMS.Dtos;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;

namespace API_FFMS.Services;

public interface IDashboardService : IBaseService
{
    public Task<ApiResponse<DashboardDto>> GetInformation();
}

public class DashboardService : BaseService, IDashboardService
{
    public DashboardService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor,
        IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {

    }

    public async Task<ApiResponse<DashboardDto>> GetInformation()
    {
        var assetDataSet = MainUnitOfWork.AssetRepository.GetQuery().Where(x => !x!.DeletedAt.HasValue);
        var typeAssetDataSet = MainUnitOfWork.AssetTypeRepository.GetQuery().Where(x => !x!.DeletedAt.HasValue);
        var assetTypeData = typeAssetDataSet.Select(assetType => new AssetTypeDashboardDto
        {
            TypeId = assetType!.Id,
            TypeName = assetType.TypeName,
            Quantity = assetDataSet
                .Where(asset => asset!.TypeId == assetType.Id)
                .Sum(asset => asset!.Quantity),
            InUsed  = assetDataSet
                .Where(asset => asset!.TypeId == assetType.Id && asset.Status == AssetStatus.Operational)
                .Sum(asset => asset!.Quantity),
            NotUsed  = assetDataSet
                .Where(asset => asset!.TypeId == assetType.Id && asset.Status == AssetStatus.Inactive)
                .Sum(asset => asset!.Quantity),
            Maintenance = assetDataSet
                .Count(asset => asset!.TypeId == assetType.Id && asset.Status == AssetStatus.Maintenance),
            Repair = assetDataSet
                .Count(asset => asset!.TypeId == assetType.Id && asset.Status == AssetStatus.Repair),
            Transportation = assetDataSet
                .Count(asset => asset!.TypeId == assetType.Id && asset.Status == AssetStatus.Transportation),
            Replacement = assetDataSet
                .Count(asset => asset!.TypeId == assetType.Id && asset.Status == AssetStatus.Replacement),
            NeedInspection = assetDataSet
                .Count(asset => asset!.TypeId == assetType.Id && asset.Status == AssetStatus.NeedInspection),
        }).ToList();
        var dashboardDto = new DashboardDto
        {
            TotalQuantity = assetTypeData.Sum(asset => asset.Quantity),
            TotalInUsed = assetTypeData.Sum(asset => asset.InUsed),
            TotalNotUsed = assetTypeData.Sum(asset => asset.NotUsed),
            TotalMaintenance = assetTypeData.Sum(asset => asset.Maintenance),
            TotalRepair = assetTypeData.Sum(asset => asset.Repair),
            TotalTransportation = assetTypeData.Sum(asset => asset.Transportation),
            TotalReplacement = assetTypeData.Sum(asset => asset.Replacement),
            TotalNeedInspection = assetTypeData.Sum(asset => asset.NeedInspection),
            AssetType = assetTypeData,
        };
        await Task.CompletedTask;
        return ApiResponse<DashboardDto>.Success(dashboardDto);
    }
}