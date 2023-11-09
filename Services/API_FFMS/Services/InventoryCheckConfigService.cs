using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using FirebaseAdmin.Messaging;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using System.Linq.Expressions;

namespace API_FFMS.Services;
public interface IInventoryCheckConfigService : IBaseService
{
    Task<ApiResponse> Create(InventoryCheckConfigCreateDto createDto);
    Task<ApiResponse<InventoryCheckConfigDto>> GetConfig(Guid id);
    //Task<ApiResponse<NextInventoryCheckDto>> NotiConfig();
    Task<ApiResponses<InventoryCheckConfigDto>> GetConfigs(InventoryCheckConfigQueryDto queryDto);
    Task<ApiResponse> Update(Guid id, InventoryCheckConfigUpdateDto updateDto);
    Task<ApiResponse> Delete(Guid id);
    Task<ApiResponse> DeleteConfigs(DeleteMutilDto deleteDto);
}

public class InventoryCheckConfigService : BaseService, IInventoryCheckConfigService
{
    public InventoryCheckConfigService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor,
                                       IMapperRepository mapperRepository)
                                       : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public async Task<ApiResponse> Create(InventoryCheckConfigCreateDto createDto)
    {
        var config = createDto.ProjectTo<InventoryCheckConfigCreateDto, InventoryCheckConfig>();
        if (!await MainUnitOfWork.InventoryCheckConfigRepository.InsertAsync(config, AccountId, CurrentDate))
        {
            throw new ApiException("Thêm mới thất bại", StatusCode.SERVER_ERROR);
        }

        return ApiResponse.Created("Thêm mới thành công");
    }

    public async Task<ApiResponse> Delete(Guid id)
    {
        var existingConfig = await MainUnitOfWork.InventoryCheckConfigRepository.FindOneAsync(id);
        if (existingConfig == null)
        {
            throw new ApiException("Không tìm thấy", StatusCode.NOT_FOUND);
        }

        if (!await MainUnitOfWork.InventoryCheckConfigRepository.DeleteAsync(existingConfig, AccountId, CurrentDate))
        {
            throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
        }

        return ApiResponse.Success();
    }

    public async Task<ApiResponse> DeleteConfigs(DeleteMutilDto deleteDto)
    {
        var configDeleteds = await MainUnitOfWork.InventoryCheckConfigRepository.FindAsync(
            new Expression<Func<InventoryCheckConfig, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => deleteDto.ListId!.Contains(x.Id)
            }, null);

        if (!await MainUnitOfWork.InventoryCheckConfigRepository.DeleteAsync(configDeleteds, AccountId, CurrentDate))
        {
            throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
        }

        return ApiResponse.Success();
    }

    public async Task<ApiResponse<InventoryCheckConfigDto>> GetConfig(Guid id)
    {
        var config = await MainUnitOfWork.InventoryCheckConfigRepository.FindOneAsync<InventoryCheckConfigDto>(
            new Expression<Func<InventoryCheckConfig, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.Id == id
            });

        if (config == null)
        {
            throw new ApiException("Không tìm thấy", StatusCode.NOT_FOUND);
        }

        config = await _mapperRepository.MapCreator(config);

        return ApiResponse<InventoryCheckConfigDto>.Success(config);
    }

    public async Task<ApiResponses<InventoryCheckConfigDto>> GetConfigs(InventoryCheckConfigQueryDto queryDto)
    {
        var response = await MainUnitOfWork.InventoryCheckConfigRepository.FindResultAsync<InventoryCheckConfigDto>(
                new Expression<Func<InventoryCheckConfig, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => string.IsNullOrEmpty(queryDto.Description) ||
                         x.Description.ToLower().Contains(queryDto.Description.Trim().ToLower())
                }, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);

        response.Items = await _mapperRepository.MapCreator(response.Items.ToList());

        return ApiResponses<InventoryCheckConfigDto>.Success(
            response.Items,
            response.TotalCount,
            queryDto.PageSize,
            queryDto.Page,
            (int)Math.Ceiling(response.TotalCount / (double)queryDto.PageSize)
        );
    }

    //public async Task<ApiResponse<NextInventoryCheckDto>> NotiConfig()
    //{
    //    var config = MainUnitOfWork.InventoryCheckConfigRepository.GetQuery();

    //    var noti = config.Select(x => new NextInventoryCheckDto
    //    {
    //        NextInventoryCheck = x.CheckPeriod != null ?
    //                            (x.LastCheckedDate != null ? x.LastCheckedDate.Value.AddMonths(x.CheckPeriod)
    //                            : x.CreatedAt.AddMonths(x.CheckPeriod)) : null

    //    });

    //    var user = MainUnitOfWork.UserRepository.GetQuery().Where(x => x!.Role == UserRole.Manager).FirstOrDefault();


    //    noti = noti.Where(x => x.NextInventoryCheck >= CurrentDate);

    //    var notification = new MainData.Entities.Notification
    //    {
    //        CreatedAt = CurrentDate,
    //        EditedAt = CurrentDate,
    //        Status = NotificationStatus.Waiting,
    //        Content = config.Select(x => x!.Description).FirstOrDefault(),
    //        Title = RequestType.InventoryCheck.GetDisplayName(),
    //        Type = NotificationType.System,
    //        CreatorId = Guid.Empty,
    //        IsRead = false,
    //        ItemId = Guid.Empty,
    //        UserId = user!.Id
    //    };


    //    if (!await MainUnitOfWork.NotificationRepository.InsertAsync(notification, AccountId, CurrentDate))
    //    {
    //        throw new ApiException("Server error for not insert notification", StatusCode.BAD_REQUEST);
    //    }

    //    return ApiResponse<NextInventoryCheckDto>.Success(notification);
    //}

    public async Task<ApiResponse> Update(Guid id, InventoryCheckConfigUpdateDto updateDto)
    {
        var existingConfig = await MainUnitOfWork.InventoryCheckConfigRepository.FindOneAsync(id);
        if (existingConfig == null)
        {
            throw new ApiException("Không tìm thấy", StatusCode.NOT_FOUND);
        }

        existingConfig.CheckPeriod = updateDto.CheckPeriod ?? existingConfig.CheckPeriod;
        existingConfig.Description = updateDto.Description ?? existingConfig.Description;

        if (!await MainUnitOfWork.InventoryCheckConfigRepository.UpdateAsync(existingConfig, AccountId, CurrentDate))
        {
            throw new ApiException("Không thể cập nhật", StatusCode.SERVER_ERROR);
        }

        return ApiResponse.Success();
    }
}
