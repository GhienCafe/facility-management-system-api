using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using API_FFMS.Repositories;

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
    Task<ApiResponses<AssetCheckTrackingDto>> AssetCheckTracking(Guid id, AssetTaskCheckQueryDto queryDto);
    Task<ApiResponses<AssetMaintenanceTrackingDto>> AssetMaintenanceTracking(Guid id, AssetTaskCheckQueryDto queryDto);
    Task<ApiResponses<AssetRepairationTrackingDto>> AssetRepairationTracking(Guid id, AssetTaskCheckQueryDto queryDto);
    Task<ApiResponses<AssetTransportationTrackingDto>> AssetTransportationTracking(Guid id, AssetTaskCheckQueryDto queryDto);
    Task<ApiResponses<AssetReplacementTrackingDto>> AssetReplacementTracking(Guid id, AssetTaskCheckQueryDto queryDto);
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
            throw new ApiException("Không tìm thấy trang thiết bị", StatusCode.NOT_FOUND);

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
            ImageUrl = x.Asset.ImageUrl,
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
                ImageUrl = x.Type.ImageUrl,
                EditedAt = x.Type.EditedAt,
                CreatorId = x.Type.CreatorId ?? Guid.Empty,
                EditorId = x.Type.EditorId ?? Guid.Empty
            } : null,
            Model = x.Model.ProjectTo<Model, ModelDto>(),
            Category = x.Type!.Category!.ProjectTo<Category, CategoryDto>()
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
                IsRented = x.IsRented,
                ManufacturingYear = x.ManufacturingYear,
                ModelId = x.ModelId,
                SerialNumber = x.SerialNumber,
                TypeId = x.TypeId,
                LastMaintenanceTime = x.LastMaintenanceTime,
                LastCheckedDate = x.LastCheckedDate,
                StartDateOfUse = x.StartDateOfUse,
                ImageUrl = x.ImageUrl,
                CreatedAt = x.CreatedAt,
                EditedAt = x.EditedAt,
                CreatorId = x.CreatorId ?? Guid.Empty,
                EditorId = x.EditorId ?? Guid.Empty,
                Type = x.Type!.ProjectTo<AssetType, AssetTypeDto>(),
                Model = x.Model!.ProjectTo<Model, ModelDto>(),
                Category = x.Type!.Category!.ProjectTo<Category, CategoryDto>()
            }).FirstOrDefaultAsync();

        if (asset == null)
            throw new ApiException("Không tìm thất trang thiết bị", StatusCode.NOT_FOUND);

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
        existingAsset.LastMaintenanceTime = updateDto.LastMaintenanceTime ?? existingAsset.LastMaintenanceTime;
        existingAsset.ImageUrl = updateDto.ImageUrl ?? existingAsset.ImageUrl;

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
                           join assetType in MainUnitOfWork.AssetTypeRepository.GetQuery() on asset.TypeId equals assetType.Id into assetTypeGroup
                           from assetType in assetTypeGroup.DefaultIfEmpty()
                           join category in MainUnitOfWork.CategoryRepository.GetQuery() on assetType.CategoryId equals category.Id into categoryGroup
                           from category in categoryGroup.DefaultIfEmpty()
                           join model in MainUnitOfWork.ModelRepository.GetQuery() on asset.ModelId equals model.Id
                             into modelGroup
                           from model in modelGroup.DefaultIfEmpty()
                           where roomAsset.RoomId == roomId
                           select new
                           {
                               RoomAsset = roomAsset,
                               Asset = asset,
                               Type = assetType,
                               Category = category,
                               Model = model
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
            Asset = new AssetDto
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
                ImageUrl = x.Asset.ImageUrl,
                TypeId = x.Asset.TypeId,
                ModelId = x.Asset.ModelId,
                CreatedAt = x.Asset.CreatedAt,
                EditedAt = x.Asset.EditedAt,
                CreatorId = x.Asset.CreatorId ?? Guid.Empty,
                EditorId = x.Asset.EditorId ?? Guid.Empty,
                Type = x.Type.ProjectTo<AssetType, AssetTypeDto>(),
                Category = x.Category.ProjectTo<Category, CategoryDto>(),
                Model = x.Model.ProjectTo<Model, ModelDto>()
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

    public async Task<ApiResponses<AssetCheckTrackingDto>> AssetCheckTracking(Guid id, AssetTaskCheckQueryDto queryDto)
    {
        var keyword = queryDto.Keyword?.Trim().ToLower();

        var asset = await MainUnitOfWork.AssetRepository.FindOneAsync(id);
        if (asset == null)
        {
            throw new ApiException("Không tìm thấy thiết bị", StatusCode.NOT_FOUND);
        }

        var assetCheckQuery = MainUnitOfWork.AssetCheckRepository.GetQueryAll().Where(x => x!.AssetId == id);

        if (keyword != null)
        {
            assetCheckQuery = assetCheckQuery.Where(x => x!.RequestCode.ToLower().Contains(keyword));
        }

        if (queryDto.AssignedTo != null)
        {
            assetCheckQuery = assetCheckQuery.Where(x => x!.AssignedTo == queryDto.AssignedTo);
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

        var totalCount = assetCheckQuery.Count();
        assetCheckQuery = assetCheckQuery.Skip(queryDto.Skip()).Take(queryDto.PageSize);

        var assetChecks = await assetCheckQuery.Select(x => new AssetCheckTrackingDto
        {
            Id = x!.Id,
            CreatedAt = x.CreatedAt,
            EditedAt = x.EditedAt,
            AssetId = x.AssetId,
            RequestCode = x.RequestCode,
            RequestDate = x.RequestDate,
            CompletionDate = x.CompletionDate,
            Status = x.Status,
            StatusObj = x.Status!.GetValue(),
            Description = x.Description,
            Notes = x.Notes,
            Result = x.Result,
            IsInternal = x.IsInternal,
            AssignedTo = x.AssignedTo,
            User = new UserDto
            {
                UserCode = x.User!.UserCode,
                Fullname = x.User.Fullname,
                Role = x.User.Role,
                RoleObj = x.User.Role.GetValue(),
                Avatar = x.User.Avatar,
                Email = x.User.Email,
                PhoneNumber = x.User.PhoneNumber,
                Address = x.User.Address,
                Gender = x.User.Gender,
                PersonalIdentifyNumber = x.User.PersonalIdentifyNumber,
                Dob = x.User.Dob
            }
        }).ToListAsync();

        assetChecks = await _mapperRepository.MapCreator(assetChecks);
        return ApiResponses<AssetCheckTrackingDto>.Success(
                assetChecks,
                totalCount,
                queryDto.PageSize,
                queryDto.Page,
                (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
    }

    public async Task<ApiResponses<AssetMaintenanceTrackingDto>> AssetMaintenanceTracking(Guid id, AssetTaskCheckQueryDto queryDto)
    {
        var keyword = queryDto.Keyword?.Trim().ToLower();

        var asset = await MainUnitOfWork.AssetRepository.FindOneAsync(id);
        if (asset == null)
        {
            throw new ApiException("Không tìm thấy thiết bị", StatusCode.NOT_FOUND);
        }

        var assetMaintenQuery = MainUnitOfWork.MaintenanceRepository.GetQueryAll().Where(x => x!.AssetId == id);

        if (keyword != null)
        {
            assetMaintenQuery = assetMaintenQuery.Where(x => x!.RequestCode.ToLower().Contains(keyword));
        }

        if (queryDto.AssignedTo != null)
        {
            assetMaintenQuery = assetMaintenQuery.Where(x => x!.AssignedTo == queryDto.AssignedTo);
        }

        if (queryDto.Status != null)
        {
            assetMaintenQuery = assetMaintenQuery.Where(x => x!.Status == queryDto.Status);
        }

        if (queryDto.RequestDate != null)
        {
            assetMaintenQuery = assetMaintenQuery.Where(x => x!.RequestDate == queryDto.RequestDate);
        }

        if (queryDto.CompletionDate != null)
        {
            assetMaintenQuery = assetMaintenQuery.Where(x => x!.CompletionDate == queryDto.CompletionDate);
        }

        var totalCount = assetMaintenQuery.Count();
        assetMaintenQuery = assetMaintenQuery.Skip(queryDto.Skip()).Take(queryDto.PageSize);

        var maintenances = await assetMaintenQuery.Select(x => new AssetMaintenanceTrackingDto
        {
            Id = x!.Id,
            CreatedAt = x.CreatedAt,
            EditedAt = x.EditedAt,
            AssetId = x.AssetId,
            RequestCode = x.RequestCode,
            RequestDate = x.RequestDate,
            CompletionDate = x.CompletionDate,
            Status = x.Status,
            StatusObj = x.Status!.GetValue(),
            Description = x.Description,
            Notes = x.Notes,
            Result = x.Result,
            IsInternal = x.IsInternal,
            AssignedTo = x.AssignedTo,
            User = new UserDto
            {
                UserCode = x.User!.UserCode,
                Fullname = x.User.Fullname,
                Role = x.User.Role,
                RoleObj = x.User.Role.GetValue(),
                Avatar = x.User.Avatar,
                Email = x.User.Email,
                PhoneNumber = x.User.PhoneNumber,
                Address = x.User.Address,
                Gender = x.User.Gender,
                PersonalIdentifyNumber = x.User.PersonalIdentifyNumber,
                Dob = x.User.Dob
            }
        }).ToListAsync();

        maintenances = await _mapperRepository.MapCreator(maintenances);
        return ApiResponses<AssetMaintenanceTrackingDto>.Success(
                maintenances,
                totalCount,
                queryDto.PageSize,
                queryDto.Page,
                (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
    }

    public async Task<ApiResponses<AssetRepairationTrackingDto>> AssetRepairationTracking(Guid id, AssetTaskCheckQueryDto queryDto)
    {
        var keyword = queryDto.Keyword?.Trim().ToLower();

        var asset = await MainUnitOfWork.AssetRepository.FindOneAsync(id);
        if (asset == null)
        {
            throw new ApiException("Không tìm thấy thiết bị", StatusCode.NOT_FOUND);
        }

        var assetRepairQuery = MainUnitOfWork.RepairationRepository.GetQueryAll().Where(x => x!.AssetId == id);

        if (keyword != null)
        {
            assetRepairQuery = assetRepairQuery.Where(x => x!.RequestCode.ToLower().Contains(keyword));
        }

        if (queryDto.AssignedTo != null)
        {
            assetRepairQuery = assetRepairQuery.Where(x => x!.AssignedTo == queryDto.AssignedTo);
        }

        if (queryDto.Status != null)
        {
            assetRepairQuery = assetRepairQuery.Where(x => x!.Status == queryDto.Status);
        }

        if (queryDto.RequestDate != null)
        {
            assetRepairQuery = assetRepairQuery.Where(x => x!.RequestDate == queryDto.RequestDate);
        }

        if (queryDto.CompletionDate != null)
        {
            assetRepairQuery = assetRepairQuery.Where(x => x!.CompletionDate == queryDto.CompletionDate);
        }

        var totalCount = assetRepairQuery.Count();
        assetRepairQuery = assetRepairQuery.Skip(queryDto.Skip()).Take(queryDto.PageSize);

        var repairations = await assetRepairQuery.Select(x => new AssetRepairationTrackingDto
        {
            Id = x!.Id,
            CreatedAt = x.CreatedAt,
            EditedAt = x.EditedAt,
            AssetId = x.AssetId,
            RequestCode = x.RequestCode,
            RequestDate = x.RequestDate,
            CompletionDate = x.CompletionDate,
            Status = x.Status,
            StatusObj = x.Status!.GetValue(),
            Description = x.Description,
            Notes = x.Notes,
            Result = x.Result,
            IsInternal = x.IsInternal,
            AssignedTo = x.AssignedTo,
            User = new UserDto
            {
                UserCode = x.User!.UserCode,
                Fullname = x.User.Fullname,
                Role = x.User.Role,
                RoleObj = x.User.Role.GetValue(),
                Avatar = x.User.Avatar,
                Email = x.User.Email,
                PhoneNumber = x.User.PhoneNumber,
                Address = x.User.Address,
                Gender = x.User.Gender,
                PersonalIdentifyNumber = x.User.PersonalIdentifyNumber,
                Dob = x.User.Dob
            }
        }).ToListAsync();

        repairations = await _mapperRepository.MapCreator(repairations);
        return ApiResponses<AssetRepairationTrackingDto>.Success(
                repairations,
                totalCount,
                queryDto.PageSize,
                queryDto.Page,
                (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
    }

    public async Task<ApiResponses<AssetTransportationTrackingDto>> AssetTransportationTracking(Guid id, AssetTaskCheckQueryDto queryDto)
    {
        var keyword = queryDto.Keyword?.Trim().ToLower();
        var asset = await MainUnitOfWork.AssetRepository.FindOneAsync(id);
        if (asset == null)
        {
            throw new ApiException("Không tìm thấy thiết bị", StatusCode.NOT_FOUND);
        }

        var roomDataset = MainUnitOfWork.RoomRepository.GetQuery();

        var transportDetails = MainUnitOfWork.TransportationDetailRepository.GetQueryAll();
        var transportDataSets = MainUnitOfWork.TransportationRepository.GetQueryAll();

        var joinedTransports = from transportDetail in transportDetails
                               where transportDetail.AssetId == id
                               join transportDataSet in transportDataSets on transportDetail.TransportationId equals transportDataSet.Id
                               select new
                               {
                                   Transport = transportDataSet
                               };

        if (keyword != null)
        {
            joinedTransports = joinedTransports.Where(x => x!.Transport.RequestCode.ToLower().Contains(keyword));
        }

        if (queryDto.AssignedTo != null)
        {
            joinedTransports = joinedTransports.Where(x => x!.Transport.AssignedTo == queryDto.AssignedTo);
        }

        if (queryDto.Status != null)
        {
            joinedTransports = joinedTransports.Where(x => x!.Transport.Status == queryDto.Status);
        }

        if (queryDto.RequestDate != null)
        {
            joinedTransports = joinedTransports.Where(x => x!.Transport.RequestDate == queryDto.RequestDate);
        }

        if (queryDto.CompletionDate != null)
        {
            joinedTransports = joinedTransports.Where(x => x!.Transport.CompletionDate == queryDto.CompletionDate);
        }

        var totalCount = await joinedTransports.CountAsync();
        joinedTransports = joinedTransports.Skip(queryDto.Skip()).Take(queryDto.PageSize);

        var transportations = await joinedTransports.Select(x => new AssetTransportationTrackingDto
        {
            Id = x!.Transport.Id,
            CreatedAt = x.Transport.CreatedAt,
            EditedAt = x.Transport.EditedAt,
            RequestCode = x.Transport.RequestCode,
            RequestDate = x.Transport.RequestDate,
            CompletionDate = x.Transport.CompletionDate,
            Status = x.Transport.Status,
            StatusObj = x.Transport.Status!.GetValue(),
            Description = x.Transport.Description,
            Notes = x.Transport.Notes,
            Result = x.Transport.Result,
            IsInternal = x.Transport.IsInternal,
            AssignedTo = x.Transport.AssignedTo,
            User = new UserDto
            {
                UserCode = x.Transport.User!.UserCode,
                Fullname = x.Transport.User.Fullname,
                Role = x.Transport.User.Role,
                RoleObj = x.Transport.User.Role.GetValue(),
                Avatar = x.Transport.User.Avatar,
                Email = x.Transport.User.Email,
                PhoneNumber = x.Transport.User.PhoneNumber,
                Address = x.Transport.User.Address,
                Gender = x.Transport.User.Gender,
                PersonalIdentifyNumber = x.Transport.User.PersonalIdentifyNumber,
                Dob = x.Transport.User.Dob
            },
            FromRoom = x.Transport.TransportationDetails!.Select(td => new RoomBaseDto
            {
                Id = td.Asset!.RoomAssets!.FirstOrDefault(ra => ra.AssetId == td.AssetId)!.Room!.Id,
                RoomCode = td.Asset.RoomAssets!.FirstOrDefault(ra => ra.AssetId == td.AssetId)!.Room!.RoomCode,
                RoomName = td.Asset.RoomAssets!.FirstOrDefault(ra => ra.AssetId == td.AssetId)!.Room!.RoomName,
                StatusId = td.Asset.RoomAssets!.FirstOrDefault(ra => ra.AssetId == td.AssetId)!.Room!.StatusId,
                FloorId = td.Asset.RoomAssets!.FirstOrDefault(ra => ra.AssetId == td.AssetId)!.Room!.FloorId,
                CreatedAt = td.Asset.RoomAssets!.FirstOrDefault(ra => ra.AssetId == td.AssetId)!.Room!.CreatedAt,
                EditedAt = td.Asset.RoomAssets!.FirstOrDefault(ra => ra.AssetId == td.AssetId)!.Room!.EditedAt
            }).FirstOrDefault(),
            ToRoom = new RoomBaseDto
            {
                Id = x.Transport.ToRoom!.Id,
                RoomCode = x.Transport.ToRoom.RoomCode,
                RoomName = x.Transport.ToRoom.RoomName,
                StatusId = x.Transport.ToRoom.StatusId,
                FloorId = x.Transport.ToRoom.FloorId,
                CreatedAt = x.Transport.ToRoom.CreatedAt,
                EditedAt = x.Transport.ToRoom.EditedAt
            }
        }).ToListAsync();

        transportations = await _mapperRepository.MapCreator(transportations);
        return ApiResponses<AssetTransportationTrackingDto>.Success(
                transportations,
                totalCount,
                queryDto.PageSize,
                queryDto.Page,
                (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
    }

    public async Task<ApiResponses<AssetReplacementTrackingDto>> AssetReplacementTracking(Guid id, AssetTaskCheckQueryDto queryDto)
    {
        var keyword = queryDto.Keyword?.Trim().ToLower();

        var existingAsset = await MainUnitOfWork.AssetRepository.FindOneAsync(id);
        if (existingAsset == null)
        {
            throw new ApiException("Không tìm thấy thiết bị", StatusCode.NOT_FOUND);
        }

        var assetReplaceQuery = MainUnitOfWork.ReplacementRepository.GetQueryAll().Where(x => x!.AssetId == id);

        if (keyword != null)
        {
            assetReplaceQuery = assetReplaceQuery.Where(x => x!.RequestCode.ToLower().Contains(keyword));
        }

        if (queryDto.AssignedTo != null)
        {
            assetReplaceQuery = assetReplaceQuery.Where(x => x!.AssignedTo == queryDto.AssignedTo);
        }

        if (queryDto.Status != null)
        {
            assetReplaceQuery = assetReplaceQuery.Where(x => x!.Status == queryDto.Status);
        }

        if (queryDto.RequestDate != null)
        {
            assetReplaceQuery = assetReplaceQuery.Where(x => x!.RequestDate == queryDto.RequestDate);
        }

        if (queryDto.CompletionDate != null)
        {
            assetReplaceQuery = assetReplaceQuery.Where(x => x!.CompletionDate == queryDto.CompletionDate);
        }

        var joinedReplaces = from replace in assetReplaceQuery
                             join replacedBy in MainUnitOfWork.AssetRepository.GetQuery() on replace.NewAssetId equals replacedBy.Id
                             //join asset in MainUnitOfWork.AssetRepository.GetQuery() on replace.AssetId equals id
                             join assetType in MainUnitOfWork.AssetTypeRepository.GetQuery() on existingAsset.TypeId equals assetType.Id into assetTypeGroup
                             from assetType in assetTypeGroup.DefaultIfEmpty()
                             join category in MainUnitOfWork.CategoryRepository.GetQuery() on assetType.CategoryId equals category.Id into categoryGroup
                             from category in categoryGroup.DefaultIfEmpty()
                             select new
                             {
                                 Replacement = replace,
                                 ReplacedBy = replacedBy,
                                 AssetType = assetType,
                                 Category = category
                             };

        var totalCount = await joinedReplaces.CountAsync();
        joinedReplaces = joinedReplaces.Skip(queryDto.Skip()).Take(queryDto.PageSize);

        var replacements = await joinedReplaces.Select(x => new AssetReplacementTrackingDto
        {
            Id = x.Replacement.Id,
            RequestCode = x.Replacement.RequestCode,
            RequestDate = x.Replacement.RequestDate,
            CompletionDate = x.Replacement.CompletionDate,
            Status = x.Replacement.Status,
            StatusObj = x.Replacement.Status!.GetValue(),
            Description = x.Replacement.Description,
            Notes = x.Replacement.Notes,
            IsInternal = x.Replacement.IsInternal,
            AssignedTo = x.Replacement.AssignedTo,
            CreatedAt = x.Replacement.CreatedAt,
            EditedAt = x.Replacement.EditedAt,
            CreatorId = x.Replacement.CreatorId ?? Guid.Empty,
            EditorId = x.Replacement.EditorId ?? Guid.Empty,
            AssignTo = new UserDto
            {
                UserCode = x.Replacement.User!.UserCode,
                Fullname = x.Replacement.User.Fullname,
                Role = x.Replacement.User.Role,
                RoleObj = x.Replacement.User.Role.GetValue(),
                Avatar = x.Replacement.User.Avatar,
                Email = x.Replacement.User.Email,
                PhoneNumber = x.Replacement.User.PhoneNumber,
                Address = x.Replacement.User.Address,
                Gender = x.Replacement.User.Gender,
                PersonalIdentifyNumber = x.Replacement.User.PersonalIdentifyNumber,
                Dob = x.Replacement.User.Dob
            },
            ReplacedBy = new AssetBaseDto
            {
                Id = x.ReplacedBy.Id,
                AssetName = x.ReplacedBy.AssetName,
                AssetCode = x.ReplacedBy.AssetCode,
                IsMovable = x.ReplacedBy.IsMovable,
                ManufacturingYear = x.ReplacedBy.ManufacturingYear,
                SerialNumber = x.ReplacedBy.SerialNumber,
                Description = x.ReplacedBy.Description,
                TypeId = x.ReplacedBy.TypeId,
                ModelId = x.ReplacedBy.ModelId,
                IsRented = x.ReplacedBy.IsRented,
                StartDateOfUse = x.ReplacedBy.StartDateOfUse
            },
            AssetType = x.AssetType.ProjectTo<AssetType, AssetTypeDto>(),
            Category = x.Category.ProjectTo<Category, CategoryDto>()
        }).ToListAsync();

        replacements = await _mapperRepository.MapCreator(replacements);

        return ApiResponses<AssetReplacementTrackingDto>.Success(
                replacements,
                totalCount,
                queryDto.PageSize,
                queryDto.Page,
                (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
    }
}
