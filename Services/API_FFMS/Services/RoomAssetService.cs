using API_FFMS.Dtos;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using System.Linq.Expressions;
using AppCore.Extensions;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Services
{
    public interface IRoomAssetService : IBaseService
    {
        Task<ApiResponses<AssetTrackingDto>> AssetUsedTracking(RoomTrackingQueryDto queryDto);
        Task<ApiResponses<RoomTrackingDto>> RoomTracking(RoomTrackingQueryDto queryDto);
        Task<ApiResponse> AddRoomAsset(RoomAssetCreateDto roomAssetCreateDto);
    }

    public class RoomAssetService : BaseService, IRoomAssetService
    {
        public RoomAssetService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
        }

        public async Task<ApiResponses<AssetTrackingDto>> AssetUsedTracking(RoomTrackingQueryDto queryDto)
        {
            Expression<Func<RoomAsset, bool>>[] conditions = new Expression<Func<RoomAsset, bool>>[]
            {
                x => !x.DeletedAt.HasValue
            };

            if (string.IsNullOrEmpty(queryDto.AssetId.ToString()) == false)
            {
                conditions = conditions.Append(x => x.AssetId.Equals(queryDto.AssetId)).ToArray();
            }

            var response = await MainUnitOfWork.RoomAssetRepository.FindResultAsync<AssetTrackingDto>(conditions, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);
            return ApiResponses<AssetTrackingDto>.Success(
                response.Items,
                response.TotalCount,
                queryDto.PageSize,
                queryDto.Skip(),
            (int)Math.Ceiling(response.TotalCount / (double)queryDto.PageSize)
                );
        }

        public async Task<ApiResponses<RoomTrackingDto>> RoomTracking(RoomTrackingQueryDto queryDto)
        {
            Expression<Func<RoomAsset, bool>>[] conditions = new Expression<Func<RoomAsset, bool>>[]
            {
                x => !x.DeletedAt.HasValue
            };

            if (string.IsNullOrEmpty(queryDto.RoomId.ToString()) == false)
            {
                conditions = conditions.Append(x => x.RoomId.Equals(queryDto.RoomId)).ToArray();
            }

            var response = await MainUnitOfWork.RoomAssetRepository.FindResultAsync<RoomTrackingDto>(conditions, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);
            return ApiResponses<RoomTrackingDto>.Success(
                response.Items,
                response.TotalCount,
                queryDto.PageSize,
                queryDto.Skip(),
            (int)Math.Ceiling(response.TotalCount / (double)queryDto.PageSize)
                );
        }

        public async Task<ApiResponse> AddRoomAsset(RoomAssetCreateDto roomAssetCreateDto)
        {
            var roomAsset = roomAssetCreateDto.ProjectTo<RoomAssetCreateDto, RoomAsset>();

            if (!await MainUnitOfWork.RoomAssetRepository.InsertAsync(roomAsset, AccountId, CurrentDate))
                throw new ApiException("Insert fail", StatusCode.SERVER_ERROR);
            
            return ApiResponse.Created("Create successfully");
        }
        
    }
}
