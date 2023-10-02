using API_FFMS.Dtos;
using AppCore.Extensions;
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
        Task<ApiResponse<RoomTypeDetailDto>> GetRoomType(Guid id);
        public Task<ApiResponse> Insert(RoomTypeCreateDto createDto);
        Task<ApiResponse> Delete(Guid id);
        Task<ApiResponse> Update(Guid id, RoomTypeUpdateDto updateDto);
    }

    public class RoomTypeService : BaseService, IRoomTypeService
    {
        public RoomTypeService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
        }

        public async Task<ApiResponse> Delete(Guid id)
        {
            var existingRoomType = await MainUnitOfWork.RoomTypeRepository.FindOneAsync(id);
            if (existingRoomType == null)
            {
                throw new ApiException("Không tìm thấy loại phòng", StatusCode.NOT_FOUND);
            }

            if (!await MainUnitOfWork.RoomTypeRepository.DeleteAsync(existingRoomType, AccountId, CurrentDate))
            {
                throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success();
        }

        public async Task<ApiResponses<RoomTypeDto>> GetRoomTypes(RoomTypeQueryDto queryDto)
        {
            var keyword = queryDto.Keyword?.Trim().ToLower(); 
            var roomTypes = await MainUnitOfWork.RoomTypeRepository.FindResultAsync<RoomTypeDto>(
                new Expression<Func<RoomType, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => keyword == null || (x.TypeName.ToLower().Contains(keyword)
                        || x.Description!.ToLower().Contains(keyword))
                }, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);

            roomTypes.Items = await _mapperRepository.MapCreator(roomTypes.Items.ToList());

            return ApiResponses<RoomTypeDto>.Success(
            roomTypes.Items,
            roomTypes.TotalCount,
            queryDto.PageSize,
            queryDto.Page,
            (int)Math.Ceiling(roomTypes.TotalCount / (double)queryDto.PageSize));
        }

        public async Task<ApiResponse<RoomTypeDetailDto>> GetRoomType(Guid id)
        {
            var roomType = await MainUnitOfWork.RoomTypeRepository.FindOneAsync<RoomTypeDetailDto>(
                new Expression<Func<RoomType, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id,
                });

            if (roomType == null)
            {
                throw new ApiException("Không tìm thấy loại phòng", StatusCode.NOT_FOUND);
            }

            roomType.TotalRooms = MainUnitOfWork.RoomRepository.GetQuery().Count(x => !x!.DeletedAt.HasValue
                    && x.RoomTypeId == roomType.Id);

            roomType = await _mapperRepository.MapCreator(roomType);
            return ApiResponse<RoomTypeDetailDto>.Success(roomType);
        }

        public async Task<ApiResponse> Insert(RoomTypeCreateDto createDto)
        {
            var roomType = createDto.ProjectTo<RoomTypeCreateDto, RoomType>();

            if (!await MainUnitOfWork.RoomTypeRepository.InsertAsync(roomType, AccountId, CurrentDate))
            {
                throw new ApiException("Thêm thất bại", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Created("Thêm thành công");
        }

        public async Task<ApiResponse> Update(Guid id, RoomTypeUpdateDto updateDto)
        {
            var roomType = await MainUnitOfWork.RoomTypeRepository.FindOneAsync(id);

            if (roomType == null)
            {
                throw new ApiException("Không tìm thấy loại phòng", StatusCode.NOT_FOUND);
            }

            roomType.TypeName = updateDto.TypeName ?? roomType.TypeName;
            roomType.Description = updateDto.Description ?? roomType.Description;

            if (!await MainUnitOfWork.RoomTypeRepository.UpdateAsync(roomType, AccountId, CurrentDate))
            {
                throw new ApiException("Cập nhật thất bại", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success("Cập nhật thành công");
        }
    }
}
