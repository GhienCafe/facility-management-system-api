using API_FFMS.Dtos;
using API_FFMS.Repositories;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace API_FFMS.Services;

public interface IMaintenanceService : IBaseService
{
    Task<ApiResponses<MaintenanceDto>> GetItems(MaintenanceQueryDto queryDto);
    Task<ApiResponse<MaintenanceDto>> GetItem(Guid id);
    Task<ApiResponse> CreateItem(MaintenanceCreateDto createDto);
    Task<ApiResponse> CreateItems(List<MaintenanceCreateDto> createDtos);
    Task<ApiResponse> UpdateItem(Guid id, MaintenanceUpdateDto updateDto);
    Task<ApiResponse> DeleteItem(Guid id);
    Task<ApiResponse> DeleteItems(DeleteMutilDto deleteDto);
    Task<ApiResponse> UpdateStatus(Guid id, BaseUpdateStatusDto updateStatusDto);
}

public class MaintenanceService : BaseService, IMaintenanceService
{
    private readonly IMaintenanceRepository _maintenanceRepository;
    public MaintenanceService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository, IMaintenanceRepository maintenanceRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
        _maintenanceRepository = maintenanceRepository;
    }

    public async Task<ApiResponses<MaintenanceDto>> GetItems(MaintenanceQueryDto queryDto)
    {
        var keyword = queryDto.Keyword?.Trim().ToLower();
        var maintenanceQueryable = MainUnitOfWork.MaintenanceRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);

        if (queryDto.AssetId != null)
        {
            maintenanceQueryable = maintenanceQueryable.Where(x => x!.AssetId == queryDto.AssetId);
        }

        if (queryDto.IsInternal != null)
        {
            maintenanceQueryable = maintenanceQueryable.Where(x => x!.IsInternal == queryDto.IsInternal);
        }

        if (queryDto.AssignedTo != null)
        {
            maintenanceQueryable = maintenanceQueryable.Where(x => x!.AssignedTo == queryDto.AssignedTo);
        }

        if (queryDto.Status != null)
        {
            maintenanceQueryable = maintenanceQueryable.Where(x => x!.Status == queryDto.Status);
        }

        if (queryDto.Priority != null)
        {
            maintenanceQueryable = maintenanceQueryable.Where(x => x!.Priority == queryDto.Priority);
        }

        if (!string.IsNullOrEmpty(keyword))
        {
            maintenanceQueryable = maintenanceQueryable.Where(x => x!.Description!.ToLower().Contains(keyword)
                                                               || x.Notes!.ToLower().Contains(keyword) ||
                                                               x.RequestCode.ToLower().Contains(keyword));
        }
        
        maintenanceQueryable = maintenanceQueryable.OrderByDescending(x => x!.CreatedAt);

        var assetQueryable = MainUnitOfWork.AssetRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);

        var userQueryable = MainUnitOfWork.UserRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);

        var assetTypeQueryable = MainUnitOfWork.AssetTypeRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);

        var categoryQueryable = MainUnitOfWork.CategoryRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);

        var joinTables = from maintenance in maintenanceQueryable
                         join user in userQueryable on maintenance.AssignedTo equals user.Id into userGroup
                         from user in userGroup.DefaultIfEmpty()
                         join asset in assetQueryable on maintenance.AssetId equals asset.Id into assetGroup
                         from asset in assetGroup.DefaultIfEmpty()
                         join assetType in assetTypeQueryable on asset.TypeId equals assetType.Id into assetTypeGroup
                         from assetType in assetTypeGroup.DefaultIfEmpty()
                         join category in categoryQueryable on assetType.CategoryId equals category.Id into categoryGroup
                         from category in categoryGroup.DefaultIfEmpty()
                         select new
                         {
                             Maintenance = maintenance,
                             User = user,
                             Asset = asset,
                             AssetType = assetType,
                             Category = category
                         };

        var totalCount = await joinTables.CountAsync();

        joinTables = joinTables.Skip(queryDto.Skip()).Take(queryDto.PageSize);

        var items = await joinTables.Select(x => new MaintenanceDto
        {
            Id = x.Maintenance.Id,
            Status = x.Maintenance.Status,
            AssetId = x.Maintenance.AssetId,
            Notes = x.Maintenance.Notes,
            Description = x.Maintenance.Description,
            IsInternal = x.Maintenance.IsInternal,
            AssignedTo = x.Maintenance.AssignedTo,
            RequestCode = x.Maintenance.RequestCode,
            CompletionDate = x.Maintenance.CompletionDate,
            Checkin = x.Maintenance.Checkin,
            Checkout = x.Maintenance.Checkout,
            Result = x.Maintenance.Result,
            RequestDate = x.Maintenance.RequestDate,
            CategoryId = x.Maintenance.CategoryId,
            AssetTypeId = x.Maintenance.AssetTypeId,
            StatusObj = x.Maintenance.Status!.GetValue(),
            CreatedAt = x.Maintenance.CreatedAt,
            Priority = x.Maintenance.Priority,
            EditedAt = x.Maintenance.EditedAt,
            CreatorId = x.Maintenance.CreatorId ?? Guid.Empty,
            EditorId = x.Maintenance.EditorId ?? Guid.Empty,
            User = x.User.ProjectTo<User, UserBaseDto>(),
            Asset = x.Asset.ProjectTo<Asset, AssetBaseDto>(),
            AssetType = x.AssetType.ProjectTo<AssetType, AssetTypeDto>(),
            Category = x.Category.ProjectTo<Category, CategoryDto>(),
            PriorityObj = x.Maintenance.Priority.GetValue()
        }).ToListAsync();

        foreach (var item in items)
        {
            if (item.User != null)
            {
                item.User.StatusObj = item.User.Status?.GetValue();
                item.User.RoleObj = item.User.Role?.GetValue();
            }

            if (item.Asset != null)
            {
                item.Asset.StatusObj = item.Asset.Status?.GetValue();
            }
        }

        items = await _mapperRepository.MapCreator(items);

        return ApiResponses<MaintenanceDto>.Success(
            items,
            totalCount,
            queryDto.PageSize,
            queryDto.Page,
            (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
    }

    public async Task<ApiResponse<MaintenanceDto>> GetItem(Guid id)
    {
        var maintenanceDto = MainUnitOfWork.MaintenanceRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue && x.Id == id);

        if (maintenanceDto == null)
            throw new ApiException("Không tìm thấy nội dung", StatusCode.NOT_FOUND);

        var assetQueryable = MainUnitOfWork.AssetRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);

        var userQueryable = MainUnitOfWork.UserRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);

        var assetTypeQueryable = MainUnitOfWork.AssetTypeRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);

        var categoryQueryable = MainUnitOfWork.CategoryRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);

        var joinTables = from maintenance in maintenanceDto
                         join user in userQueryable on maintenance.AssignedTo equals user.Id into userGroup
                         from user in userGroup.DefaultIfEmpty()
                         join asset in assetQueryable on maintenance.AssetId equals asset.Id into assetGroup
                         from asset in assetGroup.DefaultIfEmpty()
                         join assetType in assetTypeQueryable on asset.TypeId equals assetType.Id into assetTypeGroup
                         from assetType in assetTypeGroup.DefaultIfEmpty()
                         join category in categoryQueryable on assetType.CategoryId equals category.Id into categoryGroup
                         from category in categoryGroup.DefaultIfEmpty()
                         select new
                         {
                             Maintenance = maintenance,
                             User = user,
                             Asset = asset,
                             AssetType = assetType,
                             Category = category
                         };

        var item = await joinTables.Select(x => new MaintenanceDto
        {
            Id = x.Maintenance.Id,
            Status = x.Maintenance.Status,
            AssetId = x.Maintenance.AssetId,
            Notes = x.Maintenance.Notes,
            Description = x.Maintenance.Description,
            IsInternal = x.Maintenance.IsInternal,
            AssignedTo = x.Maintenance.AssignedTo,
            RequestCode = x.Maintenance.RequestCode,
            CompletionDate = x.Maintenance.CompletionDate,
            RequestDate = x.Maintenance.RequestDate,
            Result = x.Maintenance.Result,
            Checkout = x.Maintenance.Checkout,
            Checkin = x.Maintenance.Checkout,
            CategoryId = x.Maintenance.CategoryId,
            AssetTypeId = x.Maintenance.AssetTypeId,
            Priority = x.Maintenance.Priority,
            StatusObj = x.Maintenance.Status!.GetValue(),
            CreatedAt = x.Maintenance.CreatedAt,
            EditedAt = x.Maintenance.EditedAt,
            CreatorId = x.Maintenance.CreatorId ?? Guid.Empty,
            EditorId = x.Maintenance.EditorId ?? Guid.Empty,
            User = x.User.ProjectTo<User, UserBaseDto>(),
            Asset = x.Asset.ProjectTo<Asset, AssetBaseDto>(),
            AssetType = x.AssetType.ProjectTo<AssetType, AssetTypeDto>(),
            Category = x.Category.ProjectTo<Category, CategoryDto>(),
            PriorityObj = x.Maintenance.Priority.GetValue()
        }).FirstOrDefaultAsync();

        //Related file
        var relatedMediaFiles = await MainUnitOfWork.MediaFileRepository.GetQuery()
            .Where(m => m!.ItemId == id && !m.IsReported).FirstOrDefaultAsync();

        item.RelatedFiles = JsonConvert.DeserializeObject<List<MediaFileDetailDto>>(relatedMediaFiles.Uri);

        var reports = await MainUnitOfWork.MediaFileRepository.GetQuery()
            .Where(m => m!.ItemId == id && m.IsReported).OrderByDescending(x => x!.CreatedAt).ToListAsync();
        
        item.Reports = new List<MediaFileDto>();
        foreach (var report in reports)
        {
            // Deserialize the URI string back into a List<string>
            var uriList = JsonConvert.DeserializeObject<List<string>>(report.Uri);
            
            item.Reports.Add(new MediaFileDto
            {
                ItemId = report.ItemId,
                Uri = uriList,
                FileType = report.FileType,
                Content = report.Content,
                IsReject = report.IsReject,
                RejectReason = report.RejectReason
            });
        }

        return ApiResponse<MaintenanceDto>.Success(item);
    }

    public async Task<ApiResponse> CreateItem(MaintenanceCreateDto createDto)
    {
        var asset = await MainUnitOfWork.AssetRepository.FindOneAsync(createDto.AssetId);
        //var assetType = await MainUnitOfWork.AssetTypeRepository.FindOneAsync((Guid)createDto.AssetTypeId!);

        if (asset == null)
            throw new ApiException("Không cần tồn tại trang thiết bị", StatusCode.NOT_FOUND);

        if (asset.RequestStatus != RequestType.Operational)
            throw new ApiException("Trang thiết bị đang trong một yêu cầu khác", StatusCode.BAD_REQUEST);

        var maintenance = createDto.ProjectTo<MaintenanceCreateDto, Maintenance>();
        maintenance.RequestCode = GenerateRequestCode();

        // For storing json in column
        var mediaFiles = new List<Report>();
        if (createDto.RelatedFiles != null)
        {
            var listUrisJson = JsonConvert.SerializeObject(createDto.RelatedFiles);
            var report = new Report
            {
                FileName = string.Empty,
                Uri = listUrisJson,
                Content = string.Empty,
                FileType = FileType.File,
                ItemId = maintenance.Id,
                IsVerified = false,
                IsReported = false,
            };
        
            mediaFiles.Add(report);
        }

        if (!await _maintenanceRepository.InsertMaintenance(maintenance, mediaFiles, AccountId, CurrentDate))
            throw new ApiException("Tạo yêu cầu thất bại", StatusCode.SERVER_ERROR);

        return ApiResponse.Created("Gửi yêu cầu thành công");
    }

    public async Task<ApiResponse> UpdateItem(Guid id, MaintenanceUpdateDto updateDto)
    {
        var maintenance = await MainUnitOfWork.MaintenanceRepository.FindOneAsync(id);

        if (maintenance == null)
            throw new ApiException("Không tìm thấy nội dung", StatusCode.NOT_FOUND);

        if (maintenance.Status != RequestStatus.NotStart)
            throw new ApiException("Chỉ được cập nhật yêu cầu đang có trạng thái chưa bắt đầu", StatusCode.BAD_REQUEST);

        maintenance.Description = updateDto.Description ?? maintenance.Description;
        maintenance.Notes = updateDto.Notes ?? maintenance.Notes;
        maintenance.CategoryId = updateDto.CategoryId ?? maintenance.CategoryId;
        maintenance.IsInternal = updateDto.IsInternal ?? maintenance.IsInternal;
        maintenance.AssetTypeId = updateDto.AssetTypeId ?? maintenance.AssetTypeId;
        maintenance.AssignedTo = updateDto.AssignedTo ?? maintenance.AssignedTo;
        maintenance.Priority = updateDto.Priority ?? maintenance.Priority;

        var mediaFileQuery = MainUnitOfWork.MediaFileRepository.GetQuery().Where(x => x!.ItemId == id).ToList();

        var newMediaFile = updateDto.RelatedFiles.Select(dto => new Report
        {
            FileName = dto.FileName,
            Uri = dto.Uri,
            CreatedAt = CurrentDate,
            CreatorId = AccountId,
            ItemId = id,
            FileType = FileType.File
        }).ToList() ?? new List<Report>();

        var additionMediaFiles = newMediaFile.Except(mediaFileQuery).ToList();
        var removalMediaFiles = mediaFileQuery.Except(newMediaFile).ToList();

        if (!await _maintenanceRepository.UpdateMaintenance(maintenance, additionMediaFiles, removalMediaFiles, AccountId, CurrentDate))
            throw new ApiException("Cập nhật thất bại", StatusCode.SERVER_ERROR);

        return ApiResponse.Success("Cập nhật thành công");
    }

    public async Task<ApiResponse> CreateItems(List<MaintenanceCreateDto> createDtos)
    {
        var assets = await MainUnitOfWork.AssetRepository.FindAsync(
                new Expression<Func<Asset, bool>>[]
                {
                    x => !x!.DeletedAt.HasValue,
                    x => createDtos.Select(dto => dto.AssetId).Contains(x.Id)
                }, null);

        foreach(var asset in assets)
        {
            if (asset == null)
                throw new ApiException("Không cần tồn tại trang thiết bị", StatusCode.NOT_FOUND);

            if (asset.RequestStatus != RequestType.Operational)
                throw new ApiException($"Thiết bị {asset.AssetCode} đang trong một yêu cầu khác", StatusCode.BAD_REQUEST);

            var checkExist = await MainUnitOfWork.MaintenanceRepository.FindAsync(
                    new Expression<Func<Maintenance, bool>>[]
                    {
                            x => !x.DeletedAt.HasValue,
                            x => x.AssetId == asset.Id, 
                            x => x.Status != RequestStatus.Done
                    }, null);
            if (checkExist.Any())
            {
                throw new ApiException($"Đã có yêu cầu bảo trì cho thiết bị {asset.AssetCode}", StatusCode.ALREADY_EXISTS);
            }
        }

        var maintenances = new List<Maintenance>();
        var relatedFiles = new List<Report>();
        foreach (var create in createDtos)
        {
            var maintenance = create.ProjectTo<MaintenanceCreateDto, Maintenance>();
            maintenance.Id = Guid.NewGuid();
            if (create.RelatedFiles != null)
            {
                foreach (var file in create.RelatedFiles)
                {
                    var relatedFile = new Report
                    {
                        Id = Guid.NewGuid(),
                        FileName = file.FileName ?? "",
                        Uri = file.Uri ?? "",
                        CreatedAt = CurrentDate,
                        CreatorId = AccountId,
                        ItemId = maintenance.Id
                    };
                    relatedFiles.Add(relatedFile);
                }
            }
            maintenances.Add(maintenance);
        }

        if (!await _maintenanceRepository.InsertMaintenances(maintenances, relatedFiles, AccountId, CurrentDate))
            throw new ApiException("Tạo yêu cầu thất bại", StatusCode.SERVER_ERROR);

        return ApiResponse.Created("Gửi yêu cầu thành công");
    }

    public string GenerateRequestCode()
    {
        var requests = MainUnitOfWork.MaintenanceRepository.GetQueryAll().ToList();

        var numbers = new List<int>();
        foreach (var t in requests)
        {
            int.TryParse(t!.RequestCode[3..], out int lastNumber);
            numbers.Add(lastNumber);
        }

        string newRequestCode = "MTN1";

        if (requests.Any())
        {
            var lastCode = numbers.AsQueryable().OrderDescending().FirstOrDefault();
            if (requests.Any(x => x!.RequestCode.StartsWith("MTN")))
            {
                newRequestCode = $"MTN{lastCode + 1}";
            }
        }
        return newRequestCode;
    }

    public async Task<ApiResponse> DeleteItem(Guid id)
    {
        var existingMaintenance = await MainUnitOfWork.MaintenanceRepository.FindOneAsync(
                new Expression<Func<Maintenance, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id
                });
        if (existingMaintenance == null)
        {
            throw new ApiException("Không tìm thấy yêu cầu bảo trì này", StatusCode.NOT_FOUND);
        }

        if (existingMaintenance!.Status != RequestStatus.Done &&
                    existingMaintenance.Status != RequestStatus.NotStart &&
                    existingMaintenance.Status != RequestStatus.Cancelled)
        {
            throw new ApiException($"Không thể xóa yêu cầu đang có trạng thái: {existingMaintenance.Status?.GetDisplayName()}", StatusCode.BAD_REQUEST);
        }

        if (!await MainUnitOfWork.MaintenanceRepository.DeleteAsync(existingMaintenance, AccountId, CurrentDate))
        {
            throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
        }
        return ApiResponse.Success();
    }

    public async Task<ApiResponse> DeleteItems(DeleteMutilDto deleteDto)
    {
        var maintenDeleteds = await MainUnitOfWork.MaintenanceRepository.FindAsync(
            new Expression<Func<Maintenance, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => deleteDto.ListId!.Contains(x.Id)
            }, null);

        var maintenances = maintenDeleteds.Where(m => m != null).ToList();

        foreach (var maintenance in maintenances)
        {
            if (maintenance!.Status != RequestStatus.Done &&
                maintenance.Status != RequestStatus.NotStart &&
                maintenance.Status != RequestStatus.Cancelled)
            {
                throw new ApiException($"Không thể xóa yêu cầu đang có trạng thái: {maintenance.Status?.GetDisplayName()}" +
                                       $"kiểm tra yêu cầu: {maintenance.RequestCode}", StatusCode.BAD_REQUEST);
            }
        }

        if (!await MainUnitOfWork.MaintenanceRepository.DeleteAsync(maintenances, AccountId, CurrentDate))
        {
            throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
        }
        return ApiResponse.Success();
    }

    public async Task<ApiResponse> UpdateStatus(Guid id, BaseUpdateStatusDto confirmDto)
    {
        var existingMainten = MainUnitOfWork.MaintenanceRepository.GetQuery()
                                    .Include(t => t!.Asset)
                                    .Where(t => t!.Id == id)
                                    .FirstOrDefault();
        if (existingMainten == null)
        {
            throw new ApiException("Không tìm thấy yêu cầu bảo trì này", StatusCode.NOT_FOUND);
        }

        existingMainten.Status = confirmDto.Status ?? existingMainten.Status;

        if (!await _maintenanceRepository.UpdateStatus(existingMainten, confirmDto, AccountId, CurrentDate))
        {
            throw new ApiException("Cập nhật trạng thái yêu cầu thất bại", StatusCode.SERVER_ERROR);
        }

        return ApiResponse.Success("Cập nhật yêu cầu thành công");
    }
}