using API_FFMS.Dtos;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using System.Linq.Expressions;
using API_FFMS.Repositories;
using AppCore.Extensions;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Services
{
    public interface IMaintenanceScheduleService : IBaseService
    {
        Task<ApiResponse> CreateMaintenanceSchedule(MaintenanceScheduleConfigCreateDto createDto);
        Task<ApiResponses<MaintenanceScheduleConfigDto>> GetItems(MaintenanceScheduleConfigQueryDto queryDto);
        Task<ApiResponse<MaintenanceScheduleConfigDetailDto>> GetItem(Guid id);
        Task<ApiResponse> UpdateItem(Guid id, MaintenanceScheduleConfigUpdateDto updateDto);
        Task<ApiResponse> DeleteItem(Guid id);
        Task<ApiResponse> DeleteItems(List<Guid> ids);
    }
    public class MaintenanceScheduleService : BaseService, IMaintenanceScheduleService
    {
        private readonly IMaintenanceScheduleRepository _maintenanceScheduleRepository;
        public MaintenanceScheduleService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository, IMaintenanceScheduleRepository maintenanceScheduleRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
            _maintenanceScheduleRepository = maintenanceScheduleRepository;
        }

        public async Task<ApiResponse> CreateMaintenanceSchedule(MaintenanceScheduleConfigCreateDto createDto)
        {
            var isExists = await MainUnitOfWork.MaintenanceScheduleRepository.GetQuery()
                .Where(x => x.RepeatIntervalInMonths == createDto.RepeatIntervalInMonths).AnyAsync();

            if (isExists)
                throw new ApiException("Đã tồn tại cài đặt", StatusCode.ALREADY_EXISTS);

            var maintenanceConfig = createDto.ProjectTo<MaintenanceScheduleConfigCreateDto, MaintenanceScheduleConfig>();

            var assets = new List<Asset?>();
            
            if (createDto.AssetIds != null)
            {
                var assetIds = createDto.AssetIds.Select(id => id.Id).ToList();
                assets = await MainUnitOfWork.AssetRepository.FindAsync(new Expression<Func<Asset, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => assetIds.Contains(x.Id)
                }, null);
            }
            
            if(! await _maintenanceScheduleRepository.InsertMaintenanceScheduleConfig(maintenanceConfig, assets, AccountId, CurrentDate))
                throw new ApiException("Thêm mới thất bại", StatusCode.SERVER_ERROR);
            
            return ApiResponse.Created("Thêm mới thành công");
        }

        public async Task<ApiResponses<MaintenanceScheduleConfigDto>> GetItems(MaintenanceScheduleConfigQueryDto queryDto)
        {
            var keyword = queryDto.Keyword?.Trim().ToLower();
            var maintenanceConfigQueryable = MainUnitOfWork.MaintenanceScheduleRepository.GetQuery();

            if (!string.IsNullOrEmpty(keyword))
            {
                maintenanceConfigQueryable = maintenanceConfigQueryable
                    .Where(x => x!.Description!.ToLower().Contains(keyword)
                                || x.RepeatIntervalInMonths.ToString().Contains(keyword)
                                || x.Code.ToLower().Contains(keyword));
            }

            var totalCount = await maintenanceConfigQueryable.CountAsync();

            maintenanceConfigQueryable = maintenanceConfigQueryable.Skip(queryDto.Skip())
                .Take(queryDto.PageSize);

            var items = (await maintenanceConfigQueryable.ToListAsync())!
                .ProjectTo<MaintenanceScheduleConfig, MaintenanceScheduleConfigDto>();

            items = await _mapperRepository.MapCreator(items);
            
            return ApiResponses<MaintenanceScheduleConfigDto>.Success(
                items,
                totalCount,
                queryDto.PageSize,
                queryDto.Page,
                (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
        }

        public async Task<ApiResponse<MaintenanceScheduleConfigDetailDto>> GetItem(Guid id)
        {
            var item = (await MainUnitOfWork.MaintenanceScheduleRepository.FindOneAsync(id))?
                .ProjectTo<MaintenanceScheduleConfig, MaintenanceScheduleConfigDetailDto>();

            if (item == null)
                throw new ApiException("Không tìm thấy nội dung", StatusCode.NOT_FOUND);

            var assets = (await MainUnitOfWork.AssetRepository.FindAsync(new Expression<Func<Asset, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.MaintenanceConfigId == id
            }, null))!.ProjectTo<Asset, AssetBaseDto>();
            
            foreach (var asset in assets)
            {
                asset.StatusObj = asset.Status.GetValue();
            }

            item.Assets = assets;
            item = await _mapperRepository.MapCreator(item);
            
            return ApiResponse<MaintenanceScheduleConfigDetailDto>.Success(item);
        }

        public async Task<ApiResponse> UpdateItem(Guid id, MaintenanceScheduleConfigUpdateDto updateDto)
        {
            var checkExist = await MainUnitOfWork.MaintenanceScheduleRepository.FindAsync(
                new Expression<Func<MaintenanceScheduleConfig, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.RepeatIntervalInMonths == updateDto.RepeatIntervalInMonths
                }, null);

            if (checkExist != null && checkExist.Any())
                throw new ApiException("Đã tồn tại cài đặt", StatusCode.ALREADY_EXISTS);
            
            var item = await MainUnitOfWork.MaintenanceScheduleRepository.FindOneAsync(id);

            if (item == null)
                throw new ApiException("Không tìm thấy nội dung", StatusCode.NOT_FOUND);

            item.Description = updateDto.Description ?? item.Description;
            item.RepeatIntervalInMonths = updateDto.RepeatIntervalInMonths ?? item.RepeatIntervalInMonths;

            if (!await MainUnitOfWork.MaintenanceScheduleRepository.UpdateAsync(item, AccountId, CurrentDate))
                throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
            
            return ApiResponse.Success("Cập nhật thành công");
        }

        public async Task<ApiResponse> DeleteItem(Guid id)
        {
            var item = await MainUnitOfWork.MaintenanceScheduleRepository.FindOneAsync(id);

            if (item == null)
                throw new ApiException("Không tìm thấy nội dung", StatusCode.NOT_FOUND);

            if (!await MainUnitOfWork.MaintenanceScheduleRepository.DeleteAsync(item, AccountId, CurrentDate))
                throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);

            return ApiResponse.Success("Xóa nội dung thành công");
        }

        public async Task<ApiResponse> DeleteItems(List<Guid> ids)
        {
            var items = await MainUnitOfWork.MaintenanceScheduleRepository.FindAsync(
                new Expression<Func<MaintenanceScheduleConfig, bool>>[]
                {
                     x => !x.DeletedAt.HasValue,
                     x => ids.Contains(x.Id)
                }, null);

            if (items == null || !items.Any())
                throw new ApiException("Không tìm thấy nội dung", StatusCode.NOT_FOUND);

            if (!await MainUnitOfWork.MaintenanceScheduleRepository.DeleteAsync(items, AccountId, CurrentDate))
                throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);

            return ApiResponse.Success("Xóa nội dung thành công");
        }
    }
}
