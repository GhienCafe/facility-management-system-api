using API_FFMS.Dtos;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using System.Linq.Expressions;
using AppCore.Extensions;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;

namespace API_FFMS.Services
{
    public interface IRoomAssetService : IBaseService
    {
        Task<ApiResponses<AssetTrackingDto>> AssetUsedTracking(Guid id, RoomTrackingQueryDto queryDto);
        //Task<ApiResponses<RoomTrackingDto>> RoomTracking(RoomTrackingQueryDto queryDto);
        Task<ApiResponse> AddRoomAsset(RoomAssetCreateDto roomAssetCreateDto);

        Task<ApiResponse> AddListRoomAsset();
    }

    public class RoomAssetService : BaseService, IRoomAssetService
    {
        public RoomAssetService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
        }

        public async Task<ApiResponses<AssetTrackingDto>> AssetUsedTracking(Guid id, RoomTrackingQueryDto queryDto)
        {
            var keyword = queryDto.Keyword?.Trim().ToLower();
            var asset = await MainUnitOfWork.AssetRepository.FindOneAsync<AssetDto>(new Expression<Func<Asset, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.Id == id
            });
            if(asset == null)
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

            if(queryDto.ToDate != null)
            {
                joinedRooms = joinedRooms.Where(x => x!.RoomAsset.ToDate <= queryDto.ToDate);
            }

            if(queryDto.FromDate != null)
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

        //public async Task<ApiResponses<RoomTrackingDto>> RoomTracking(RoomTrackingQueryDto queryDto)
        //{
        //    Expression<Func<RoomAsset, bool>>[] conditions = new Expression<Func<RoomAsset, bool>>[]
        //    {
        //        x => !x.DeletedAt.HasValue
        //    };

        //    if (string.IsNullOrEmpty(queryDto.RoomId.ToString()) == false)
        //    {
        //        conditions = conditions.Append(x => x.RoomId.Equals(queryDto.RoomId)).ToArray();
        //    }

        //    var response = await MainUnitOfWork.RoomAssetRepository.FindResultAsync<RoomTrackingDto>(conditions, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);
        //    return ApiResponses<RoomTrackingDto>.Success(
        //        response.Items,
        //        response.TotalCount,
        //        queryDto.PageSize,
        //        queryDto.Skip(),
        //    (int)Math.Ceiling(response.TotalCount / (double)queryDto.PageSize)
        //        );
        //}

        public async Task<ApiResponse> AddRoomAsset(RoomAssetCreateDto roomAssetCreateDto)
        {
            var roomAsset = roomAssetCreateDto.ProjectTo<RoomAssetCreateDto, RoomAsset>();

            if (!await MainUnitOfWork.RoomAssetRepository.InsertAsync(roomAsset, AccountId, CurrentDate))
                throw new ApiException("Insert fail", StatusCode.SERVER_ERROR);
            
            return ApiResponse.Created("Create successfully");
        }

        public async Task<ApiResponse> AddListRoomAsset()
        {
            var assets = await MainUnitOfWork.AssetRepository.FindAsync(new Expression<Func<Asset, bool>>[]
            {
                x => !x.DeletedAt.HasValue
            }, null);

            var listIds = assets.Select(x => x?.Id);

            var roomAssets = listIds.Select(x => new RoomAsset
            {
                FromDate = CurrentDate,
                AssetId = x.Value,
                RoomId = Guid.Parse("ac512c36-7c7c-430d-0cbb-08dbb8db2c89"),
                Status = AssetStatus.Operational
            }).ToList();

            if (!await MainUnitOfWork.RoomAssetRepository.InsertAsync(roomAssets, AccountId, CurrentDate))
                throw new ApiException("fail", StatusCode.BAD_REQUEST);

            return ApiResponse.Success();
        }
    }
}
