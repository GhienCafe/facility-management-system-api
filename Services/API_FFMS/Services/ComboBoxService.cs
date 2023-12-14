using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Services;

public interface IComboBoxService : IBaseService
{
    Task<ApiResponses<RoomComboBoxDto>> GetRoomBomboBoxs(ComboBoxQueryDto queryDto);
}

public class ComboBoxService : BaseService, IComboBoxService
{
    public ComboBoxService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor,
                           IMapperRepository mapperRepository)
                           : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public async Task<ApiResponses<RoomComboBoxDto>> GetRoomBomboBoxs(ComboBoxQueryDto queryDto)
    {
        var keyword = queryDto.Keyword?.Trim().ToLower();
        var roomQuery = MainUnitOfWork.RoomRepository.GetQuery();

        if (!string.IsNullOrEmpty(keyword))
        {
            roomQuery = roomQuery.Where(x => x!.RoomName!.ToLower().Contains(keyword) ||
                                             x.RoomCode.ToLower().Contains(keyword));
        }

        var response = from room in roomQuery
                       join status in MainUnitOfWork.RoomStatusRepository.GetQuery() on room.StatusId equals status.Id
                           into statusGroup
                       from status in statusGroup.DefaultIfEmpty()
                       select new
                       {
                           Room = room,
                           Status = status
                       };

        var rooms = await response.Select(x => new RoomComboBoxDto
        {
            Id = x.Room.Id,
            RoomCode = x.Room.RoomCode,
            RoomName = x.Room.RoomName,
            StatusId = x.Room.StatusId,
            Status = x.Status.ProjectTo<RoomStatus, RoomStatusComboBoxDto>()
        }).ToListAsync();

        return ApiResponses<RoomComboBoxDto>.Success(rooms);
    }
}
