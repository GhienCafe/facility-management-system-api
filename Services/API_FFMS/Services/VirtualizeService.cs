using System.Linq.Expressions;
using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Services;
public interface IVirtualizeService : IBaseService
{
    Task<ApiResponse<VirtualizeFloorDto>> GetVirtualizeFloor(Guid floorId);
    Task<ApiResponse<IEnumerable<VirtualizeRoomDto>>> GetVirtualizeRoom(VirtualizeRoomQueryDto queryDto);
    Task<ApiResponse<VirtualDashboard>> GetVirtualDashBoard();
}
public class VirtualizeService : BaseService, IVirtualizeService
{
    public VirtualizeService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor,
        IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public async Task<ApiResponse<VirtualizeFloorDto>> GetVirtualizeFloor(Guid floorId)
    {
        var floor = await MainUnitOfWork.FloorRepository.FindOneAsync(floorId);

        if (floor == null)
            throw new ApiException("Không tìm thấy phòng", StatusCode.NOT_FOUND);

        var floorDto = floor.ProjectTo<Floor, VirtualizeFloorDto>();
        
        return ApiResponse<VirtualizeFloorDto>.Success(floorDto);
    }

    public async Task<ApiResponse<IEnumerable<VirtualizeRoomDto>>> GetVirtualizeRoom(VirtualizeRoomQueryDto queryDto)
    {
        var roomQueryable = MainUnitOfWork.RoomRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue && x.FloorId == queryDto.FloorId);

        var roomAssetQueryable = MainUnitOfWork.RoomAssetRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);
            
        var statusQueryable = MainUnitOfWork.RoomStatusRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);
        
        var roomTypeQueryable = MainUnitOfWork.RoomTypeRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);
        
        var result = from r in roomQueryable
             join ra in roomAssetQueryable on r.Id equals ra.RoomId into groupedAssets
             from ga in groupedAssets.DefaultIfEmpty()
             join st in statusQueryable on r.StatusId equals st.Id into statusGroup
             from st in statusGroup.DefaultIfEmpty() 
             join rt in roomTypeQueryable on r.RoomTypeId equals rt.Id into roomTypeGroup
             from rt in roomTypeGroup.DefaultIfEmpty() 
             group new { r, ga, st, rt } by new
             {
                 r.Id,
                 r.RoomName,
                 r.Area,
                 r.PathRoom,
                 r.RoomCode,
                 r.RoomTypeId,
                 r.Capacity,
                 r.StatusId,
                 r.FloorId,
                 r.Description,
                 r.CreatedAt,
                 r.EditedAt,
                 r.CreatorId,
                 r.EditorId,
                 StatusName = st.StatusName,
                 StatusDescription = st.Description,
                 Color = st.Color,
                 StatusCreatedAt = st.CreatedAt,
                 StatusEditedAt = st.EditedAt,
                 StatusCreator = st.CreatorId,
                 StatusEditor = st.EditorId,
                 RoomTypeName = rt.TypeName,
                 RoomTypeDesciption= rt.Description,
                 RoomTypeCreatedAt = rt.CreatedAt,
                 RoomTypeEditedAt = rt.EditedAt,
                 RoomTypeCreator = rt.CreatorId,                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            
                 RoomTypeEditor = rt.EditorId,
                 
             } into groupedData
             select new VirtualizeRoomDto
             {
                 TotalAssets = groupedData.Count(item => item.ga.ToDate == null && item.ga.Room != null),
                 TotalNormalAssets = groupedData.Count(item => item.ga.ToDate == null && item.ga.Status == AssetStatus.Operational),
                 TotalDamagedAssets = groupedData.Count(item => item.ga.ToDate == null && item.ga.Status == AssetStatus.Damaged),
                 TotalOtherAssets = groupedData.Count(item => item.ga.ToDate == null && item.ga.Status != AssetStatus.Operational && item.ga.Status != AssetStatus.Damaged),
                 Id = groupedData.Key.Id,
                 RoomName = groupedData.Key.RoomName,
                 Area = groupedData.Key.Area,
                 PathRoom = groupedData.Key.PathRoom,
                 RoomCode = groupedData.Key.RoomCode,
                 RoomTypeId = groupedData.Key.RoomTypeId,
                 Capacity = groupedData.Key.Capacity,
                 StatusId = groupedData.Key.StatusId,
                 FloorId = groupedData.Key.FloorId,
                 Description = groupedData.Key.Description,
                 CreatedAt = groupedData.Key.CreatedAt,
                 EditedAt = groupedData.Key.EditedAt,
                 CreatorId = groupedData.Key.CreatorId ?? Guid.Empty,
                 EditorId = groupedData.Key.EditorId ?? Guid.Empty,
                 Status = new RoomStatusDto
                 {
                     Id = groupedData.Key.StatusId,
                     StatusName = groupedData.Key.StatusName,
                     Description = groupedData.Key.StatusDescription,
                     Color = groupedData.Key.Color,
                     CreatedAt = groupedData.Key.StatusCreatedAt,
                     EditedAt = groupedData.Key.StatusEditedAt,
                     CreatorId = groupedData.Key.StatusCreator ?? Guid.Empty,
                     EditorId = groupedData.Key.StatusEditor ?? Guid.Empty
                 },
                 RoomType = new RoomTypeDto
                 {
                     Id = groupedData.Key.RoomTypeId ?? Guid.Empty,
                     TypeName = groupedData.Key.RoomTypeName,
                     Description = groupedData.Key.RoomTypeDesciption,
                     CreatedAt = groupedData.Key.RoomTypeCreatedAt,
                     EditedAt = groupedData.Key.RoomTypeEditedAt,
                     CreatorId = groupedData.Key.RoomTypeCreator ?? Guid.Empty,
                     EditorId = groupedData.Key.RoomTypeEditor ?? Guid.Empty
                 }
             };
        
        // Execute the query and retrieve the result
        var queryResult = await result.ToListAsync();

        //foreach(var item in queryResult)
        //{
        //    item.StatusBaseOnAsset = GetRoomAssetStatus(item.Id).GetValue();
        //}

        return ApiResponse<IEnumerable<VirtualizeRoomDto>>.Success(queryResult);
    }

    public async Task<ApiResponse<VirtualDashboard>> GetVirtualDashBoard()
    {
        var virtualDashboard = new VirtualDashboard();

        virtualDashboard.TotalAsset = await MainUnitOfWork.AssetRepository.CountAsync(null);
        virtualDashboard.TotalFloor = await MainUnitOfWork.FloorRepository.CountAsync(null);
        virtualDashboard.TotalRoom = await MainUnitOfWork.RoomRepository.CountAsync(null);
        virtualDashboard.TotalAssetType = await MainUnitOfWork.AssetTypeRepository.CountAsync(null);
        virtualDashboard.TotalUser = await MainUnitOfWork.UserRepository.CountAsync(null);
        
        return ApiResponse<VirtualDashboard>.Success(virtualDashboard);
    }

    //public RoomAssetStatus GetRoomAssetStatus(Guid roomId)
    //{
    //    var roomAssets = MainUnitOfWork.RoomAssetRepository.GetQuery()
    //        .Where(x => x!.RoomId == roomId && x.ToDate == null);

    //    var currentQuantityAssetInRoom = roomAssets.Sum(x => x!.Quantity);

    //    var assetIdsInRoom = roomAssets.Select(ra => ra!.AssetId);

    //    var assetInRooms = MainUnitOfWork.AssetRepository.GetQuery()
    //        .Where(x => assetIdsInRoom.Contains(x!.Id) && x.Status == AssetStatus.Damaged);

    //    var currentAssetDamagedInRoom = assetInRooms.Sum(x => x!.Quantity);

    //    var roomAssetStatus = RoomAssetStatus.Operational;

    //    if (currentQuantityAssetInRoom > 0 && currentAssetDamagedInRoom == 0)
    //    {
    //        roomAssetStatus = RoomAssetStatus.Operational;
    //    }
    //    else if (currentAssetDamagedInRoom > 0 && currentAssetDamagedInRoom <= 5)
    //    {
    //        roomAssetStatus = RoomAssetStatus.Notice;
    //    }
    //    else if (currentAssetDamagedInRoom > 5 && currentAssetDamagedInRoom <= 15)
    //    {
    //        roomAssetStatus = RoomAssetStatus.Caution;
    //    }
    //    else if (currentAssetDamagedInRoom > 15 && currentAssetDamagedInRoom <= 30)
    //    {
    //        roomAssetStatus = RoomAssetStatus.Warning;
    //    }
    //    else if (currentQuantityAssetInRoom > 0 && currentAssetDamagedInRoom == currentQuantityAssetInRoom)
    //    {
    //        roomAssetStatus = RoomAssetStatus.Danger;
    //    }
    //    else if (currentQuantityAssetInRoom == 0)
    //    {
    //        roomAssetStatus = RoomAssetStatus.Empty;
    //    }

    //    return roomAssetStatus;
    //}
}