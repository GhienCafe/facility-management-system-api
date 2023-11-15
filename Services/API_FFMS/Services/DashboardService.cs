using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Services;

public interface IDashboardService : IBaseService
{
    public Task<ApiResponse<DashboardDto>> GetInformation();
    public Task<ApiResponse<IEnumerable<AssetDashBoardInformation>>> GetAssetStatusInformation();
    public Task<ApiResponse<IEnumerable<TaskDashBoardInformation>>> GetBaseTaskInformation();
    public Task<ApiResponse<IEnumerable<TaskBasedOnStatusDashboardDto>>> GetBaseTaskStatusInformation();
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

    public async Task<ApiResponse<IEnumerable<AssetDashBoardInformation>>> GetAssetStatusInformation()
    {
        var assetDashboardInfo = await MainUnitOfWork.AssetRepository.GetQuery()
            .GroupBy(a => new { a!.Status })
            .Select(g => new AssetDashBoardInformation
            {
                Status = g.Key.Status,
                StatusObj = g.Key.Status.GetValue(),
                Total = g.Count(),
                PercentPerTotal = ((double)g.Count() / MainUnitOfWork.AssetRepository.GetQuery().Count()) * 100
            })
            .ToListAsync();

        return ApiResponse<IEnumerable<AssetDashBoardInformation>>.Success(assetDashboardInfo);
    }

    public async Task<ApiResponse<IEnumerable<TaskDashBoardInformation>>> GetBaseTaskInformation()
    {
        var currentDate = DateTime.Now;
        var last12Months = currentDate.AddMonths(-12);

        var query = await MainUnitOfWork.TaskRepository.GetQueryAll()
            .Where(task => task!.CreatedAt >= last12Months)
            .GroupBy(task => task!.Type)
            .Select(group => new TaskDashBoardInformation
            {
                Type = group.Key,
                TypeObj = group.Key.GetValue(),
                TaskData = group
                    .GroupBy(t => new { t.CreatedAt.Year, t.CreatedAt.Month })
                    .Select(monthGroup => new TaskBasedOnMonthDto
                    {
                        Total = monthGroup.Count(),
                        Month = monthGroup.Key.Month,
                        Year = monthGroup.Key.Year
                    })
            })
            .ToListAsync();

        return ApiResponse<IEnumerable<TaskDashBoardInformation>>.Success(query);
    }

    public async Task<ApiResponse<IEnumerable<TaskBasedOnStatusDashboardDto>>> GetBaseTaskStatusInformation()
    {
        var rawData = await MainUnitOfWork.TaskRepository.GetQueryAll()
            .GroupBy(t => new { t!.Type, t.Status })
            .Select(g => new
            {
                Type = g.Key.Type,
                Status = g.Key.Status,
                TaskCount = g.Count()
            })
            .ToListAsync();

        var groupedData = rawData.GroupBy(x => x.Type)
            .Select(g => new TaskBasedOnStatusDashboardDto
            {
                Type = g.Key,
                TypeObj = g.Key.GetValue(),
                Data = g.Select(d => new TaskBasedOnStatusDto
                {
                    Count = d.TaskCount,
                    Status = d.Status
                })
            });

        var response = groupedData.ToList();
        return ApiResponse<IEnumerable<TaskBasedOnStatusDashboardDto>>.Success(response);
    }
}