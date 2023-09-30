using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API_FFMS.Services
{
    public interface IRepairationService : IBaseService
    {
        Task<ApiResponses<RepairationDto>> GetRepairations(RepairationQueryDto queryDto);
        Task<ApiResponse> CreateRepairation(RepairationCreateDto createDto);
        Task<ApiResponse<RepairationDetailDto>> GetRepairation(Guid id);
        public Task<ApiResponse> Update(Guid id, RepairationUpdateDto updateDto);
        Task<ApiResponse> Delete(Guid id);
    }
    public class RepairationService : BaseService, IRepairationService
    {
        public RepairationService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
        }

        public async Task<ApiResponse> CreateRepairation(RepairationCreateDto createDto)
        {
            var existingAsset = await MainUnitOfWork.AssetRepository.FindOneAsync(new Expression<Func<Asset, bool>>[]
            {
                x => !x.DeletedAt.HasValue
                && x.Id == createDto.AssetId
            });
            if (existingAsset == null)
            {
                throw new ApiException("Not found this asset", StatusCode.NOT_FOUND);
            }

            if (existingAsset.Status != AssetStatus.Operational)
            {
                throw new ApiException("This asset is in another request", StatusCode.NOT_ACTIVE);
            }

            var existingAssignee = await MainUnitOfWork.UserRepository.FindOneAsync(new Expression<Func<User, bool>>[]
            {
                x => !x.DeletedAt.HasValue && x.Id == createDto.AssignedTo
            });
            if (existingAssignee == null)
            {
                throw new ApiException("Not found this user", StatusCode.NOT_FOUND);
            }

            var repairation = createDto.ProjectTo<RepairationCreateDto, Repairation>();
            repairation.Id = Guid.NewGuid();

            // check asset is rent or not

            repairation.Status = ActionStatus.NotStarted;
            existingAsset.Status = AssetStatus.Repair;

            if (!await MainUnitOfWork.RepairationRepository.InsertAsync(repairation, AccountId, CurrentDate))
            {
                throw new ApiException("Tạo yêu cầu thất bại", StatusCode.SERVER_ERROR);
            }
            var notification = new Notification
            {
                UserId = repairation.AssignedTo,
                Status = NotificationStatus.Waiting,
                Content = "Sửa chữa trang thiết bị ...",
                Title = "Sửa chữa trang thiết bị",
                Type = NotificationType.Task,
                IsRead = false,
                ItemId = repairation.Id,
                ShortContent = "Sửa chữa trang thiết bị"
            };

            if (!await MainUnitOfWork.NotificationRepository.InsertAsync(notification, AccountId, CurrentDate))
            {
                throw new ApiException("Thông báo tới nhân viên không thành công", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Created("Tạo yêu cầu thành công");
        }

        public async Task<ApiResponse> Delete(Guid id)
        {
            var existingRepairation = await MainUnitOfWork.RepairationRepository.FindOneAsync(id);

            if (existingRepairation == null)
            {
                throw new ApiException("Not found this transportation", StatusCode.NOT_FOUND);
            }

            existingRepairation.Status = ActionStatus.Cancelled;
            existingRepairation.Asset!.Status = AssetStatus.Operational;

            if (!await MainUnitOfWork.RepairationRepository.UpdateAsync(existingRepairation, AccountId, CurrentDate))
            {
                throw new ApiException("Delete fail", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success();
        }

        public async Task<ApiResponse<RepairationDetailDto>> GetRepairation(Guid id)
        {
            var repairation = await MainUnitOfWork.RepairationRepository.FindOneAsync<RepairationDetailDto>(
                new Expression<Func<Repairation, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.Id == id
            });

            if (repairation == null)
            {
                throw new ApiException("Not found this repairation", StatusCode.NOT_FOUND);
            }

            repairation.PersonInCharge = await MainUnitOfWork.UserRepository.FindOneAsync<UserDto>(
                new Expression<Func<User, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.Id == repairation.AssignedTo
            });

            repairation.Asset = await MainUnitOfWork.AssetRepository.FindOneAsync<AssetDto>(
                new Expression<Func<Asset, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.Id == repairation.AssetId
            });

            repairation = await _mapperRepository.MapCreator(repairation);

            return ApiResponse<RepairationDetailDto>.Success(repairation);
        }

        public async Task<ApiResponses<RepairationDto>> GetRepairations(RepairationQueryDto queryDto)
        {
            var repairQuery = MainUnitOfWork.RepairationRepository.GetQuery()
                                 .Where(x => !x!.DeletedAt.HasValue);

            if (queryDto.RequestedDate != null)
            {
                repairQuery = repairQuery.Where(x => x!.RequestedDate == queryDto.RequestedDate);
            }

            if (queryDto.CompletionDate != null)
            {
                repairQuery = repairQuery.Where(x => x!.CompletionDate == queryDto.CompletionDate);
            }

            if (queryDto.Note != null)
            {
                repairQuery = repairQuery.Where(x => x!.Note == queryDto.Note);
            }

            if (queryDto.Status != null)
            {
                repairQuery = repairQuery.Where(x => x!.Status == queryDto.Status);
            }

            if (queryDto.AssignedTo != null)
            {
                repairQuery = repairQuery.Where(x => x!.AssignedTo == queryDto.AssignedTo);
            }

            if (queryDto.AssetId != null)
            {
                repairQuery = repairQuery.Where(x => x!.AssetId == queryDto.AssetId);
            }

            var joinTables = from repair in repairQuery
                             join asset in MainUnitOfWork.AssetRepository.GetQuery() on repair.AssetId equals asset.Id into assetGroup
                             from asset in assetGroup.DefaultIfEmpty()
                             join personInCharge in MainUnitOfWork.UserRepository.GetQuery() on repair.AssignedTo equals personInCharge.Id into personInChargeGroup
                             from personInCharge in personInChargeGroup.DefaultIfEmpty()
                             select new
                             {
                                 Repairation = repair,
                                 Asset = asset,
                                 PersonInCharge = personInCharge
                             };

            var totalCount = joinTables.Count();
            joinTables = joinTables.Skip(queryDto.Skip()).Take(queryDto.PageSize);

            var repairations = await joinTables.Select(
                x => new RepairationDto
                {
                    AssignedTo = x.Repairation.AssignedTo,
                    Status = x.Repairation.Status.GetValue(),
                    Id = x.Repairation.Id,
                    CompletionDate = x.Repairation.CompletionDate,
                    CreatedAt = x.Repairation.CreatedAt,
                    EditedAt = x.Repairation.EditedAt,
                    CreatorId = x.Repairation.CreatorId ?? Guid.Empty,
                    EditorId = x.Repairation.EditorId ?? Guid.Empty,
                    AssetId = x.Repairation.AssetId,
                    RequestedDate = x.Repairation.RequestedDate,
                    Note = x.Repairation.Note,
                    Asset = x.Asset.ProjectTo<Asset, AssetDto>(),
                    PersonInCharge = x.PersonInCharge.ProjectTo<User, UserDto>(),
                }).ToListAsync();

            repairations = await _mapperRepository.MapCreator(repairations);

            return ApiResponses<RepairationDto>.Success(
                   repairations,
                   totalCount,
                   queryDto.PageSize,
                   queryDto.Page,
                   (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
        }

        public async Task<ApiResponse> Update(Guid id, RepairationUpdateDto updateDto)
        {
            var existingRepairation = await MainUnitOfWork.RepairationRepository.FindOneAsync(
                new Expression<Func<Repairation, bool>>[]
            {
                x => !x.DeletedAt.HasValue
                && x.Id == id
                && x.Status == ActionStatus.NotStarted
            });
            if (existingRepairation == null)
            {
                throw new ApiException("Not found this repairation", StatusCode.NOT_FOUND);
            }

            existingRepairation.RequestedDate = updateDto.RequestedDate ?? existingRepairation.RequestedDate;
            existingRepairation.CompletionDate = updateDto.CompletionDate ?? existingRepairation.CompletionDate;
            existingRepairation.Description = updateDto.Description ?? existingRepairation.Description;
            existingRepairation.Note = updateDto.Note ?? existingRepairation.Note;
            existingRepairation.Reason = updateDto.Reason ?? existingRepairation.Reason;
            existingRepairation.AssignedTo = updateDto.AssignedTo ?? existingRepairation.AssignedTo;
            //existingRepairation.AssetId = updateDto.AssetId ?? existingRepairation.AssetId;

            var existingAsset = await MainUnitOfWork.AssetRepository.FindOneAsync(new Expression<Func<Asset, bool>>[]
            {
                x => !x.DeletedAt.HasValue
                && x.Id == existingRepairation.AssetId
            });

            var existingAssignee = await MainUnitOfWork.UserRepository.FindOneAsync(new Expression<Func<User, bool>>[]
            {
                x => !x.DeletedAt.HasValue && x.Id == updateDto.AssignedTo
            });
            if (existingAssignee == null)
            {
                throw new ApiException("Not found this user", StatusCode.NOT_FOUND);
            }

            if (!existingAssignee.TeamId.Equals(existingAsset!.Type!.Category!.TeamId))
            {
                throw new ApiException("Assign have wrong major for this asset", StatusCode.BAD_REQUEST);
            }

            if (!await MainUnitOfWork.RepairationRepository.UpdateAsync(existingRepairation, AccountId, CurrentDate))
            {
                throw new ApiException("Update failed", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success();
        }
    }
}
