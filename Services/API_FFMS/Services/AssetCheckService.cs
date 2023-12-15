using API_FFMS.Dtos;
using API_FFMS.Repositories;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using DocumentFormat.OpenXml.Spreadsheet;

namespace API_FFMS.Services;

public interface IAssetCheckService : IBaseService
{
    Task<ApiResponses<AssetCheckDto>> GetAssetChecks(AssetCheckQueryDto queryDto);
    Task<ApiResponse> DeleteMulti(DeleteMutilDto deleteDto);
    Task<ApiResponse> Delete(Guid id);
    Task<ApiResponse> Create(AssetCheckCreateDto createDto);
    Task<ApiResponse<AssetCheckDto>> GetAssetCheck(Guid id);
    Task<ApiResponse> Update(Guid id, AssetCheckUpdateDto updateDto);
    Task<ApiResponse> ConfirmOrReject(Guid id, AssetCheckUpdateStatusDto requestStatus);
    Task<ApiResponse> CreateItems(List<AssetCheckCreateDto> createDtos);

}

public class AssetCheckService : BaseService, IAssetCheckService
{
    private readonly IAssetcheckRepository _assetcheckRepository;
    public AssetCheckService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor,
                            IMapperRepository mapperRepository, IAssetcheckRepository assetcheckRepository)
                            : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
        _assetcheckRepository = assetcheckRepository;
    }

    public async Task<ApiResponse> Create(AssetCheckCreateDto createDto)
    {
        var asset = await MainUnitOfWork.AssetRepository.FindOneAsync(createDto.AssetId);
        if (asset == null)
            throw new ApiException("Không tồn tại trang thiết bị", StatusCode.NOT_FOUND);

        if (asset.RequestStatus == RequestType.StatusCheck)
        {
            throw new ApiException("Đã có yêu cầu kiểm tra cho thiết bị này", StatusCode.ALREADY_EXISTS);
        }

        if (asset.RequestStatus != RequestType.Operational)
            throw new ApiException("Thiết bị đang trong một yêu cầu khác", StatusCode.BAD_REQUEST);

        var assetCheck = createDto.ProjectTo<AssetCheckCreateDto, AssetCheck>();

        assetCheck.Description = createDto.Description ?? "Yêu cầu kiểm tra";
        assetCheck.RequestCode = GenerateRequestCode();
        assetCheck.Priority ??= Priority.Medium;

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
                ItemId = assetCheck.Id,
                IsVerified = false,
                IsReported = false,
            };

            mediaFiles.Add(report);
        }

        if (!await _assetcheckRepository.InsertAssetCheck(assetCheck, mediaFiles, AccountId, CurrentDate))
        {
            throw new ApiException("Tạo yêu cầu thất bại", StatusCode.SERVER_ERROR);
        }

        return ApiResponse.Created("Tạo yêu cầu thành công");
    }

    public async Task<ApiResponse> Delete(Guid id)
    {
        var existingAssetcheck = await MainUnitOfWork.AssetCheckRepository.FindOneAsync(
                                new Expression<Func<AssetCheck, bool>>[]
                                {
                                    x => !x.DeletedAt.HasValue,
                                    x => x.Id == id
                                });
        if (existingAssetcheck == null)
        {
            throw new ApiException("Không tìm thấy yêu cầu kiểm tra này", StatusCode.NOT_FOUND);
        }

        if (existingAssetcheck.Status != RequestStatus.Done &&
            existingAssetcheck.Status != RequestStatus.NotStart &&
            existingAssetcheck.Status != RequestStatus.Cancelled)
        {
            throw new ApiException($"Không thể xóa yêu cầu đang có trạng thái: {existingAssetcheck.Status?.GetDisplayName()}", StatusCode.BAD_REQUEST);
        }

        if (!await _assetcheckRepository.DeleteAssetCheck(existingAssetcheck, AccountId, CurrentDate))
        {
            throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
        }
        return ApiResponse.Success();
    }

    public async Task<ApiResponse> DeleteMulti(DeleteMutilDto deleteDto)
    {
        var assetcheckDeleteds = await MainUnitOfWork.AssetCheckRepository.FindAsync(
                                new Expression<Func<AssetCheck, bool>>[]
                                {
                                    x => !x.DeletedAt.HasValue,
                                    x => deleteDto.ListId!.Contains(x.Id)
                                }, null);

        var assetChecks = assetcheckDeleteds.Where(a => a != null).ToList();

        foreach (var assetCheck in assetChecks)
        {
            if (assetCheck!.Status != RequestStatus.Done &&
                assetCheck.Status != RequestStatus.NotStart &&
                assetCheck.Status != RequestStatus.Cancelled)
            {
                throw new ApiException($"Không thể xóa yêu cầu đang có trạng thái: {assetCheck.Status?.GetDisplayName()}" +
                                       $"kiểm tra yêu cầu: {assetCheck.RequestCode}", StatusCode.BAD_REQUEST);
            }
        }

        if (!await _assetcheckRepository.DeleteMulti(assetChecks, AccountId, CurrentDate))
        {
            throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
        }
        return ApiResponse.Success();
    }

    public async Task<ApiResponse<AssetCheckDto>> GetAssetCheck(Guid id)
    {
        var assetCheck = await MainUnitOfWork.AssetCheckRepository.FindOneAsync<AssetCheckDto>(
            new Expression<Func<AssetCheck, bool>>[]
            {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id
            });
        if (assetCheck == null)
        {
            throw new ApiException("Không tìm thấy yêu cầu", StatusCode.NOT_FOUND);
        }

        assetCheck.Asset = await MainUnitOfWork.AssetRepository.FindOneAsync<AssetBaseDto>(
                new Expression<Func<Asset, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == assetCheck.AssetId
                });

        if (assetCheck.Asset != null)
        {
            assetCheck.Asset.StatusObj = assetCheck.Asset.Status.GetValue();
            assetCheck.Asset.RequestStatusObj = assetCheck.Asset.RequestStatus.GetValue();

            assetCheck.AssetType = await MainUnitOfWork.AssetTypeRepository.FindOneAsync<AssetTypeDto>(
                new Expression<Func<AssetType, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == assetCheck.Asset.TypeId
                });
            if (assetCheck.AssetType != null)
            {
                assetCheck.Category = await MainUnitOfWork.CategoryRepository.FindOneAsync<CategoryDto>(
                    new Expression<Func<Category, bool>>[]
                    {
                        x => !x.DeletedAt.HasValue,
                        x => x.Id == assetCheck.AssetType.CategoryId
                    });
            }
        }

        assetCheck.Location = await MainUnitOfWork.RoomRepository.FindOneAsync<RoomBaseDto>(
                        new Expression<Func<Room, bool>>[]
                        {
                            x => !x.DeletedAt.HasValue,
                            x => x.Id == assetCheck.RoomId
                        });

        assetCheck.AssignTo = await MainUnitOfWork.UserRepository.FindOneAsync<UserBaseDto>(
            new Expression<Func<User, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.Id == assetCheck.AssignedTo
            });

        var relatedMediaFiles = await MainUnitOfWork.MediaFileRepository.GetQuery()
            .Where(m => m!.ItemId == id && !m.IsReported).FirstOrDefaultAsync();

        if (relatedMediaFiles != null)
        {
            assetCheck.RelatedFiles = JsonConvert.DeserializeObject<List<MediaFileDetailDto>>(relatedMediaFiles.Uri);
        }

        var reports = await MainUnitOfWork.MediaFileRepository.GetQuery()
            .Where(m => m!.ItemId == id && m.IsReported).OrderByDescending(x => x!.CreatedAt).ToListAsync();

        //TODO: orderby
        assetCheck.Reports = new List<MediaFileDto>();
        foreach (var report in reports)
        {
            // Deserialize the URI string back into a List<string>
            var uriList = JsonConvert.DeserializeObject<List<string>>(report.Uri);

            assetCheck.Reports.Add(new MediaFileDto
            {
                ItemId = report.ItemId,
                Uri = uriList,
                FileType = report.FileType,
                Content = report.Content,
                IsReject = report.IsReject,
                RejectReason = report.RejectReason
            });
        }

        assetCheck.PriorityObj = assetCheck.Priority.GetValue();
        assetCheck.IsVerified = assetCheck.IsVerified;
        assetCheck.Status = assetCheck.Status;
        assetCheck.StatusObj = assetCheck.Status!.GetValue();

        assetCheck = await _mapperRepository.MapCreator(assetCheck);
        return ApiResponse<AssetCheckDto>.Success(assetCheck);
    }

    public async Task<ApiResponses<AssetCheckDto>> GetAssetChecks(AssetCheckQueryDto queryDto)
    {
        var keyword = queryDto.Keyword?.Trim().ToLower();
        var assetCheckQuery = MainUnitOfWork.AssetCheckRepository.GetQuery()
                                  .Where(x => !x!.DeletedAt.HasValue);

        if (queryDto.IsInternal != null)
        {
            assetCheckQuery = assetCheckQuery.Where(x => x!.IsInternal == queryDto.IsInternal);
        }

        if (queryDto.AssignedTo != null)
        {
            assetCheckQuery = assetCheckQuery.Where(x => x!.AssignedTo == queryDto.AssignedTo);
        }

        if (queryDto.AssetId != null)
        {
            assetCheckQuery = assetCheckQuery.Where(x => x!.AssetId == queryDto.AssetId);
        }

        if (queryDto.Status != null)
        {
            assetCheckQuery = assetCheckQuery.Where(x => x!.Status == queryDto.Status);
        }

        if (queryDto.Priority != null)
        {
            assetCheckQuery = assetCheckQuery.Where(x => x!.Priority == queryDto.Priority);
        }

        if (!string.IsNullOrEmpty(keyword))
        {
            assetCheckQuery = assetCheckQuery.Where(x => x!.Description!.ToLower().Contains(keyword)
                                                      || x.Notes!.ToLower().Contains(keyword)
                                                      || x.RequestCode.ToLower().Contains(keyword));
        }

        assetCheckQuery = assetCheckQuery.OrderByDescending(x => x!.CreatedAt);

        var joinTables = from assetCheck in assetCheckQuery
                         join asset in MainUnitOfWork.AssetRepository.GetQuery() on assetCheck.AssetId equals asset.Id into
                             assetGroup
                         from asset in assetGroup.DefaultIfEmpty()
                         join location in MainUnitOfWork.RoomRepository.GetQuery() on assetCheck.RoomId equals location.Id into
                             locationGroup
                         from location in locationGroup.DefaultIfEmpty()
                         select new
                         {
                             AssetCheck = assetCheck,
                             Asset = asset,
                             Location = location
                         };

        var totalCount = await joinTables.CountAsync();
        joinTables = joinTables.Skip(queryDto.Skip()).Take(queryDto.PageSize);

        var assetChecks = await joinTables.Select(x => new AssetCheckDto
        {
            Id = x.AssetCheck.Id,
            IsVerified = x.AssetCheck.IsVerified,
            AssetId = x.AssetCheck.AssetId,
            RoomId = x.Location.Id,
            Description = x.AssetCheck.Description,
            RequestCode = x.AssetCheck.RequestCode,
            RequestDate = x.AssetCheck.RequestDate,
            CompletionDate = x.AssetCheck.CompletionDate,
            IsInternal = x.AssetCheck.IsInternal,
            Status = x.AssetCheck.Status,
            StatusObj = x.AssetCheck.Status.GetValue(),
            PriorityObj = x.AssetCheck.Priority.GetValue(),
            Result = x.AssetCheck.Result,
            Notes = x.AssetCheck.Notes,
            Checkin = x.AssetCheck.Checkin,
            Checkout = x.AssetCheck.Checkout,
            AssetTypeId = x.AssetCheck.AssetTypeId,
            CategoryId = x.AssetCheck.CategoryId,
            Asset = new AssetBaseDto
            {
                Id = x.Asset.Id,
                Description = x.Asset.Description,
                AssetCode = x.Asset.AssetCode,
                AssetName = x.Asset.AssetName,
                Quantity = x.Asset.Quantity,
                IsMovable = x.Asset.IsMovable,
                IsRented = x.Asset.IsRented,
                ManufacturingYear = x.Asset.ManufacturingYear,
                StatusObj = x.Asset.Status.GetValue(),
                Status = x.Asset.Status,
                RequestStatusObj = x.Asset.RequestStatus.GetValue(),
                RequestStatus = x.Asset.RequestStatus,
                StartDateOfUse = x.Asset.StartDateOfUse,
                SerialNumber = x.Asset.SerialNumber,
                LastCheckedDate = x.Asset.LastCheckedDate,
                LastMaintenanceTime = x.Asset.LastMaintenanceTime,
                TypeId = x.Asset.TypeId,
                ModelId = x.Asset.ModelId,
                CreatedAt = x.Asset.CreatedAt,
                EditedAt = x.Asset.EditedAt,
                CreatorId = x.Asset.CreatorId ?? Guid.Empty,
                EditorId = x.Asset.EditorId ?? Guid.Empty
            },
            Location = new RoomBaseDto
            {
                Area = x.Location.Area,
                Capacity = x.Location.Capacity,
                Id = x.Location.Id,
                FloorId = x.Location.FloorId,
                RoomName = x.Location.RoomName,
                Description = x.Location.Description,
                RoomTypeId = x.Location.RoomTypeId,
                CreatedAt = x.Location.CreatedAt,
                EditedAt = x.Location.EditedAt,
                RoomCode = x.Location.RoomCode,
                StatusId = x.Location.StatusId,
                CreatorId = x.Location.CreatorId ?? Guid.Empty,
                EditorId = x.Location.EditorId ?? Guid.Empty
            }
        }).ToListAsync();

        assetChecks = await _mapperRepository.MapCreator(assetChecks);

        return ApiResponses<AssetCheckDto>.Success(
            assetChecks,
            totalCount,
            queryDto.PageSize,
            queryDto.Page,
            (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
    }

    public async Task<ApiResponse> Update(Guid id, AssetCheckUpdateDto updateDto)
    {
        var existingAssetcheck = await MainUnitOfWork.AssetCheckRepository.FindOneAsync(id);
        if (existingAssetcheck == null)
        {
            throw new ApiException("Không tìm thấy yêu cầu", StatusCode.NOT_FOUND);
        }

        if (existingAssetcheck.Status != RequestStatus.NotStart)
        {
            throw new ApiException("Chỉ được cập nhật yêu cầu đang có trạng thái chưa bắt đầu", StatusCode.BAD_REQUEST);
        }

        existingAssetcheck.Description = updateDto.Description ?? existingAssetcheck.Description;
        existingAssetcheck.Notes = updateDto.Notes ?? existingAssetcheck.Notes;
        existingAssetcheck.CategoryId = updateDto.CategoryId ?? existingAssetcheck.CategoryId;
        existingAssetcheck.IsInternal = updateDto.IsInternal ?? existingAssetcheck.IsInternal;
        existingAssetcheck.AssetTypeId = updateDto.AssetTypeId ?? existingAssetcheck.AssetTypeId;
        existingAssetcheck.AssignedTo = updateDto.AssignedTo ?? existingAssetcheck.AssignedTo;
        existingAssetcheck.Priority = updateDto.Priority ?? existingAssetcheck.Priority;

        if (updateDto.AssetId != null && updateDto.AssetId != existingAssetcheck.AssetId)
        {
            var assetUpdate = await MainUnitOfWork.AssetRepository.FindOneAsync((Guid)updateDto.AssetId);
            if (assetUpdate == null)
            {
                throw new ApiException("Không cần tồn tại thiết bị", StatusCode.NOT_FOUND);
            }

            if (assetUpdate.RequestStatus == RequestType.StatusCheck)
            {
                throw new ApiException("Đã có yêu cầu kiểm tra cho thiết bị này", StatusCode.ALREADY_EXISTS);
            }

            if (assetUpdate.RequestStatus != RequestType.Operational)
            {
                throw new ApiException("Thiết bị đang trong một yêu cầu khác", StatusCode.BAD_REQUEST);
            }
        }
        else if (updateDto.AssetId != null && updateDto.AssetId == existingAssetcheck.AssetId)
        {
            existingAssetcheck.AssetId = updateDto.AssetId ?? existingAssetcheck.AssetId;
        }

        var mediaFileQuery = MainUnitOfWork.MediaFileRepository.GetQuery().Where(x => x!.ItemId == id).ToList();

        var newMediaFile = updateDto.RelatedFiles != null ? updateDto.RelatedFiles.Select(dto => new Report
        {
            FileName = dto.FileName,
            Uri = dto.Uri,
            CreatedAt = CurrentDate,
            CreatorId = AccountId,
            ItemId = id,
            FileType = FileType.File
        }).ToList() : new List<Report>();


        var additionMediaFiles = newMediaFile.Except(mediaFileQuery).ToList();
        var removalMediaFiles = mediaFileQuery.Except(newMediaFile).ToList();

        if (!await _assetcheckRepository.UpdateAssetCheck(existingAssetcheck, additionMediaFiles, removalMediaFiles, AccountId, CurrentDate))
        {
            throw new ApiException("Cập nhật thất bại", StatusCode.SERVER_ERROR);
        }

        return ApiResponse.Success("Cập nhật thành công");

    }

    public async Task<ApiResponse> ConfirmOrReject(Guid id, AssetCheckUpdateStatusDto confirmOrRejectDto)
    {
        var existingAssetCheck = MainUnitOfWork.AssetCheckRepository.GetQuery()
                                .Include(t => t!.Asset)
                                .Where(t => t!.Id == id)
                                .FirstOrDefault();
        if (existingAssetCheck == null)
        {
            throw new ApiException("Không tìm thấy yêu cầu kiểm tra này", StatusCode.NOT_FOUND);
        }

        existingAssetCheck.Status = confirmOrRejectDto.Status ?? existingAssetCheck.Status;
        existingAssetCheck.IsVerified = confirmOrRejectDto.IsVerified ?? existingAssetCheck.IsVerified;

        if (!await _assetcheckRepository.ConfirmOrReject(existingAssetCheck, confirmOrRejectDto.Status, AccountId, CurrentDate))
        {
            throw new ApiException("Cập nhật trạng thái yêu cầu thất bại", StatusCode.SERVER_ERROR);
        }

        return ApiResponse.Success("Cập nhật yêu cầu thành công");
    }

    public string GenerateRequestCode()
    {
        var requests = MainUnitOfWork.AssetCheckRepository.GetQueryAll().ToList();

        var numbers = new List<int>();
        foreach (var t in requests)
        {
            int.TryParse(t!.RequestCode[2..], out int lastNumber);
            numbers.Add(lastNumber);
        }

        string newRequestCode = "AC1";

        if (requests.Any())
        {
            var lastCode = numbers.AsQueryable().OrderDescending().FirstOrDefault();
            if (requests.Any(x => x!.RequestCode.StartsWith("AC")))
            {
                newRequestCode = $"AC{lastCode + 1}";
            }
        }
        return newRequestCode;
    }

    public async Task<ApiResponse> CreateItems(List<AssetCheckCreateDto> createDtos)
    {
        var assets = await MainUnitOfWork.AssetRepository.FindAsync(
                new Expression<Func<Asset, bool>>[]
                {
                    x => !x!.DeletedAt.HasValue,
                    x => createDtos.Select(dto => dto.AssetId).Contains(x.Id)
                }, null);

        if (assets.Any(asset => asset!.Status != AssetStatus.Operational))
        {
            throw new ApiException("Trang thiết bị đang trong một yêu cầu khác", StatusCode.SERVER_ERROR);
        }

        var assetChecks = createDtos.Select(dto => dto.ProjectTo<AssetCheckCreateDto, AssetCheck>())
                                         .ToList();

        if (!await _assetcheckRepository.InsertAssetChecks(assetChecks, AccountId, CurrentDate))
            throw new ApiException("Tạo yêu cầu thất bại", StatusCode.SERVER_ERROR);

        return ApiResponse.Created("Gửi yêu cầu thành công");
    }
}