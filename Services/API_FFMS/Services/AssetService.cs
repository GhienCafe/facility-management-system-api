using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Office2010.Excel;
using API_FFMS.Repositories;
using System.Linq;
using DocumentFormat.OpenXml.Spreadsheet;

namespace API_FFMS.Services;
public interface IAssetService : IBaseService
{
    Task<ApiResponses<AssetDto>> GetAssets(AssetQueryDto queryDto);
    Task<ApiResponse<AssetDetailDto>> GetAsset(Guid id);
    Task<ApiResponse> Create(AssetCreateDto createDto);
    Task<ApiResponse> Update(Guid id, AssetUpdateDto updateDto);
    Task<ApiResponse> Delete(Guid id);
    Task<ApiResponse> DeleteAssets(List<Guid> ids);
    Task<ApiResponses<RoomAssetDto>> GetAssetsInRoom(Guid roomId, RoomAssetQueryDto queryDto);
}

public class AssetService : BaseService, IAssetService
{
    private readonly IAssetRepository _assetRepository;
    public AssetService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor,
                        IMapperRepository mapperRepository, IAssetRepository assetRepository)
                        : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
        _assetRepository = assetRepository;
    }

    public async Task<ApiResponse> Create(AssetCreateDto createDto)
    {
        var asset = createDto.ProjectTo<AssetCreateDto, Asset>();

        if (!await _assetRepository.InsertAsset(asset, AccountId, CurrentDate))
            throw new ApiException("Thêm trang thiết bị thất bại", StatusCode.SERVER_ERROR);

        return ApiResponse.Created("Thêm trang thiết bị thành công");
    }

    public async Task<ApiResponse> Delete(Guid id)
    {
        var existingAsset = await MainUnitOfWork.AssetRepository.FindOneAsync(id);

        if (existingAsset == null)
            throw new ApiException("Không tìm thất trang thiết bị", StatusCode.NOT_FOUND);

        if (!await MainUnitOfWork.AssetRepository.DeleteAsync(existingAsset, AccountId, CurrentDate))
            throw new ApiException("Xóa trang thiết bị thất bại", StatusCode.SERVER_ERROR);

        return ApiResponse.Success("Xóa trang thiết bị thành công");
    }

    public async Task<ApiResponses<AssetDto>> GetAssets(AssetQueryDto queryDto)
    {
        var keyword = queryDto.Keyword?.Trim().ToLower();
        var assetDataSet = MainUnitOfWork.AssetRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);

        if (!string.IsNullOrEmpty(keyword))
        {
            assetDataSet = assetDataSet.Where(x => x!.AssetCode!.ToLower().Contains(keyword)
                || x.AssetName.ToLower().Contains(keyword)
                || x.Description!.ToLower().Contains(keyword)
                || x.SerialNumber!.ToLower().Contains(keyword));
        }

        if (queryDto.Status != null)
            assetDataSet = assetDataSet.Where(x => x!.Status == queryDto.Status);

        if (queryDto.IsMovable != null)
            assetDataSet = assetDataSet.Where(x => x!.IsMovable == queryDto.IsMovable);

        if (queryDto.IsRented != null)
            assetDataSet = assetDataSet.Where(x => x!.IsRented == queryDto.IsRented);

        if (queryDto.TypeId != null)
            assetDataSet = assetDataSet.Where(x => x!.TypeId == queryDto.TypeId);

        if (queryDto.ModelId != null)
            assetDataSet = assetDataSet.Where(x => x!.ModelId == queryDto.ModelId);

        if (queryDto.CreateAtFrom != null)
            assetDataSet = assetDataSet.Where(x => x!.CreatedAt >= queryDto.CreateAtFrom);

        if (queryDto.CreateAtTo != null)
            assetDataSet = assetDataSet.Where(x => x!.CreatedAt <= queryDto.CreateAtTo);

        // Order
        var isDescending = queryDto.OrderBy.EndsWith("desc", StringComparison.OrdinalIgnoreCase);
        var orderByColumn = queryDto.OrderBy.Split(' ')[0];
        assetDataSet = isDescending ? assetDataSet.OrderByDescending(user => EF.Property<object>(user!, orderByColumn)) : assetDataSet.OrderBy(user => EF.Property<object>(user!, orderByColumn));

        var joinTables = from asset in assetDataSet
                         join type in MainUnitOfWork.AssetTypeRepository.GetQuery() on asset.TypeId equals type.Id
                             into typeGroup
                         from type in typeGroup.DefaultIfEmpty()
                         join model in MainUnitOfWork.ModelRepository.GetQuery() on asset.ModelId equals model.Id
                             into modelGroup
                         from model in modelGroup.DefaultIfEmpty()
                         select new
                         {
                             Asset = asset,
                             Type = type,
                             Model = model
                         };

        var totalCount = joinTables.Count();

        joinTables = joinTables.Skip(queryDto.Skip())
            .Take(queryDto.PageSize);

        var assets = await joinTables.Select(x => new AssetDto
        {
            Id = x.Asset.Id,
            Description = x.Asset.Description,
            Status = x.Asset.Status,
            StatusObj = x.Asset.Status.GetValue(),
            LastCheckedDate = x.Asset.LastCheckedDate,
            StartDateOfUse = x.Asset.StartDateOfUse,
            AssetCode = x.Asset.AssetCode,
            AssetName = x.Asset.AssetName,
            Quantity = x.Asset.Quantity,
            IsMovable = x.Asset.IsMovable,
            IsRented = x.Asset.IsRented,
            ManufacturingYear = x.Asset.ManufacturingYear,
            ModelId = x.Asset.ModelId,
            SerialNumber = x.Asset.SerialNumber,
            TypeId = x.Asset.TypeId,
            LastMaintenanceTime = x.Asset.LastMaintenanceTime,
            CreatedAt = x.Asset.CreatedAt,
            EditedAt = x.Asset.EditedAt,
            CreatorId = x.Asset.CreatorId ?? Guid.Empty,
            EditorId = x.Asset.EditorId ?? Guid.Empty,
            Type = x.Type != null ? new AssetTypeDto
            {
                Id = x.Type.Id,
                Description = x.Type.Description,
                TypeCode = x.Type.TypeCode,
                TypeName = x.Type.TypeName,
                Unit = x.Type.Unit,
                UnitObj = x.Type.Unit.GetValue(),
                CreatedAt = x.Type.CreatedAt,
                EditedAt = x.Type.EditedAt,
                CreatorId = x.Type.CreatorId ?? Guid.Empty,
                EditorId = x.Type.EditorId ?? Guid.Empty
            } : null,
            Model = x.Model.ProjectTo<Model, ModelDto>()
        }).ToListAsync();

        assets = await _mapperRepository.MapCreator(assets);

        return ApiResponses<AssetDto>.Success(
            assets,
            totalCount,
            queryDto.PageSize,
            queryDto.Page,
            (int)Math.Ceiling(totalCount / (double)queryDto.PageSize)
        );
    }

    public async Task<ApiResponse<AssetDetailDto>> GetAsset(Guid id)
    {
        var asset = await MainUnitOfWork.AssetRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue && x.Id == id)
            .Select(x => new AssetDetailDto
            {
                Id = x!.Id,
                Description = x.Description,
                Status = x.Status,
                StatusObj = x.Status.GetValue(),
                AssetCode = x.AssetCode,
                AssetName = x.AssetName,
                Quantity = x.Quantity,
                IsMovable = x.IsMovable,
                //IsRented = x.IsRented,
                ManufacturingYear = x.ManufacturingYear,
                ModelId = x.ModelId,
                SerialNumber = x.SerialNumber,
                TypeId = x.TypeId,
                LastMaintenanceTime = x.LastMaintenanceTime,
                LastCheckedDate = x.LastCheckedDate,
                StartDateOfUse = x.StartDateOfUse,
                CreatedAt = x.CreatedAt,
                EditedAt = x.EditedAt,
                CreatorId = x.CreatorId ?? Guid.Empty,
                EditorId = x.EditorId ?? Guid.Empty,
            }).FirstOrDefaultAsync();

        if (asset == null)
            throw new ApiException("Không tìm thất trang thiết bị", StatusCode.NOT_FOUND);

        asset.Model = (await MainUnitOfWork.ModelRepository
            .FindOneAsync(asset.ModelId ?? Guid.Empty))?.ProjectTo<Model, ModelDto>();

        // asset.Type = (await MainUnitOfWork.AssetTypeRepository
        //     .FindOneAsync(asset.TypeId ?? Guid.Empty))?.ProjectTo<AssetType, AssetTypeDto>();

        asset = await _mapperRepository.MapCreator(asset);

        return ApiResponse<AssetDetailDto>.Success(asset);
    }

    public async Task<ApiResponse> Update(Guid id, AssetUpdateDto updateDto)
    {
        var existingAsset = await MainUnitOfWork.AssetRepository.FindOneAsync(id);
        if (existingAsset == null)
        {
            throw new ApiException("Không tìm thấy trang thiết bị", StatusCode.NOT_FOUND);
        }

        existingAsset.TypeId = updateDto.TypeId ?? existingAsset.TypeId;
        existingAsset.ModelId = updateDto.ModelId ?? existingAsset.ModelId;
        existingAsset.IsRented = updateDto.IsRented ?? existingAsset.IsRented;
        existingAsset.TypeId = updateDto.TypeId ?? existingAsset.TypeId;
        existingAsset.AssetName = updateDto.AssetName ?? existingAsset.AssetName;
        existingAsset.Status = updateDto.Status ?? existingAsset.Status;
        existingAsset.ManufacturingYear = updateDto.ManufacturingYear ?? existingAsset.ManufacturingYear;
        existingAsset.SerialNumber = updateDto.SerialNumber ?? existingAsset.SerialNumber;
        existingAsset.Quantity = updateDto.Quantity ?? existingAsset.Quantity;
        existingAsset.Description = updateDto.Description ?? existingAsset.Description;
        existingAsset.AssetCode = updateDto.AssetCode ?? existingAsset.AssetCode;
        existingAsset.IsMovable = updateDto.IsMovable ?? existingAsset.IsMovable;
        //existingAsset.IsRented = updateDto.IsRented ?? existingAsset.IsRented;
        existingAsset.LastMaintenanceTime = updateDto.LastMaintenanceTime ?? existingAsset.LastMaintenanceTime;

        if (!await MainUnitOfWork.AssetRepository.UpdateAsync(existingAsset, AccountId, CurrentDate))
            throw new ApiException("Cập nhật thông tin trang thiết bị thất bại", StatusCode.SERVER_ERROR);

        return ApiResponse.Success("Cập nhật thông tin trang thiết bị thành công");
    }


    public async Task<ApiResponses<RoomAssetDto>> GetAssetsInRoom(Guid roomId, RoomAssetQueryDto queryDto)
    {
        var keyword = queryDto.Keyword?.Trim().ToLower();
        var room = await MainUnitOfWork.RoomRepository.FindOneAsync<RoomDto>(new Expression<Func<Room, bool>>[]
           {
                x => !x.DeletedAt.HasValue,
                x => x.Id == roomId
           });

        if (room == null)
            throw new ApiException("Không tìm thấy phòng", StatusCode.NOT_FOUND);

        var roomAssetDataset = MainUnitOfWork.RoomAssetRepository.GetQuery();
        var assetDataset = MainUnitOfWork.AssetRepository.GetQuery();

        var joinedAssets = from roomAsset in roomAssetDataset
                           join asset in assetDataset on roomAsset.AssetId equals asset.Id
                           where roomAsset.RoomId == roomId
                           select new
                           {
                               RoomAsset = roomAsset,
                               Asset = asset
                           };

        if (queryDto.IsInCurrent is true)
        {
            joinedAssets = joinedAssets.Where(x => x!.RoomAsset.ToDate == null);
        }
        else if (queryDto.ToDate != null)
        {
            joinedAssets = joinedAssets.Where(x => x!.RoomAsset.ToDate <= queryDto.ToDate);
        }

        if (queryDto.FromDate != null)
            joinedAssets = joinedAssets.Where(x => x!.RoomAsset.FromDate >= queryDto.FromDate);

        if (!string.IsNullOrEmpty(keyword))
        {
            joinedAssets = joinedAssets.Where(x => x!.Asset!.AssetCode!
                .ToLower().Equals(keyword) && x.Asset.AssetName.Contains(keyword));
        }

        if (queryDto.Status != null)
            joinedAssets = joinedAssets.Where(x => x!.Asset.Status == queryDto.Status);

        var totalCount = joinedAssets.Count();

        joinedAssets = joinedAssets.Skip(queryDto.Skip()).Take(queryDto.PageSize);

        var assets = await joinedAssets.Select(x => new RoomAssetDto
        {
            FromDate = x.RoomAsset.FromDate,
            ToDate = x.RoomAsset.ToDate,
            Id = x.RoomAsset.Id,
            Status = x.RoomAsset.Status,
            StatusObj = x.RoomAsset.Status.GetValue(),
            Quantity = x.RoomAsset.Quantity,
            CreatedAt = x.RoomAsset.CreatedAt,
            EditedAt = x.RoomAsset.EditedAt,
            CreatorId = x.RoomAsset.CreatorId ?? Guid.Empty,
            EditorId = x.RoomAsset.EditorId ?? Guid.Empty,
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
            }
        }).ToListAsync();

        assets = await _mapperRepository.MapCreator(assets);

        return ApiResponses<RoomAssetDto>.Success(
            assets,
            totalCount,
            queryDto.PageSize,
            queryDto.Skip(),
            (int)Math.Ceiling(totalCount / (double)queryDto.PageSize)
        );
    }

    public async Task<ApiResponse> DeleteAssets(List<Guid> ids)
    {
        var assets = await MainUnitOfWork.AssetRepository.FindAsync(new Expression<Func<Asset, bool>>[]
        {
            x => !x.DeletedAt.HasValue,
            x => ids.Contains(x.Id)
        }, null);

        if (!await MainUnitOfWork.AssetRepository.DeleteAsync(assets, AccountId, CurrentDate))
        {
            throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
        }
        
        return ApiResponse.Success();
    }

    public Room? GetWareHouse(string roomName)
    {
        var wareHouse = MainUnitOfWork.RoomRepository.GetQuery()
                        .Where(x => x!.RoomName!.Trim()
                        .Equals(roomName.Trim()))
                        .FirstOrDefault();
        return wareHouse;
    }
}
