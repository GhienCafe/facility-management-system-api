using API_FFMS.Dtos;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using System.Linq.Expressions;

namespace API_FFMS.Services
{
    public interface IRoomTypeService : IBaseService
    {
        Task<ApiResponses<RoomTypeDto>> GetRoomTypes(RoomTypeQueryDto queryDto);
        Task<ApiResponse<RoomTypeDto>> GetRoomTypes(Guid id);
    }

    public class RoomTypeService : BaseService, IRoomTypeService
    {
        public RoomTypeService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
        }

        public async Task<ApiResponses<RoomTypeDto>> GetRoomTypes(RoomTypeQueryDto queryDto)
        {
            var roomTypes = await MainUnitOfWork.RoomTypeRepository.FindResultAsync<RoomTypeDto>(
                new Expression<Func<RoomType, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => queryDto.TypeName == null || x.TypeName.ToLower().Contains(queryDto.TypeName.ToLower())
                }, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);

            roomTypes.Items = await _mapperRepository.MapCreator(roomTypes.Items.ToList());

            return ApiResponses<RoomTypeDto>.Success(
            roomTypes.Items,
            roomTypes.TotalCount,
            queryDto.PageSize,
            queryDto.Skip(),
            (int)Math.Ceiling(roomTypes.TotalCount / (double)queryDto.PageSize));
        }

        public async Task<ApiResponse<RoomTypeDto>> GetRoomTypes(Guid id)
        {
            //var roomType = await MainUnitOfWork.RoomTypeRepository.FindOneAsync<RoomTypeDto>(
            //    new Expression<Func<RoomType, bool>>[]
            //    {
            //        x => !x.DeletedAt.HasValue,
            //        x => x.Id == id
            //    });

            //if(roomType != null)
            //{
            //    throw new ApiException("Not found this room type", StatusCode.NOT_FOUND);
            //}
            throw new ApiException("");
        }
    }
}
