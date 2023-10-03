using System.Diagnostics;
using API_FFMS.Dtos;
using AppCore.Models;
using MainData;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;
using AppCore.Extensions;

namespace API_FFMS.Services
{
    public interface IExportService : IBaseService
    {
        public Task<ExportFile> Export(QueryRoomExportDto queryDto);
        public Task<ExportFile> Export(QueryAssetExportDto queryDto);
    }

    public class ExportService : BaseService, IExportService
    {
        public ExportService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor,
            IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
        }

        public async Task<ExportFile> Export(QueryRoomExportDto queryDto)
        {
            var roomQuery = MainUnitOfWork.RoomRepository.GetQuery();

            if (queryDto.RoomTypeId != null)
            {
                roomQuery = roomQuery.Where(x => x!.RoomTypeId == queryDto.RoomTypeId);
            }

            if (!string.IsNullOrEmpty(queryDto.Keyword))
            {
                var keyword = queryDto.Keyword.ToLower();
                roomQuery = roomQuery.Where(x => x!.RoomCode.ToLower().Contains(keyword)
                                                 || x.RoomName!.ToLower().Contains(keyword) ||
                                                 x.Description!.ToLower().Contains(keyword));
            }

            if (queryDto.StatusId != null)
            {
                roomQuery = roomQuery.Where(x => x!.StatusId == queryDto.StatusId);
            }

            if (queryDto.FromArea != null)
            {
                roomQuery = roomQuery.Where(x => x!.Area >= queryDto.FromArea);
            }

            if (queryDto.ToArea != null)
            {
                roomQuery = roomQuery.Where(x => x!.Area <= queryDto.ToArea);
            }

            if (queryDto.FromCapacity != null)
            {
                roomQuery = roomQuery.Where(x => x!.Capacity >= queryDto.FromCapacity);
            }

            if (queryDto.ToCapacity != null)
            {
                roomQuery = roomQuery.Where(x => x!.Capacity <= queryDto.ToCapacity);
            }

            if (queryDto.FloorId != null)
            {
                roomQuery = roomQuery.Where(x => x!.FloorId == queryDto.FloorId);
            }

            if (queryDto.AssetStatus != null)
            {
                roomQuery = roomQuery.Where(x =>
                    x!.RoomAssets!.Any(ra => ra.Asset!.Status == queryDto.AssetStatus.Value));
            }

            var rooms = await roomQuery.ToListAsync();

            var exportTrackingData = rooms.Select(room =>
            {
                Debug.Assert(room.Status != null, "room.Status != null");
                return new ExportTrackingRoomDto
                {
                    RoomName = room!.RoomName,
                    Area = room.Area,
                    PathRoom = room.PathRoom,
                    RoomCode = room.RoomCode,
                    Capacity = room.Capacity,
                    Description = room.Description,

                    TypeName = room.RoomTypeId != null
                        ? MainUnitOfWork.RoomTypeRepository.GetQuery()
                            .SingleOrDefault(x => !x!.DeletedAt.HasValue && x.Id == room.RoomTypeId)!
                            .TypeName
                        : null,
                    StatusName = MainUnitOfWork.RoomStatusRepository.GetQuery()
                        .SingleOrDefault(x => !x!.DeletedAt.HasValue && x.Id == room.StatusId)!
                        .StatusName,
                };
            }).ToList();

            var streamFile = ExportHelperList<ExportTrackingRoomDto>.Export(
                exportTrackingData,
                "Room",
                "Room Export",
                6);

            return new ExportFile
            {
                FileName = $"Room",
                Stream = streamFile
            };
        }

        public async Task<ExportFile> Export(QueryAssetExportDto queryDto)
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

            var assetData = await assetDataSet.ToListAsync();
            var assets =  assetData.Select(x => new AssetExportDto
            {
                Description = x.Description,
                Status = x.Status,
                StatusObj = x.Status.GetValue(),
                LastCheckedDate = x.LastCheckedDate,
                StartDateOfUse = x.StartDateOfUse,
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
                CreatedAt = x.CreatedAt,
                EditedAt = x.EditedAt,
                CreatorId = x.CreatorId ?? Guid.Empty,
                EditorId = x.EditorId ?? Guid.Empty,
                TypeName=  x.TypeId != null ? MainUnitOfWork.AssetTypeRepository.GetQuery().SingleOrDefault(type=> !type!.DeletedAt.HasValue && type.Id == x.TypeId )!.TypeName : null,
                ModelName=  x.ModelId != null ? MainUnitOfWork.ModelRepository.GetQuery().SingleOrDefault(model=> !model!.DeletedAt.HasValue && model.Id == x.ModelId )!.ModelName : null
            }).ToList();

            assets = await _mapperRepository.MapCreator(assets);
            var streamFile = ExportHelperList<AssetExportDto>.Export(
                assets,
                "Asset",
                "Asset Export",
                6);

            return new ExportFile
            {
                FileName = $"Asset",
                Stream = streamFile
            };
        }
    }
}
