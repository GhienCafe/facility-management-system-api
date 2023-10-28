using API_FFMS.Dtos;
using API_FFMS.Repositories;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API_FFMS.Services;

public interface IAssetCheckService : IBaseService
{
    Task<ApiResponses<AssetCheckDto>> GetAssetChecks(AssetCheckQueryDto queryDto);
    Task<ApiResponse> DeleteAssetChecks(List<Guid> ids);
    Task<ApiResponse> Delete(Guid id);
    Task<ApiResponse> Create(AssetCheckCreateDto createDto);
    Task<ApiResponse<AssetCheckDto>> GetAssetCheck(Guid id);
    Task<ApiResponse> Update(Guid id, AssetCheckUpdateDto updateDto);
    Task<ApiResponse> UpdateStatus(Guid id, AssetCheckUpdateStatusDto requestStatus);
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
        var assetCheck = createDto.ProjectTo<AssetCheckCreateDto, AssetCheck>();
        assetCheck.RequestCode = GenerateRequestCode();
        if (!await _assetcheckRepository.InsertAssetCheck(assetCheck, AccountId, CurrentDate))
        {
            throw new ApiException("Thêm mới thất bại", StatusCode.SERVER_ERROR);
        }

        return ApiResponse.Created("Thêm mới thành công");
    }

    public async Task<ApiResponse> Delete(Guid id)
    {
        var existingAssetcheck = await MainUnitOfWork.AssetCheckRepository.FindAsync(
                                new Expression<Func<AssetCheck, bool>>[]
                                {
                                    x => !x.DeletedAt.HasValue,
                                    x => x.Id == id
                                }, null);
        if (existingAssetcheck == null)
        {
            throw new ApiException("Không tìm thấy yêu cầu kiểm tra này", StatusCode.NOT_FOUND);
        }

        if (!await MainUnitOfWork.AssetCheckRepository.DeleteAsync(existingAssetcheck, AccountId, CurrentDate))
        {
            throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
        }
        return ApiResponse.Success();
    }

    public async Task<ApiResponse> DeleteAssetChecks(List<Guid> ids)
    {
        var assetcheckDeleteds = await MainUnitOfWork.AssetCheckRepository.FindAsync(
                                new Expression<Func<AssetCheck, bool>>[]
                                {
                                    x => !x.DeletedAt.HasValue,
                                    x => ids.Contains(x.Id)
                                }, null);
        if (!await MainUnitOfWork.AssetCheckRepository.DeleteAsync(assetcheckDeleteds, AccountId, CurrentDate))
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
            assetCheck.Asset!.StatusObj = assetCheck.Asset.Status?.GetValue();
            var location = await MainUnitOfWork.RoomAssetRepository.FindOneAsync<RoomAsset>(
                new Expression<Func<RoomAsset, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.AssetId == assetCheck.Asset.Id,
                    x => x.ToDate == null
                });
            if (location != null)
            {
                assetCheck.Location = await MainUnitOfWork.RoomRepository.FindOneAsync<RoomBaseDto>(
                        new Expression<Func<Room, bool>>[]
                        {
                            x => !x.DeletedAt.HasValue,
                            x => x.Id == location.RoomId
                        });
            }

            assetCheck.AssetType = await MainUnitOfWork.AssetTypeRepository.FindOneAsync<AssetTypeDto>(
                new Expression<Func<AssetType, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == assetCheck.Asset.TypeId
                });
            if(assetCheck.AssetType != null)
            {
                assetCheck.Category = await MainUnitOfWork.CategoryRepository.FindOneAsync<CategoryDto>(
                    new Expression<Func<Category, bool>>[]
                    {
                        x => !x.DeletedAt.HasValue,
                        x => x.Id == assetCheck.AssetType.CategoryId
                    });
            }
        }

        assetCheck.AssignTo = await MainUnitOfWork.UserRepository.FindOneAsync<UserBaseDto>(
            new Expression<Func<User, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.Id == assetCheck.AssignedTo
            });

        var mediaFileQuery = MainUnitOfWork.MediaFileRepository.GetQuery().Where(m => m!.ItemId == assetCheck.Id);
        assetCheck.MediaFile = new MediaFileDto
        {
            FileType = mediaFileQuery.Select(m => m!.FileType).FirstOrDefault(),
            Uri = mediaFileQuery.Select(m => m!.Uri).ToList(),
            Content = mediaFileQuery.Select(m => m!.Content).FirstOrDefault()
        };
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

        if (!string.IsNullOrEmpty(keyword))
        {
            assetCheckQuery = assetCheckQuery.Where(x => x!.Description!.ToLower().Contains(keyword)
                                                      || x.Notes!.ToLower().Contains(keyword)
                                                      || x.RequestCode.ToLower().Contains(keyword));
        }

        var joinTables = from assetCheck in assetCheckQuery
                         join asset in MainUnitOfWork.AssetRepository.GetQuery() on assetCheck.AssetId equals asset.Id into
                             assetGroup
                         from asset in assetGroup.DefaultIfEmpty()
                         join assetRoom in MainUnitOfWork.RoomAssetRepository.GetQuery() on asset.Id equals assetRoom.AssetId into
                             assetRoomGroup
                         from assetRoom in assetRoomGroup.DefaultIfEmpty()
                         where assetRoom.ToDate == null
                         join location in MainUnitOfWork.RoomRepository.GetQuery() on assetRoom.RoomId equals location.Id into
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
            //RequestId = x.AssetCheck.Id,
            IsVerified = x.AssetCheck.IsVerified,
            AssetId = x.AssetCheck.AssetId,
            Description = x.AssetCheck.Description,
            RequestCode = x.AssetCheck.RequestCode,
            RequestDate = x.AssetCheck.RequestDate,
            CompletionDate = x.AssetCheck.CompletionDate,
            Status = x.AssetCheck.Status,
            StatusObj = x.AssetCheck.Status!.GetValue(),
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
            throw new ApiException("Không tìm thấy nội dung", StatusCode.NOT_FOUND);
        }

        existingAssetcheck.Description = updateDto.Description ?? existingAssetcheck.Description;
        //existingAssetcheck.Status = updateDto.Status ?? existingAssetcheck.Status;
        existingAssetcheck.Notes = updateDto.Notes ?? existingAssetcheck.Notes;
        existingAssetcheck.CategoryId = updateDto.CategoryId ?? existingAssetcheck.CategoryId;
        existingAssetcheck.IsInternal = updateDto.IsInternal ?? existingAssetcheck.IsInternal;
        existingAssetcheck.AssetTypeId = updateDto.AssetTypeId ?? existingAssetcheck.AssetTypeId;
        existingAssetcheck.AssignedTo = updateDto.AssignedTo ?? existingAssetcheck.AssignedTo;
        existingAssetcheck.CompletionDate = updateDto.CompletionDate ?? existingAssetcheck.CompletionDate;
        existingAssetcheck.RequestDate = updateDto.RequestDate ?? existingAssetcheck.RequestDate;
        existingAssetcheck.Priority = updateDto.Priority ?? existingAssetcheck.Priority;

        if (!await MainUnitOfWork.AssetCheckRepository.UpdateAsync(existingAssetcheck, AccountId, CurrentDate))
        {
            throw new ApiException("Cập nhật thất bại", StatusCode.SERVER_ERROR);
        }

        return ApiResponse.Success("Cập nhật thành công");
    }

    public async Task<ApiResponse> UpdateStatus(Guid id, AssetCheckUpdateStatusDto requestStatus)
    {
        var existingAssetCheck = MainUnitOfWork.AssetCheckRepository.GetQuery()
                                .Include(t => t!.Asset)
                                .Where(t => t!.Id == id)
                                .FirstOrDefault();
        if (existingAssetCheck == null)
        {
            throw new ApiException("Không tìm thấy yêu cầu kiểm tra này", StatusCode.NOT_FOUND);
        }

        existingAssetCheck.Status = requestStatus.Status ?? existingAssetCheck.Status;
        existingAssetCheck.IsVerified = requestStatus.IsVerified ?? existingAssetCheck.IsVerified; 

        if (!await _assetcheckRepository.UpdateStatus(existingAssetCheck, requestStatus.Status, AccountId, CurrentDate))
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
}