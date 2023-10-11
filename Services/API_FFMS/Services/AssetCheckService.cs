using API_FFMS.Dtos;
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
}

public class AssetCheckService : BaseService, IAssetCheckService
{
    public AssetCheckService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
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

    public async Task<ApiResponses<AssetCheckDto>> GetAssetChecks(AssetCheckQueryDto queryDto)
    {
        var keyword = queryDto.Keyword?.Trim().ToLower();
        var assetCheckQuery = MainUnitOfWork.AssetCheckRepository.GetQuery()
                                  .Where(x => !x!.DeletedAt.HasValue);

        if (keyword != null)
        {
            assetCheckQuery = assetCheckQuery.Where(x => x!.RequestCode.ToLower().Contains(keyword));
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

        if (queryDto.RequestDate != null)
        {
            assetCheckQuery = assetCheckQuery.Where(x => x!.RequestDate == queryDto.RequestDate);
        }

        if (queryDto.CompletionDate != null)
        {
            assetCheckQuery = assetCheckQuery.Where(x => x!.CompletionDate == queryDto.CompletionDate);
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
            RequestId = x.AssetCheck.Id,
            IsVerified = x.AssetCheck.IsVerified,
            AssetId = x.AssetCheck.AssetId,
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
}