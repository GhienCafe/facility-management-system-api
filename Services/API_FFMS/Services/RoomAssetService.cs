using API_FFMS.Dtos;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using System.Linq.Expressions;
using AppCore.Extensions;
using Microsoft.EntityFrameworkCore;
using API_FFMS.Repositories;

namespace API_FFMS.Services
{
    public interface IRoomAssetService : IBaseService
    {
        Task<ApiResponses<AssetTrackingDto>> AssetUsedTracking(Guid id, RoomTrackingQueryDto queryDto);
        Task<ApiResponse> CreateRoomAsset(RoomAssetCreateBaseDto createBaseDto);
        Task<ApiResponses<RoomAssetBaseDto>> GetItems(RoomAssetQueryDto queryDto);
        Task<ApiResponse<RoomAssetDetailDto>> GetItem(Guid id);
        Task<ApiResponse> DeleteItem(Guid id);
        Task<ApiResponse> UpdateItem(Guid id, RoomAssetUpdateBaseDto updateBaseDto);

        Task<ApiResponse> CreateRoomAssets(RoomAssetMultiCreateBaseDto createBaseDto);
    }

    public class RoomAssetService : BaseService, IRoomAssetService
    {
        private readonly IRoomAssetRepository _roomAssetRepository;
        public RoomAssetService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor,
                                IMapperRepository mapperRepository, IRoomAssetRepository roomAssetRepository)
                                : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
            _roomAssetRepository = roomAssetRepository;
        }

        public async Task<ApiResponses<AssetTrackingDto>> AssetUsedTracking(Guid id, RoomTrackingQueryDto queryDto)
        {
            var keyword = queryDto.Keyword?.Trim().ToLower();
            var asset = await MainUnitOfWork.AssetRepository.FindOneAsync<AssetDto>(new Expression<Func<Asset, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.Id == id
            });
            if (asset == null)
            {
                throw new ApiException("Không tìm thấy thiết bị", StatusCode.NOT_FOUND);
            }

            var roomAssetDataset = MainUnitOfWork.RoomAssetRepository.GetQuery();
            var roomDataset = MainUnitOfWork.RoomRepository.GetQuery();

            var joinedRooms = from roomAsset in roomAssetDataset
                              where roomAsset.AssetId == id
                              join room in roomDataset on roomAsset.RoomId equals room.Id
                              select new
                              {
                                  RoomAsset = roomAsset,
                                  Room = room,
                              };

            if (queryDto.ToDate != null)
            {
                joinedRooms = joinedRooms.Where(x => x!.RoomAsset.ToDate <= queryDto.ToDate);
            }

            if (queryDto.FromDate != null)
            {
                joinedRooms = joinedRooms.Where(x => x!.RoomAsset.FromDate >= queryDto.FromDate);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                joinedRooms = joinedRooms.Where(x => x!.Room!.RoomCode!
                    .ToLower().Equals(keyword) && x.Room.RoomName!.Contains(keyword));
            }

            var totalCount = joinedRooms.Count();

            joinedRooms = joinedRooms.Skip(queryDto.Skip()).Take(queryDto.PageSize);

            var rooms = await joinedRooms.Select(x => new AssetTrackingDto
            {
                FromDate = x.RoomAsset.FromDate,
                ToDate = x.RoomAsset.ToDate,
                Id = x.RoomAsset.Id,
                Status = x.RoomAsset.Status,
                CreatedAt = x.RoomAsset.CreatedAt,
                EditedAt = x.RoomAsset.EditedAt,
                CreatorId = x.RoomAsset.CreatorId ?? Guid.Empty,
                EditorId = x.RoomAsset.EditorId ?? Guid.Empty,
                Room = new RoomBaseDto
                {
                    Id = x.Room.Id,
                    RoomName = x.Room.RoomName,
                    RoomCode = x.Room.RoomCode,
                    Area = x.Room.Area,
                    Description = x.Room.Description,
                    RoomTypeId = x.Room.RoomTypeId,
                    Capacity = x.Room.Capacity,
                    StatusId = x.Room.StatusId,
                    FloorId = x.Room.FloorId,
                    CreatedAt = x.Room.CreatedAt,
                    EditedAt = x.Room.EditedAt,
                    CreatorId = x.Room.CreatorId ?? Guid.Empty,
                    EditorId = x.Room.EditorId ?? Guid.Empty
                }
            }).ToListAsync();

            rooms = await _mapperRepository.MapCreator(rooms);

            return ApiResponses<AssetTrackingDto>.Success(
                rooms,
                totalCount,
                queryDto.PageSize,
                queryDto.Page,
                (int)Math.Ceiling(totalCount / (double)queryDto.PageSize)
            );
        }

        public async Task<ApiResponse> CreateRoomAsset(RoomAssetCreateBaseDto createBaseDto)
        {
            var room = await MainUnitOfWork.RoomRepository.FindOneAsync(createBaseDto.RoomId);
            if (room == null)
            {
                throw new ApiException("Phòng không tồn tại", StatusCode.NOT_FOUND);
            }
            var asset = MainUnitOfWork.AssetRepository.GetQuery()
                                                      .Include(x => x!.Type)
                                                      .FirstOrDefault(x => x!.Id == createBaseDto.AssetId);
            if (asset == null)
            {
                throw new ApiException("Thiết bị không tồn tại", StatusCode.NOT_FOUND);
            }

            var checkExist = await MainUnitOfWork.RoomAssetRepository.FindOneAsync(
                new Expression<Func<RoomAsset, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.AssetId == createBaseDto.AssetId,
                    x => x.RoomId == createBaseDto.RoomId,
                    x => x.ToDate == null
                });

            if (asset.Type!.Unit == Unit.Individual || asset.Type.IsIdentified == true)
            {
                if (checkExist != null)
                {
                    throw new ApiException("Đã tồn tại trang thiết bị trong phòng", StatusCode.ALREADY_EXISTS);
                }
            }

            var roomAssets = await MainUnitOfWork.RoomAssetRepository.FindAsync(
                new Expression<Func<RoomAsset, bool>>[]
                {
                        x => !x.DeletedAt.HasValue,
                        x => x.RoomId == room.Id,
                        x => x.ToDate == null
                }, null);

            var currentQuantityAssetInRoom = roomAssets.Sum(x => x!.Quantity);
            var checkCapacity = currentQuantityAssetInRoom + createBaseDto.Quantity;
            if (checkCapacity > room.Capacity)
            {
                throw new ApiException("Số lượng trang thiết bị vượt quá dung tích phòng", StatusCode.UNPROCESSABLE_ENTITY);
            }

            var roomAsset = createBaseDto.ProjectTo<RoomAssetCreateBaseDto, RoomAsset>();

            if (!await _roomAssetRepository.AddAssetToRoom(roomAsset, AccountId, CurrentDate))
                throw new ApiException("Thêm mới trang thiết bị vào phòng thất bại", StatusCode.SERVER_ERROR);

            return ApiResponse.Created("Thêm mới trang thiết bị vào phòng thành công");
        }

        public async Task<ApiResponse> CreateRoomAssets(RoomAssetMultiCreateBaseDto createBaseDto)
        {
            //var room = await MainUnitOfWork.RoomRepository.FindOneAsync(createBaseDto.RoomId);

            var roomAssets = new List<RoomAsset>();
            foreach (var room in createBaseDto.Rooms)
            {
                foreach (var asset in room.Assets)
                {
                    var roomAsset = new RoomAsset
                    {
                        RoomId = room.RoomId,
                        AssetId = asset.AssetId,
                        Quantity = asset.Quantity
                    };
                    roomAssets.Add(roomAsset);
                }
            }

            if (!await _roomAssetRepository.AddAssetToRooms(roomAssets, AccountId, CurrentDate))
                throw new ApiException("Thêm mới trang thiết bị vào phòng thất bại", StatusCode.SERVER_ERROR);

            return ApiResponse.Created("Thêm mới trang thiết bị vào phòng thành công");
        }

        public async Task<ApiResponses<RoomAssetBaseDto>> GetItems(RoomAssetQueryDto queryDto)
        {
            try
            {
                var keyword = queryDto.Keyword?.Trim().ToLower();

                var roomAssetDataset = MainUnitOfWork.RoomAssetRepository.GetQuery()
                    .Where(x => !x!.DeletedAt.HasValue);
                var roomDataset = MainUnitOfWork.RoomRepository.GetQuery()
                    .Where(x => !x!.DeletedAt.HasValue);
                var assetDataset = MainUnitOfWork.AssetRepository.GetQuery()
                    .Where(x => !x!.DeletedAt.HasValue);

                var joinedRooms = from roomAsset in roomAssetDataset
                                  join room in roomDataset on roomAsset.RoomId equals room.Id into roomGroup
                                  from room in roomGroup.DefaultIfEmpty()
                                  join assets in assetDataset on roomAsset.AssetId equals assets.Id into assetGroup
                                  from assets in assetGroup.DefaultIfEmpty()
                                  select new
                                  {
                                      RoomAsset = roomAsset,
                                      Room = room,
                                      Asset = assets
                                  };

                if (!string.IsNullOrEmpty(keyword))
                {
                    joinedRooms = joinedRooms.Where(x => x.Asset.AssetCode!.ToLower().Contains(keyword)
                                                         || x.Asset.AssetName.ToLower().Contains(keyword)
                                                         || x.Room.RoomName!.ToLower().Contains(keyword)
                                                         || x.Room.RoomCode.ToLower().Contains(keyword));
                }

                if (queryDto.RoomId != null)
                {
                    joinedRooms = joinedRooms.Where(x => x!.RoomAsset.RoomId == queryDto.RoomId);
                }

                if (queryDto.AssetId != null)
                {
                    joinedRooms = joinedRooms.Where(x => x!.RoomAsset.AssetId == queryDto.AssetId);
                }

                if (queryDto.IsInCurrent != null)
                {
                    joinedRooms = joinedRooms.Where(x => x!.RoomAsset.ToDate == null);
                }
                else if (queryDto.ToDate != null)
                {
                    joinedRooms = joinedRooms.Where(x => x!.RoomAsset.ToDate <= queryDto.ToDate);
                }

                if (queryDto.FromDate != null)
                {
                    joinedRooms = joinedRooms.Where(x => x!.RoomAsset.FromDate >= queryDto.FromDate);
                }

                if (queryDto.Status != null)
                {
                    joinedRooms = joinedRooms.Where(x => x.RoomAsset.Status == queryDto.Status);
                }

                var totalCount = await joinedRooms.CountAsync();

                joinedRooms = joinedRooms.Skip(queryDto.Skip()).Take(queryDto.PageSize);

                var items = await joinedRooms.Select(x => new RoomAssetBaseDto
                {
                    RoomId = x.RoomAsset.RoomId,
                    AssetId = x.RoomAsset.AssetId,
                    FromDate = x.RoomAsset.FromDate,
                    ToDate = x.RoomAsset.ToDate,
                    Id = x.RoomAsset.Id,
                    Status = x.RoomAsset.Status,
                    CreatedAt = x.RoomAsset.CreatedAt,
                    EditedAt = x.RoomAsset.EditedAt,
                    CreatorId = x.RoomAsset.CreatorId ?? Guid.Empty,
                    EditorId = x.RoomAsset.EditorId ?? Guid.Empty,
                    Room = x.Room.ProjectTo<Room, RoomBaseDto>(),
                    Asset = new AssetBaseDto
                    {
                        Id = x.Asset.Id,
                        AssetName = x.Asset.AssetName,
                        AssetCode = x.Asset.AssetCode,
                        IsMovable = x.Asset.IsMovable,
                        Status = x.Asset.Status,
                        StatusObj = x.Asset.Status.GetValue(),
                        ManufacturingYear = x.Asset.ManufacturingYear,
                        SerialNumber = x.Asset.SerialNumber,
                        Quantity = x.RoomAsset.Quantity,
                        Description = x.Asset.Description,
                        TypeId = x.Asset.TypeId,
                        ModelId = x.Asset.ModelId,
                        IsRented = x.Asset.IsRented,
                        StartDateOfUse = x.Asset.StartDateOfUse
                    }
                }).ToListAsync();

                items.ForEach(x =>
                {
                    if (x.Asset != null)
                    {
                        x.Asset.StatusObj = x.Asset.Status?.GetValue();
                    }
                });

                items = await _mapperRepository.MapCreator(items);
                // Return data
                return ApiResponses<RoomAssetBaseDto>.Success(
                    items,
                    totalCount,
                    queryDto.PageSize,
                    queryDto.Page,
                    (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        public async Task<ApiResponse<RoomAssetDetailDto>> GetItem(Guid id)
        {
            var roomAsset = await MainUnitOfWork.RoomAssetRepository.FindOneAsync(id);

            if (roomAsset == null)
                throw new ApiException("Không tìm thấy thông tin", StatusCode.NOT_FOUND);

            var roomAssetDto = roomAsset.ProjectTo<RoomAsset, RoomAssetDetailDto>();

            var asset = await MainUnitOfWork.AssetRepository.FindOneAsync(roomAsset.AssetId);
            roomAssetDto.Asset = new AssetBaseDto
            {
                Id = asset!.Id,
                AssetName = asset.AssetName,
                AssetCode = asset.AssetCode,
                IsMovable = asset.IsMovable,
                Status = roomAsset.Status,
                StatusObj = roomAsset.Status.GetValue(),
                ManufacturingYear = asset.ManufacturingYear,
                SerialNumber = asset.SerialNumber,
                Quantity = roomAsset.Quantity,
                Description = asset.Description,
                TypeId = asset.TypeId,
                ModelId = asset.ModelId,
                IsRented = asset.IsRented,
                StartDateOfUse = asset.StartDateOfUse
            };

            roomAssetDto.Room = (await MainUnitOfWork.RoomRepository.FindOneAsync(roomAsset.RoomId))?
                .ProjectTo<Room, RoomBaseDto>();

            if (roomAssetDto.Asset != null)
            {
                roomAssetDto.Asset.StatusObj = roomAsset.Asset?.Status.GetValue();
            }

            roomAssetDto = await _mapperRepository.MapCreator(roomAssetDto);

            return ApiResponse<RoomAssetDetailDto>.Success(roomAssetDto);
        }

        public async Task<ApiResponse> DeleteItem(Guid id)
        {
            var roomAsset = await MainUnitOfWork.RoomAssetRepository.FindOneAsync(id);

            if (roomAsset == null)
                throw new ApiException("Không tìm thấy thông tin", StatusCode.NOT_FOUND);

            if (!await MainUnitOfWork.RoomAssetRepository.DeleteAsync(roomAsset, AccountId, CurrentDate))
                throw new ApiException("Xóa nội dung thất bại", StatusCode.SERVER_ERROR);

            return ApiResponse.Success("Xóa nội dung thành công");
        }

        public async Task<ApiResponse> UpdateItem(Guid id, RoomAssetUpdateBaseDto updateBaseDto)
        {
            var roomAsset = await MainUnitOfWork.RoomAssetRepository.FindOneAsync(id);

            if (roomAsset == null)
                throw new ApiException("Không tìm thấy thông tin", StatusCode.NOT_FOUND);

            roomAsset.AssetId = updateBaseDto.AssetId ?? roomAsset.AssetId;
            roomAsset.RoomId = updateBaseDto.RoomId ?? roomAsset.RoomId;
            roomAsset.Status = updateBaseDto.Status ?? roomAsset.Status;
            roomAsset.Quantity = updateBaseDto.Quantity ?? roomAsset.Quantity;
            roomAsset.FromDate = updateBaseDto.FromDate ?? roomAsset.FromDate;
            roomAsset.ToDate = updateBaseDto.ToDate ?? roomAsset.ToDate;

            if (!await MainUnitOfWork.RoomAssetRepository.UpdateAsync(roomAsset, AccountId, CurrentDate))
                throw new ApiException("Cập nhật thông tin thất bại", StatusCode.SERVER_ERROR);

            return ApiResponse.Success("Cập nhật thông tin thành công");
        }
    }
}
