using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace API_FFMS.Services;

public interface IDashboardService : IBaseService
{
    public Task<ApiResponse<IEnumerable<AssetDashBoardInformation>>> GetAssetStatusInformation();
    public Task<ApiResponse<IEnumerable<TaskDashBoardInformation>>> GetBaseTaskInformation();
    public Task<ApiResponse<IEnumerable<TaskBasedOnStatusDashboardDto>>> GetBaseTaskStatusInformation();
    public Task<ApiResponse<TaskStatisticDto>> GetTaskStatistic();
    public Task<ApiResponse<AssetStatisticDto>> GetAssetStatistic(AssetStatisticQueryDto queryDto);
    public Task<ApiResponse<IEnumerable<ModelStatisticDto>>> GetModelStatistic();
}

public class DashboardService : BaseService, IDashboardService
{
    public DashboardService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor,
        IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {

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
            .Select(g =>
            {
                var totalCount = g.Sum(x => x.TaskCount);
                return new TaskBasedOnStatusDashboardDto
                {
                    Type = g.Key.GetValue(),
                    Total = totalCount,
                    Data = g.Select(d => new TaskBasedOnStatusDto
                    {
                        Count = d.TaskCount,
                        Status = d.Status.GetValue(),
                        Percent = totalCount > 0 ? (float)d.TaskCount / totalCount * 100 : 0
                    })
                };
            });

        var response = groupedData.ToList();
        return ApiResponse<IEnumerable<TaskBasedOnStatusDashboardDto>>.Success(response);
    }

    public async Task<ApiResponse<TaskStatisticDto>> GetTaskStatistic()
    {
        try
        {
            var taskQuery = MainUnitOfWork.TaskRepository.GetQueryAll().Where(x => x!.AssignedTo == AccountId);

            var response = new TaskStatisticDto
            {
                AssetCheckTask = await GetTaskStatisticDetails(taskQuery, RequestType.StatusCheck),
                RepairTask = await GetTaskStatisticDetails(taskQuery, RequestType.Repairation),
                ReplaceTask = await GetTaskStatisticDetails(taskQuery, RequestType.Replacement),
                TransportTask = await GetTaskStatisticDetails(taskQuery, RequestType.Transportation),
                MaintenanceTask = await GetTaskStatisticDetails(taskQuery, RequestType.Maintenance),
                InventoryCheckTask = await GetTaskStatisticDetails(taskQuery, RequestType.InventoryCheck)
            };

            return ApiResponse<TaskStatisticDto>.Success(response);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    private static async Task<TaskStatisticDetailDto?> GetTaskStatisticDetails(IQueryable<TaskView?> taskQuery, RequestType requestType)
    {
        var taskSelects = taskQuery.Select(x => new TaskView
        {
            Status = x!.Status,
            AssignedTo = x.AssignedTo,
            Type = x.Type
        });
        var filteredTasks = await taskSelects.Where(x => x!.Type == requestType).ToListAsync();

        if (filteredTasks.Any())
        {
            return new TaskStatisticDetailDto
            {
                Total = filteredTasks.Count,
                Process = filteredTasks.Count(t => t!.Status == RequestStatus.InProgress),
                Complete = filteredTasks.Count(t => t!.Status == RequestStatus.Done),
                Waiting = filteredTasks.Count(t => t!.Status == RequestStatus.NotStart),
                Reported = filteredTasks.Count(t => t!.Status == RequestStatus.Reported)
            };
        }
        return null;
    }

    public async Task<ApiResponse<AssetStatisticDto>> GetAssetStatistic(AssetStatisticQueryDto queryDto)
    {
        var assetQuery = MainUnitOfWork.AssetRepository.GetQuery().Include(x => x!.Type)
                         .Where(x => !x!.DeletedAt.HasValue);

        var transportQuery = MainUnitOfWork.TransportationRepository.GetQuery()
                                                                       .Where(x => x!.Status == RequestStatus.Reported ||
                                                                                   x.Status == RequestStatus.InProgress)
                                                                       .ToList();
        var unIdentAssetQuery = MainUnitOfWork.AssetRepository.GetQuery()
                                                         .Include(x => x!.Type)
                                                         .Where(x => !x!.DeletedAt.HasValue && x!.Type!.Unit == Unit.Quantity)
                                                         .ToList();

        var transportDetails = MainUnitOfWork.TransportationDetailRepository.GetQuery()
                            .Where(td => unIdentAssetQuery.Select(a => a!.Id).Contains((Guid)td!.AssetId!) &&
                                         transportQuery.Select(t => t!.Id).Contains((Guid)td.TransportationId!))
                            .Select(td => new
                            {
                                td!.Id
                            }).ToList();
        var transportDetailQuery = MainUnitOfWork.TransportationDetailRepository.GetQuery();

        //assetQuery = assetQuery.Where(x => x!.Type!.Unit == queryDto.Unit);

        if (queryDto.IsRent != null)
        {
            assetQuery = assetQuery.Where(x => x!.IsRented == queryDto.IsRent);
        }

        var assetStatistc = new AssetStatisticDto();
        if(queryDto.Unit == Unit.Individual)
        {
            assetQuery = assetQuery.Where(x => x!.Type!.Unit == queryDto.Unit);

            assetStatistc.TotalQuantity = assetQuery.Sum(x => x!.Quantity);
            assetStatistc.TotalOperational = assetQuery.Where(x => x!.Status == AssetStatus.Operational).Sum(x => x!.Quantity);
            assetStatistc.TotalNotUsed = assetQuery.Where(x => x!.Status == AssetStatus.Inactive).Sum(x => x!.Quantity);
            assetStatistc.TotalMaintenance = assetQuery.Where(x => x!.RequestStatus == RequestType.Maintenance).Sum(x => x!.Quantity);
            assetStatistc.TotalRepair = assetQuery.Where(x => x!.RequestStatus == RequestType.Repairation).Sum(x => x!.Quantity);
            assetStatistc.TotalTransportation = assetQuery.Where(x => x!.RequestStatus == RequestType.Transportation).Sum(x => x!.Quantity);
            assetStatistc.TotalReplacement = assetQuery.Where(x => x!.RequestStatus == RequestType.Replacement).Sum(x => x!.Quantity);
            assetStatistc.TotalNeedInspection = assetQuery.Where(x => x!.RequestStatus == RequestType.InventoryCheck).Sum(x => x!.Quantity);
        }
        else if (queryDto.Unit == Unit.Quantity)
        {
            assetStatistc.TotalQuantity = assetQuery.Where(x => x!.Type!.Unit == Unit.Quantity).Sum(x => x!.Quantity);
            assetStatistc.TotalOperational = assetQuery.Where(x => x!.Status == AssetStatus.Operational && x!.Type!.Unit == Unit.Quantity).Sum(x => x!.Quantity);
            assetStatistc.TotalNotUsed = assetQuery.Where(x => x!.Status == AssetStatus.Inactive && x!.Type!.Unit == Unit.Quantity).Sum(x => x!.Quantity);
            //assetStatistc.TotalMaintenance = assetQuery.Where(x => x!.RequestStatus == RequestType.Maintenance).Sum(x => x!.Quantity);
            //assetStatistc.TotalRepair = assetQuery.Where(x => x!.RequestStatus == RequestType.Repairation).Sum(x => x!.Quantity);
            assetStatistc.TotalTransportation = transportDetailQuery.Where(x => transportDetails.Select(td => td.Id).Contains(x.Id)).Sum(x => x!.Quantity);
            //assetStatistc.TotalReplacement = assetQuery.Where(x => x!.RequestStatus == RequestType.Replacement).Sum(x => x!.Quantity);
            //assetStatistc.TotalNeedInspection = assetQuery.Where(x => x!.RequestStatus == RequestType.InventoryCheck).Sum(x => x!.Quantity);
        }

        //var assetStatistc = new AssetStatisticDto
        //{
        //    TotalQuantity = assetQuery.Sum(x => x!.Quantity),
        //    TotalOperational = assetQuery.Where(x => x!.Status == AssetStatus.Operational).Sum(x => x!.Quantity),
        //    TotalNotUsed = assetQuery.Where(x => x!.Status == AssetStatus.Inactive).Sum(x => x!.Quantity),
        //    TotalMaintenance = assetQuery.Where(x => x!.RequestStatus == RequestType.Maintenance).Sum(x => x!.Quantity),
        //    TotalRepair = assetQuery.Where(x => x!.RequestStatus == RequestType.Repairation).Sum(x => x!.Quantity),
        //    TotalTransportation = assetQuery.Where(x => x!.RequestStatus == RequestType.Transportation).Sum(x => x!.Quantity),
        //    TotalReplacement = assetQuery.Where(x => x!.RequestStatus == RequestType.Replacement).Sum(x => x!.Quantity),
        //    TotalNeedInspection = assetQuery.Where(x => x!.RequestStatus == RequestType.InventoryCheck).Sum(x => x!.Quantity),
        //};

        return ApiResponse<AssetStatisticDto>.Success(assetStatistc);
    }

    public async Task<ApiResponse<IEnumerable<ModelStatisticDto>>> GetModelStatistic()
    {
        var modelQuery = MainUnitOfWork.ModelRepository.GetQuery();

        var taskQuery = MainUnitOfWork.TaskRepository.GetQueryAll()
                                                     .Where(x => x!.Type == RequestType.Repairation &&
                                                                 x.Status != RequestStatus.NotStart);

        var assetQuery = MainUnitOfWork.AssetRepository.GetQuery()
                                                       .Include(a => a!.Type)
                                                       .Where(a => a!.Type!.Unit == Unit.Individual);

        var join = from model in modelQuery
                   join asset in assetQuery on model.Id equals asset.ModelId into groupAssets
                   from asset in groupAssets.DefaultIfEmpty()
                   join task in taskQuery on asset.Id equals task.AssetId into groupTasks
                   from task in groupTasks.DefaultIfEmpty()
                   group new { model, asset, task } by new
                   {
                       model.Id,
                       model.ModelName
                   } into groupData
                   select new ModelStatisticDto
                   {
                       ModelName = groupData.Key.ModelName,
                       TotalRepair = groupData.Count(item => item.task != null)
                   };

        var result = await join.ToListAsync();

        return ApiResponse<IEnumerable<ModelStatisticDto>>.Success(result);
    }
}