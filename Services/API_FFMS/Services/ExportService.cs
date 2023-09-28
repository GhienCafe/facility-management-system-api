using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Services
{
    public interface IExportService : IBaseService
    {
        Task<ApiResponse<Stream>> ExportCombinedData(ExportQueryTrackingDto queryDto);
    }

    public class ExportService : BaseService, IExportService
    {
        public ExportService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor,
            IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
        }

        public async Task<ApiResponse<Stream>> ExportCombinedData(ExportQueryTrackingDto queryDto)
        {
            // Retrieve data from repositories (RoomAsset, Room, and Asset)
            var roomAssetData = await MainUnitOfWork.RoomAssetRepository.GetQuery().Where(x => !x.DeletedAt.HasValue)
                .ToListAsync();
            var roomData = await MainUnitOfWork.RoomRepository.GetQuery().Where(x => !x.DeletedAt.HasValue)
                .ToListAsync();
            var assetData = await MainUnitOfWork.AssetRepository.GetQuery().Where(x => !x.DeletedAt.HasValue)
                .ToListAsync();

            var combinedData = new List<ExportTrackingDto>();
            foreach (var roomAsset in roomAssetData)
            {
                if (roomAsset != null)
                {
                    var room = roomData.FirstOrDefault(r => r.Id == roomAsset.RoomId);
                    var asset = assetData.FirstOrDefault(a => a.Id == roomAsset.Asset.Id);

                    if (room != null && asset != null)
                    {
                        var roomStatus = await GetRoomStatusFromRoom(room);

                        var roomType = await GetRoomTypeFromRoom(room);

                        var exportTracking = new ExportTrackingDto
                        {
                            FromDateTracking = roomAsset.FromDate??DateTime.MinValue,
                            ToDateTracking = roomAsset.ToDate,
                            AssetId = asset.Id,
                            TypeId = asset.TypeId ?? Guid.Empty,
                            AssetName = asset.AssetName,
                            AssetCode = asset.AssetCode,
                            IsMovable = asset.IsMovable,
                            AssetStatus = asset.Status,
                            ManufacturingYear = asset.ManufacturingYear,
                            SerialNumber = asset.SerialNumber,
                            Quantity = asset.Quantity,
                            Description = asset.Description,
                            LastMaintenanceTime = asset.LastMaintenanceTime,
                            RoomName = room.RoomName,
                            Area = room.Area,
                            PathRoom = room.PathRoom,
                            RoomCode = room.RoomCode,
                            RoomTypeId = room.RoomTypeId,
                            Capacity = room.Capacity,
                            StatusId = room.StatusId,
                            FloorId = room.FloorId,
                            RoomStatus = roomStatus,
                            RoomType = roomType
                        };

                        combinedData.Add(exportTracking);
                    }
                }
            }

            var exportStream =
                ExcelExtension<ExportTrackingDto>.ExportV2(combinedData, "CombinedData", "Danh sách kết hợp", 6);

            return ApiResponse<Stream>.Success(exportStream);
        }
        private async Task<RoomStatusDto> GetRoomStatusFromRoom(Room room)
        {
            var roomStatus = await MainUnitOfWork.RoomStatusRepository.GetQuery()
                .Where(x => !x!.DeletedAt.HasValue && x.Id == room.StatusId)
                .SingleOrDefaultAsync();
        
            return new RoomStatusDto
            {
                StatusName = roomStatus!.StatusName,
                Description = roomStatus.Description,
                Color = roomStatus.Color
            };
        }

        private async Task<RoomTypeDto> GetRoomTypeFromRoom(Room room)
        {
            var roomType = await MainUnitOfWork.RoomTypeRepository.GetQuery()
                .Where(x => !x!.DeletedAt.HasValue && x.Id == room.RoomTypeId)
                .SingleOrDefaultAsync();

            return new RoomTypeDto
            {
                TypeName = roomType!.TypeName,
                Description = roomType.Description
            };
        }
    }
}
