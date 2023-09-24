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
                throw new ApiException("Not found this room type", StatusCode.NOT_FOUND);
            }

            if (!await MainUnitOfWork.RoomTypeRepository.DeleteAsync(existingRoomType, AccountId, CurrentDate))
            {
                throw new ApiException("Can't not delete", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success();
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
                throw new ApiException("Not found this room type", StatusCode.NOT_FOUND);
            }

            //roomType.RoomInclude = MainUnitOfWork

            roomType = await _mapperRepository.MapCreator(roomType);
            return ApiResponse<RoomTypeDetailDto>.Success(roomType);
        }

        public async Task<ApiResponse> Insert(RoomTypeCreateDto createDto)
        {
            var roomType = createDto.ProjectTo<RoomTypeCreateDto, RoomType>();

            if (!await MainUnitOfWork.RoomTypeRepository.InsertAsync(roomType, AccountId, CurrentDate))
            {
                throw new ApiException("Insert fail", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Created("Create successfully");
        }

        public async Task<ApiResponse> Update(Guid id, RoomTypeUpdateDto updateDto)
        {
            var roomType = await MainUnitOfWork.RoomTypeRepository.FindOneAsync(id);

            if (roomType == null)
            {
                throw new ApiException("Not found this room type", StatusCode.NOT_FOUND);
            }

            roomType.TypeName = updateDto.TypeName ?? roomType.TypeName;
            roomType.Description = updateDto.Description ?? roomType.Description;

            if (!await MainUnitOfWork.RoomTypeRepository.UpdateAsync(roomType, AccountId, CurrentDate))
            {
                throw new ApiException("Insert fail", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success();
        }
    }
}
