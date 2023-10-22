using API_FFMS.Dtos;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using System.Linq.Expressions;
using API_FFMS.Repositories;
using AppCore.Extensions;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Services
{
    public interface IMaintenanceScheduleService : IBaseService
    {
        Task<ApiResponse> CreateMaintenanceSchedule(MaintenanceScheduleConfigCreateDto createDto);
        Task<ApiResponses<MaintenanceScheduleConfigDto>> GetItems(MaintenanceScheduleConfigQueryDto queryDto);
        Task<ApiResponse<MaintenanceScheduleConfigDetailDto>> GetItem(Guid id);
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
            
            if (createDto.AssetIds != null && createDto.AssetIds.Any())
            {
                assets = await MainUnitOfWork.AssetRepository.FindAsync(new Expression<Func<Asset, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => createDto.AssetIds.Contains(x.Id)
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
                    .Where(x => x!.Description.ToLower().Contains(keyword)
                                || x.RepeatIntervalInMonths.ToString().Contains(keyword));
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

            item = await _mapperRepository.MapCreator(item);
            
            return ApiResponse<MaintenanceScheduleConfigDetailDto>.Success(item);
        }
    }
}
