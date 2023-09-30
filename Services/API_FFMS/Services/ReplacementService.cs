using System.Linq.Expressions;
using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Services;

public interface IReplacementService : IBaseService
{
    public Task<ApiResponse> Create(CreateReplacementDto createDto);
    public Task<ApiResponse> Update(Guid id, UpdateReplacementDto updateDto);
    Task<ApiResponse<DetailReplacementDto>> GetDetail(Guid id);
    Task<ApiResponses<ReplacementDto>> GetList(QueryReplacementDto queryDto);
    Task<ApiResponse> Delete(Guid id);
}

public class ReplacementService : BaseService, IReplacementService
{
    public ReplacementService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public async Task<ApiResponse> Create(CreateReplacementDto createDto)
    {
        var existingAsset = await MainUnitOfWork.AssetRepository.FindOneAsync(new Expression<Func<Asset, bool>>[]
        {
            x => !x.DeletedAt.HasValue
                 && x.Id == createDto.AssetId
        });
            
        if (existingAsset == null)
        {
            throw new ApiException("Không tìm thấy tài sản này hoặc tài sản đang có yêu cầu khác!", StatusCode.NOT_FOUND);
        }

        var existingAssignee = await MainUnitOfWork.UserRepository.FindOneAsync(new Expression<Func<User, bool>>[]
        {
            x => !x.DeletedAt.HasValue && x.Id == createDto.AssignedTo
        });
        if (existingAssignee == null)
        {
            throw new ApiException("Không tìm thấy người dùng này!", StatusCode.NOT_FOUND);
        }
        if(existingAsset.Status != AssetStatus.Operational && existingAsset.Status != AssetStatus.Maintenance && existingAsset.Status == AssetStatus.Repair)
        {
            throw new ApiException("Tài sản này hiện nay đang được trong hoạt động khác!", StatusCode.NOT_ACTIVE);
        }
        
        var existingNewAsset = await MainUnitOfWork.AssetRepository.FindOneAsync(new Expression<Func<Asset, bool>>[]
        {
            x => !x.DeletedAt.HasValue && x.Id == createDto.NewAssetId
                                       && x.Status == AssetStatus.Operational
        });
        if (existingNewAsset == null)
        {
            throw new ApiException("Không tìm thấy tài sản này", StatusCode.NOT_FOUND);
        }
        var replace = createDto.ProjectTo<CreateReplacementDto, Replacement>();
        replace.Id = Guid.NewGuid();
        replace.Status = ActionStatus.NotStarted;
        existingAsset.Status = AssetStatus.Replacement;
        if (!await MainUnitOfWork.ReplacementRepository.InsertAsync(replace, AccountId, CurrentDate))
        {
            throw new ApiException("Thêm thất bại!", StatusCode.BAD_REQUEST);
        }

        if (!await MainUnitOfWork.AssetRepository.UpdateAsync(existingNewAsset, AccountId,CurrentDate))
        {
            throw new ApiException("Cập nhật tài sản thất bại!", StatusCode.BAD_REQUEST);
        }
        var notification = new Notification
        {
            UserId = replace.AssignedTo,
            Status = NotificationStatus.Waiting,
            Content = "Thay thế " + existingAsset.AssetName,
            Title = "Thay thế trang thiết bị",
            Type = NotificationType.Task,
            IsRead = false,
            ItemId = replace.Id,
            ShortContent = "Thay thế trang thiết bị"
        };
        
        if (!await MainUnitOfWork.NotificationRepository.InsertAsync(notification, AccountId, CurrentDate))
            throw new ApiException("Tạo thông báo không thành công!", StatusCode.SERVER_ERROR);

        return ApiResponse.Created("Thêm thành công!");
    }

    public async Task<ApiResponse> Update(Guid id, UpdateReplacementDto updateDto)
    {
        var existingReplacement = await MainUnitOfWork.ReplacementRepository.FindOneAsync(
            new Expression<Func<Replacement, bool>>[]
            {
                x => !x.DeletedAt.HasValue
                     && x.Id == id
                     && x.Status == ActionStatus.NotStarted
            });
        if (existingReplacement==null)
        {
            throw new ApiException("Không tìm thấy yêu cầu này");
        }

        existingReplacement.RequestedDate = updateDto.RequestedDate?? existingReplacement.RequestedDate;
        existingReplacement.CompletionDate = updateDto.CompletionDate ?? existingReplacement.CompletionDate;

        existingReplacement.AssetId = updateDto.AssetId ?? existingReplacement.AssetId;
        existingReplacement.Description = updateDto.Description ?? existingReplacement.Description;
        existingReplacement.Note = updateDto.Note ?? existingReplacement.Note;
        existingReplacement.AssignedTo = updateDto.AssignedTo ?? existingReplacement.AssignedTo;
        existingReplacement.NewAssetId = updateDto.NewAssetId ?? existingReplacement.NewAssetId;
        var existingAsset = await MainUnitOfWork.AssetRepository.FindOneAsync(new Expression<Func<Asset, bool>>[]
        {
            x => !x.DeletedAt.HasValue
                 && x.Id == updateDto.AssetId
                 && x.Status == AssetStatus.Replacement
        });
        if (existingAsset == null)
        {
            throw new ApiException("Không tìm thấy tài sản này!", StatusCode.NOT_FOUND);
        }
        var existingAssignee = await MainUnitOfWork.UserRepository.FindOneAsync(new Expression<Func<User, bool>>[]
        {
            x => !x.DeletedAt.HasValue && x.Id == updateDto.AssignedTo
        });
        if (existingAssignee == null)
        {
            throw new ApiException("Không tìm thấy người dùng này!", StatusCode.NOT_FOUND);
        }
        if (!existingAssignee.TeamId.Equals(existingAsset.Type!.Category!.TeamId))
        {
            throw new ApiException("Assign have wrong major for this asset", StatusCode.BAD_REQUEST);
        }
        
        if (!await MainUnitOfWork.ReplacementRepository.UpdateAsync(existingReplacement, AccountId, CurrentDate))
        {
            throw new ApiException("Update failed", StatusCode.SERVER_ERROR);
        }
        return ApiResponse.Created("Cập Nhật thành công!");
    }

    public async Task<ApiResponse<DetailReplacementDto>> GetDetail(Guid id)
    {
        var existingReplacement = await MainUnitOfWork.ReplacementRepository.FindOneAsync<DetailReplacementDto>(
            new Expression<Func<Replacement, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.Id == id
            });
        if (existingReplacement == null)
        {
            throw new ApiException("Thay thế tài sản không tìm thấy", StatusCode.NOT_FOUND);
        }

        existingReplacement = await _mapperRepository.MapCreator(existingReplacement);
        return ApiResponse<DetailReplacementDto>.Success(existingReplacement);
    }

    public async Task<ApiResponses<ReplacementDto>> GetList(QueryReplacementDto queryDto)
    {
        var replaceQuery = MainUnitOfWork.ReplacementRepository.GetQuery().Where(x => !x!.DeletedAt.HasValue);
        if (queryDto.CompletionDate != null)
        {
            replaceQuery = replaceQuery.Where(x => x!.CompletionDate == queryDto.CompletionDate);
        }
        if (queryDto.AssignedTo != null)
        {
            replaceQuery = replaceQuery.Where(x => x!.AssignedTo == queryDto.AssignedTo);
        }
        if (queryDto.AssetId != null)
        {
            replaceQuery = replaceQuery.Where(x => x!.AssetId == queryDto.AssetId);
        }
        if (queryDto.NewAssetId != null)
        {
            replaceQuery = replaceQuery.Where(x => x!.AssetId == queryDto.AssetId);
        }
        var totalCount = replaceQuery.Count();

        var replacements = await replaceQuery
            .Skip(queryDto.Skip())
            .Take(queryDto.PageSize)
            .Select(x => new ReplacementDto
            {
                RequestedDate = x!.RequestedDate,
                CompletionDate = x.CompletionDate,
                Description = x.Description,
                Reason = x.Reason,
                Id = x.Id,
                Status = x.Status.GetValue(),
                AssignedTo = x.AssignedTo,
                AssetId = x.AssetId,
                NewAssetId = x.NewAssetId,
                CreatorId = x.CreatorId ?? Guid.Empty,
                EditorId = x.EditorId ??Guid.Empty,
                EditedAt = x.EditedAt,
                CreatedAt = x.CreatedAt,
                PersonInCharge = x.PersonInCharge!.ProjectTo<User, UserDto>(),
                Asset = x.Asset!.ProjectTo<Asset, AssetDto>()
            })
            .ToListAsync();

        replacements = await _mapperRepository.MapCreator(replacements);

        return ApiResponses<ReplacementDto>.Success(
            replacements,
            totalCount,
            queryDto.PageSize,
            queryDto.Page,
            (int)Math.Ceiling(totalCount / (double)queryDto.PageSize)
        );
    }
    public async Task<ApiResponse> Delete(Guid id)
    {
        var existingReplacement = await MainUnitOfWork.ReplacementRepository.FindOneAsync(id);

        if (existingReplacement == null)
        {
            throw new ApiException("Không tìm thấy thiết bị thay thế này", StatusCode.NOT_FOUND);
        }

        existingReplacement.Status = ActionStatus.Cancelled;
        existingReplacement.Asset!.Status = AssetStatus.Operational;
        
        Guid? idNewAsset = existingReplacement.NewAssetId;

        var existingNewAsset = await MainUnitOfWork.AssetRepository.FindOneAsync(idNewAsset!.Value);
        if (existingNewAsset == null)
        {
            throw new ApiException("Không tìm thấy thiết bị thay thế này", StatusCode.NOT_FOUND);
        }

        existingNewAsset.Status = AssetStatus.Operational;
        if (!await MainUnitOfWork.AssetRepository.UpdateAsync(existingNewAsset, AccountId, CurrentDate))
        {
            throw new ApiException("Update fail", StatusCode.SERVER_ERROR);
        }
        if (!await MainUnitOfWork.ReplacementRepository.UpdateAsync(existingReplacement, AccountId, CurrentDate))
        {
            throw new ApiException("Delete fail", StatusCode.SERVER_ERROR);
        }

        return ApiResponse.Success();
    }
}