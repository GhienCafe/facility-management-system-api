// using API_FFMS.Dtos;
// using AppCore.Extensions;
// using AppCore.Models;
// using MainData;
// using MainData.Entities;
// using MainData.Repositories;
// using Microsoft.EntityFrameworkCore;
//
// namespace API_FFMS.Services;
//
// public interface IAssetCheckService : IBaseService
// {
//     Task<ApiResponses<AssetCheckDto>> GetAssetChecks(AssetCheckQueryDto queryDto);
// }
//
// public class AssetCheckService : BaseService, IAssetCheckService
// {
//     public AssetCheckService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
//     {
//     }
//
//     public async Task<ApiResponses<AssetCheckDto>> GetAssetChecks(AssetCheckQueryDto queryDto)
//     {
//         var assetCheckQueryable = MainUnitOfWork.AssetCheckRepository.GetQuery()
//             .Where(x => !x!.DeletedAt.HasValue);
//         
//         if (queryDto.AssetId != null)
//         {
//             assetCheckQueryable = assetCheckQueryable.Where(x => x!.AssetId == queryDto.AssetId);
//         }
//         
//         if (queryDto.RequestId != null)
//         {
//             assetCheckQueryable = assetCheckQueryable.Where(x => x!.RequestId == queryDto.RequestId);
//         }
//         
//         // if (queryDto.IsVerified != null)
//         // {
//         //     assetCheckQueryable = assetCheckQueryable.Where(x => x!.IsVerified == queryDto.IsVerified);
//         // }
//         
//         var joinTables = from assetCheck in assetCheckQueryable
//             join asset in MainUnitOfWork.AssetRepository.GetQuery() on assetCheck.AssetId equals asset.Id into
//                 assetGroup
//             from asset in assetGroup.DefaultIfEmpty()
//             join assetRoom in MainUnitOfWork.RoomAssetRepository.GetQuery() on asset.Id equals assetRoom.AssetId into
//                 assetRoomGroup
//             from assetRoom in assetRoomGroup.DefaultIfEmpty()
//             where assetRoom.ToDate == null
//             join location in MainUnitOfWork.RoomRepository.GetQuery() on assetRoom.RoomId equals location.Id into
//                 locationGroup
//             from location in locationGroup.DefaultIfEmpty()
//             select new
//             {
//                 AssetCheck = assetCheck,
//                 Asset = asset,
//                 location = location
//             };
//         
//         var totalCount = await joinTables.CountAsync();
//         
//         joinTables = joinTables.Skip(queryDto.Skip()).Take(queryDto.PageSize);
//         
//         var assetChecks = await joinTables.Select(x => new AssetCheckDto
//         {
//             RequestId = x.AssetCheck.RequestId,
//             IsVerified = x.AssetCheck.IsVerified,
//             AssetId = x.AssetCheck.AssetId,
//             Asset = new AssetBaseDto
//             {
//                 Id = x.Asset.Id,
//                 Description = x.Asset.Description,
//                 AssetCode = x.Asset.AssetCode,
//                 AssetName = x.Asset.AssetName,
//                 Quantity = x.Asset.Quantity,
//                 IsMovable = x.Asset.IsMovable,
//                 IsRented = x.Asset.IsRented,
//                 ManufacturingYear = x.Asset.ManufacturingYear,
//                 StatusObj = x.Asset.Status.GetValue(),
//                 Status = x.Asset.Status,
//                 StartDateOfUse = x.Asset.StartDateOfUse,
//                 SerialNumber = x.Asset.SerialNumber,
//                 LastCheckedDate = x.Asset.LastCheckedDate,
//                 LastMaintenanceTime = x.Asset.LastMaintenanceTime,
//                 TypeId = x.Asset.TypeId,
//                 ModelId = x.Asset.ModelId,
//                 CreatedAt = x.Asset.CreatedAt,
//                 EditedAt = x.Asset.EditedAt,
//                 CreatorId = x.Asset.CreatorId ?? Guid.Empty,
//                 EditorId = x.Asset.EditorId ?? Guid.Empty
//             },
//             Location = new RoomBaseDto
//             {
//                 Area = x.location.Area,
//                 Capacity = x.location.Capacity,
//                 Id = x.location.Id,
//                 FloorId = x.location.FloorId,
//                 RoomName = x.location.RoomName,
//                 Description = x.location.Description,
//                 RoomTypeId = x.location.RoomTypeId,
//                 CreatedAt = x.location.CreatedAt,
//                 EditedAt = x.location.EditedAt,
//                 RoomCode = x.location.RoomCode,
//                 StatusId = x.location.StatusId,
//                 CreatorId = x.location.CreatorId ?? Guid.Empty,
//                 EditorId = x.location.EditorId ?? Guid.Empty
//             }
//         }).ToListAsync();
//         
//         assetChecks = await _mapperRepository.MapCreator(assetChecks);
//         
//         // Return data
//         return ApiResponses<AssetCheckDto>.Success(
//             assetChecks,
//             totalCount,
//             queryDto.PageSize,
//             queryDto.Page,
//             (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
//     }
// }