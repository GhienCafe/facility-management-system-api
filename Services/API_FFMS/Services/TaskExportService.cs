using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Services;

public interface ITaskExportService : IBaseService
{
    public Task<ExportFile> ExportAsset();
    public Task<ExportFile> ExportTaskUltra();
}

public class TaskExportService : BaseService, ITaskExportService
{
    public TaskExportService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor,
                             IMapperRepository mapperRepository)
                             : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public async Task<ExportFile> ExportAsset()
    {
        var assets = MainUnitOfWork.AssetRepository.GetQuery()
                      .Include(x => x!.Type).ThenInclude(t => t!.Category)
                      .Include(x => x!.Model)
                      .Include(x => x!.RoomAssets).ThenInclude(ra => ra.Room);

        var assetExports = assets!.Select(x => new AssetExportDtos
        {
            AssetName = x!.AssetName,
            AssetCode = x.AssetCode,
            IsMovable = IsTrueOrFalse(x.IsMovable),
            Status = x.Status.GetValue().DisplayName,
            ManufacturingYear = x.ManufacturingYear,
            Category = x.Type!.Category!.CategoryName,
            CurrentRoom = x.RoomAssets!.Where(ra => ra.AssetId == x.Id && ra.ToDate == null)
                                       .Select(ra => ra.Room!.RoomName)
                                       .FirstOrDefault(),
            SerialNumber = x.SerialNumber,
            Quantity = x.Quantity,
            Description = x.Description,
            LastMaintenanceTime = x.LastMaintenanceTime,
            LastCheckedDate = x.LastCheckedDate,
            StartDateOfUse = x.StartDateOfUse,
            IsRented = IsTrueOrFalse(x.IsRented),
            TypeName = x.Type!.TypeName,
            ModelName = x.Model!.ModelName
        }).ToList();

        var streamFile = ExportHelperList<AssetExportDtos>.Export(
            assetExports,
            "Asset",
            "Asset export",
            6);
        return new ExportFile
        {
            FileName = $"Asset Export",
            Stream = streamFile
        };
    }

    private static string IsTrueOrFalse(bool? value)
    {
        if (value == true)
        {
            return "Có";
        }
        else
        {
            return "Không";
        }
    }

    private static string IsInternal(bool? value)
    {
        if (value == true)
        {
            return "Thực hiện nội bộ";
        }
        else
        {
            return "Thực hiện bên ngoài";
        }
    }

    public async Task<ExportFile> ExportTaskUltra()
    {
        var itemsList = new List<List<TaskExportDto>>();
        var sheetNames = new List<string>();
        var titles = new List<string>();
        //REPAIRATION
        var repairQuery = MainUnitOfWork.RepairRepository.GetQuery()
                              .Where(x => !x!.DeletedAt.HasValue);
        var joinRepairTable = from repair in repairQuery
                                join user in MainUnitOfWork.UserRepository.GetQuery() on repair.AssignedTo equals user.Id
                                join asset in MainUnitOfWork.AssetRepository.GetQuery() on repair.AssetId equals asset.Id
                                join assetType in MainUnitOfWork.AssetTypeRepository.GetQuery() on asset.TypeId equals assetType.Id into assetTypeGroup
                                from assetType in assetTypeGroup.DefaultIfEmpty()
                                join category in MainUnitOfWork.CategoryRepository.GetQuery() on assetType.CategoryId equals category.Id
                                select new
                                {
                                    Repairation = repair,
                                    Asset = asset,
                                    User = user,
                                    AssetType = assetType,
                                    Category = category
                                };
        var repairations = await joinRepairTable.Select(x => new TaskExportDto
        {
            RequestCode = x.Repairation.RequestCode,
            RequestDate = x.Repairation.RequestDate,
            CompletionDate = x.Repairation.CompletionDate.ToString() ?? "Chưa hoàn thành",
            Priority = x.Repairation.Priority.GetValue().DisplayName,
            AssetName = x.Asset.AssetName,
            Category = x.Category.CategoryName,
            TypeName = x.AssetType.TypeName,
            AssignedTo = x.User.Fullname,
            Status = x.Repairation.Status!.GetValue().DisplayName,
            Description = x.Repairation.Description,
            Checkin = x.Repairation.Checkin,
            Checkout = x.Repairation.Checkout,
            Notes = x.Repairation.Notes,
            Result = x.Repairation.Result,
            IsInternal = IsInternal(x.Repairation.IsInternal)
        }).OrderByDescending(x => x.RequestDate).ToListAsync();
        if(repairations != null)
        {
            itemsList.Add(repairations);
            sheetNames.Add("Sửa chữa");
            titles.Add("Lịch sử sửa chữa");
        }

        //MAINTENANCE
        var maintenQuery = MainUnitOfWork.MaintenanceRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);

        var joinMaintenTable = from maintenance in maintenQuery
                                    join user in MainUnitOfWork.UserRepository.GetQuery() on maintenance.AssignedTo equals user.Id
                                    join asset in MainUnitOfWork.AssetRepository.GetQuery() on maintenance.AssetId equals asset.Id into assetGroup
                               from asset in assetGroup.DefaultIfEmpty()
                                    join assetType in MainUnitOfWork.AssetTypeRepository.GetQuery() on asset.TypeId equals assetType.Id into assetTypeGroup
                               from assetType in assetTypeGroup.DefaultIfEmpty()
                                    join category in MainUnitOfWork.CategoryRepository.GetQuery() on assetType.CategoryId equals category.Id
                                    select new
                                    {
                                        Maintenance = maintenance,
                                        User = user,
                                        Asset = asset,
                                        AssetType = assetType,
                                        Category = category
                                    };
        var maintenances = await joinMaintenTable.Select(x => new TaskExportDto
        {
            RequestCode = x.Maintenance.RequestCode,
            RequestDate = x.Maintenance.RequestDate,
            CompletionDate = x.Maintenance.CompletionDate.ToString() ?? "Chưa hoàn thành",
            Priority = x.Maintenance.Priority.GetValue().DisplayName,
            AssetName = x.Asset.AssetName,
            Category = x.Category.CategoryName,
            TypeName = x.AssetType.TypeName,
            AssignedTo = x.User.Fullname,
            Status = x.Maintenance.Status!.GetValue().DisplayName,
            Description = x.Maintenance.Description,
            Notes = x.Maintenance.Notes,
            Checkin = x.Maintenance.Checkin,
            Checkout = x.Maintenance.Checkout,
            Result = x.Maintenance.Result,
            IsInternal = IsInternal(x.Maintenance.IsInternal)
        }).OrderByDescending(x => x.RequestDate).ToListAsync();
        if (maintenances != null)
        {
            itemsList.Add(maintenances);
            sheetNames.Add("Bảo trì");
            titles.Add("Lịch sử bảo trì");
        }

        //ASSET CHECK
        var assetCheckQuery = MainUnitOfWork.AssetCheckRepository.GetQuery()
                              .Where(x => !x!.DeletedAt.HasValue);

        var joinAssetCheckTable = from assetCheck in assetCheckQuery
                                    join user in MainUnitOfWork.UserRepository.GetQuery() on assetCheck.AssignedTo equals user.Id
                                    join asset in MainUnitOfWork.AssetRepository.GetQuery() on assetCheck.AssetId equals asset.Id into assetGroup
                                  from asset in assetGroup.DefaultIfEmpty()
                                    join assetType in MainUnitOfWork.AssetTypeRepository.GetQuery() on asset.TypeId equals assetType.Id into assetTypeGroup
                                  from assetType in assetTypeGroup.DefaultIfEmpty()
                                    join category in MainUnitOfWork.CategoryRepository.GetQuery() on assetType.CategoryId equals category.Id
                                    join location in MainUnitOfWork.RoomRepository.GetQuery() on assetCheck.RoomId equals location.Id
                                  select new
                                  {
                                      AssetCheck = assetCheck,
                                      User = user,
                                      Asset = asset,
                                      Location = location,
                                      AssetType = assetType,
                                      Category = category
                                  };
        var assetChecks = await joinAssetCheckTable.Select(x => new TaskExportDto
        {
            RequestCode = x.AssetCheck.RequestCode,
            RequestDate = x.AssetCheck.RequestDate,
            CompletionDate = x.AssetCheck.CompletionDate.ToString() ?? "Chưa hoàn thành",
            AssetName = x.Asset.AssetName,
            Priority = x.AssetCheck.Priority.GetValue().DisplayName,
            Category = x.Category.CategoryName,
            TypeName = x.AssetType.TypeName,
            Location = x.Location.RoomName,
            AssignedTo = x.User.Fullname,
            Status = x.AssetCheck.Status.GetValue().DisplayName,
            Description = x.AssetCheck.Description,
            Notes = x.AssetCheck.Notes,
            Checkin = x.AssetCheck.Checkin,
            Checkout = x.AssetCheck.Checkout,
            Result = x.AssetCheck.Result,
            IsInternal = IsInternal(x.AssetCheck.IsInternal)
        }).OrderByDescending(x => x.RequestDate).ToListAsync();
        if (assetChecks != null)
        {
            itemsList.Add(assetChecks);
            sheetNames.Add("Kiểm tra");
            titles.Add("Lịch sử kiểm tra");
        }

        //REPLACEMENT
        var replaceQuery = MainUnitOfWork.ReplacementRepository.GetQuery()
                               .Where(x => !x!.DeletedAt.HasValue);

        var joinReplaceTable = from replace in replaceQuery
                                 join asset in MainUnitOfWork.AssetRepository.GetQuery() on replace.AssetId equals asset.Id
                                 join user in MainUnitOfWork.UserRepository.GetQuery() on replace.AssignedTo equals user.Id
                                 join newAsset in MainUnitOfWork.AssetRepository.GetQuery() on replace.NewAssetId equals newAsset.Id
                                 join assetType in MainUnitOfWork.AssetTypeRepository.GetQuery() on asset.TypeId equals assetType.Id into assetTypeGroup
                               from assetType in assetTypeGroup.DefaultIfEmpty()
                                 join category in MainUnitOfWork.CategoryRepository.GetQuery() on assetType.CategoryId equals category.Id
                                 select new
                                 {
                                     Replacement = replace,
                                     User = user,
                                     Asset = asset,
                                     NewAsset = newAsset,
                                     AssetType = assetType,
                                     Category = category
                                 };
        var replacements = await joinReplaceTable.Select(x => new TaskExportDto
        {
            RequestCode = x.Replacement.RequestCode,
            RequestDate = x.Replacement.RequestDate,
            CompletionDate = x.Replacement.CompletionDate.ToString() ?? "Chưa hoàn thành",
            Priority = x.Replacement.Priority.GetValue().DisplayName,
            AssetName = x.Asset.AssetName,
            Category = x.Category.CategoryName,
            TypeName = x.AssetType.TypeName,
            NewAsset = x.NewAsset.AssetName,
            AssignedTo = x.User.Fullname,
            Status = x.Replacement.Status!.GetValue().DisplayName,
            Description = x.Replacement.Description,
            Notes = x.Replacement.Notes,
            Checkin = x.Replacement.Checkin,
            Checkout = x.Replacement.Checkout,
            Result = x.Replacement.Result,
            IsInternal = IsInternal(x.Replacement.IsInternal)
        }).OrderByDescending(x => x.RequestDate).ToListAsync();
        if (replacements != null)
        {
            itemsList.Add(replacements);
            sheetNames.Add("Thay thế");
            titles.Add("Lịch sử thay thế");
        }

        //TRANSPORTATION
        var transportQuery = MainUnitOfWork.TransportationRepository.GetQuery()
                                    .Include(x => x!.User)
                                 .Where(x => !x!.DeletedAt.HasValue);

        var transportDetails = MainUnitOfWork.TransportationDetailRepository.GetQuery();

        var transportations = await transportQuery.Select(x => new TaskExportDto
        {
            RequestCode = x!.RequestCode,
            RequestDate = x.RequestDate,
            CompletionDate = x.CompletionDate.ToString() ?? "Chưa hoàn thành",
            AssignedTo = x.User!.Fullname,
            Status = x.Status.GetValue().DisplayName,
            Priority = x.Priority.GetValue().DisplayName,
            Description = x.Description,
            Notes = x.Notes,
            Checkin = x.Checkin,
            Checkout= x.Checkout,
            Result = x.Result,
            IsInternal = IsInternal(x.IsInternal),
            Assets = MainUnitOfWork.TransportationDetailRepository.GetQuery()
                            .Where(ta => ta!.TransportationId == x.Id)
                            .Select(ta => new AssetTransportExportDto
                            {
                                AssetName = ta!.Asset!.AssetName + " Số lượng(cái): " + ta.Quantity
                            }).ToList()
        }).OrderByDescending(x => x.RequestDate).ToListAsync();

        if (transportations != null)
        {
            itemsList.Add(transportations);
            sheetNames.Add("Vận chuyển");
            titles.Add("Lịch sử vận chuyển");
        }

        var streamFile = ExportHelperList<TaskExportDto>.ExportUltra(
            itemsList,
            sheetNames,
            titles,
            6);
        return new ExportFile
        {
            FileName = $"Danh sách phiếu yêu cầu",
            Stream = streamFile
        };
    }
}