using API_FFMS.Dtos;
using API_FFMS.Repositories;
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
        Task<ApiResponse<RepairationDto>> GetRepairation(Guid id);
        Task<ApiResponse> Update(Guid id, BaseRequestUpdateDto updateDto);
        Task<ApiResponse> Delete(Guid id);
        Task<ApiResponse> DeleteReplairations(List<Guid> ids);
    }
    public class RepairationService : BaseService, IRepairationService
    {
        private readonly IRepairationRepository _repairationRepository;
        public RepairationService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor,
                                 IMapperRepository mapperRepository, IRepairationRepository repairationRepository)
                                 : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
            _repairationRepository = repairationRepository;
        }

        public async Task<ApiResponse> CreateRepairation(RepairationCreateDto createDto)
        {
            var asset = await MainUnitOfWork.AssetRepository.FindOneAsync(createDto.AssetId);

            if (asset == null)
                throw new ApiException("Không cần tồn tại trang thiết bị", StatusCode.NOT_FOUND);

            if (asset.Status != AssetStatus.Operational)
                throw new ApiException("Trang thiết bị đang trong một yêu cầu khác", StatusCode.BAD_REQUEST);

            var repairation = createDto.ProjectTo<RepairationCreateDto, Repairation>();

            if (!await _repairationRepository.InsertRepairation(repairation, AccountId, CurrentDate))
                throw new ApiException("Tạo yêu cầu thất bại", StatusCode.SERVER_ERROR);

            return ApiResponse.Created("Gửi yêu cầu thành công");
        }

        public async Task<ApiResponse> Delete(Guid id)
        {
            var existingRepair = await MainUnitOfWork.RepairationRepository.FindOneAsync(
                new Expression<Func<Repairation, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id
                });
            if (existingRepair == null)
            {
                throw new ApiException("Không tìm thấy yêu cầu sửa chữa này", StatusCode.NOT_FOUND);
            }

            existingRepair.Status = RequestStatus.Cancelled;

            if (!await MainUnitOfWork.RepairationRepository.DeleteAsync(existingRepair, AccountId, CurrentDate))
            {
                throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
            }
            return ApiResponse.Success();
        }

        public async Task<ApiResponse> DeleteReplairations(List<Guid> ids)
        {
            var replairDeleteds = await MainUnitOfWork.RepairationRepository.FindAsync(
                new Expression<Func<Repairation, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                x => ids.Contains(x.Id)
                }, null);
            if (!await MainUnitOfWork.RepairationRepository.DeleteAsync(replairDeleteds, AccountId, CurrentDate))
            {
                throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
            }
            return ApiResponse.Success();
        }

        public async Task<ApiResponse<RepairationDto>> GetRepairation(Guid id)
        {
            var repairation = await MainUnitOfWork.RepairationRepository.FindOneAsync<RepairationDto>(
                new Expression<Func<Repairation, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id
                });
            if (repairation == null)
            {
                throw new ApiException("Không tìm thấy yêu cầu", StatusCode.NOT_FOUND);
            }

            repairation.Asset = await MainUnitOfWork.AssetRepository.FindOneAsync<AssetBaseDto>(
                new Expression<Func<Asset, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == repairation.AssetId
                });
            if (repairation.Asset != null)
            {
                repairation.Asset!.StatusObj = repairation.Asset.Status?.GetValue();
            }

            repairation.User = await MainUnitOfWork.UserRepository.FindOneAsync<UserBaseDto>(
                new Expression<Func<User, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == repairation.AssignedTo
                });
            if (repairation.User != null)
            {
                repairation.User!.StatusObj = repairation.User.Status?.GetValue();
                repairation.User!.RoleObj = repairation.User.Role?.GetValue();
            }


            repairation = await _mapperRepository.MapCreator(repairation);

            return ApiResponse<RepairationDto>.Success(repairation);
        }

        public async Task<ApiResponses<RepairationDto>> GetRepairations(RepairationQueryDto queryDto)
        {
            var keyword = queryDto.Keyword?.Trim().ToLower();

            var repairQuery = MainUnitOfWork.RepairationRepository.GetQuery()
                              .Where(x => !x!.DeletedAt.HasValue);

            //if (queryDto.IsInternal != null)
            //{
            //    repairQuery = repairQuery.Where(x => x!.IsInternal == queryDto.IsInternal);
            //}

            if (keyword != null)
            {
                repairQuery = repairQuery.Where(x => x!.RequestCode.ToLower().Contains(keyword));
            }

            if (queryDto.AssignedTo != null)
            {
                repairQuery = repairQuery.Where(x => x!.AssignedTo == queryDto.AssignedTo);
            }

            if (queryDto.AssetId != null)
            {
                repairQuery = repairQuery.Where(x => x!.AssetId == queryDto.AssetId);
            }

            if (queryDto.Status != null)
            {
                repairQuery = repairQuery.Where(x => x!.Status == queryDto.Status);
            }

            if (queryDto.RequestDate != null)
            {
                repairQuery = repairQuery.Where(x => x!.RequestDate == queryDto.RequestDate);
            }

            if (queryDto.CompletionDate != null)
            {
                repairQuery = repairQuery.Where(x => x!.CompletionDate == queryDto.CompletionDate);
            }

            var joinTable = from repair in repairQuery
                            join user in MainUnitOfWork.UserRepository.GetQuery() on repair.AssignedTo equals user.Id
                            join asset in MainUnitOfWork.AssetRepository.GetQuery() on repair.AssetId equals asset.Id
                            select new
                            {
                                Repairation = repair,
                                Asset = asset,
                                User = user
                            };

            var totalCount = await joinTable.CountAsync();

            joinTable = joinTable.Skip(queryDto.Skip()).Take(queryDto.PageSize);

            var repairations = await joinTable.Select(x => new RepairationDto
            {
                Id = x.Repairation.Id,
                RequestCode = x.Repairation.RequestCode,
                RequestDate = x.Repairation.RequestDate,
                CompletionDate = x.Repairation.CompletionDate,
                Status = x.Repairation.Status,
                Description = x.Repairation.Description,
                Notes = x.Repairation.Notes,
                IsInternal = x.Repairation.IsInternal,
                AssignedTo = x.Repairation.AssignedTo,
                AssetId = x.Repairation.AssetId,
                CreatedAt = x.Repairation.CreatedAt,
                EditedAt = x.Repairation.EditedAt,
                CreatorId = x.Repairation.CreatorId ?? Guid.Empty,
                EditorId = x.Repairation.EditorId ?? Guid.Empty,
                Asset = new AssetBaseDto
                {
                    Id = x.Asset.Id,
                    AssetName = x.Asset.AssetName,
                    AssetCode = x.Asset.AssetCode,
                    IsMovable = x.Asset.IsMovable,
                    Status = x.Asset.Status,
                    StatusObj = x.Asset.Status.GetValue(),
                    ManufacturingYear = x.Asset.ManufacturingYear,
                    SerialNumber = x.Asset.SerialNumber,
                    Quantity = x.Asset.Quantity,
                    Description = x.Asset.Description,
                    LastCheckedDate = x.Asset.LastCheckedDate,
                    LastMaintenanceTime = x.Asset.LastMaintenanceTime,
                    TypeId = x.Asset.TypeId,
                    ModelId = x.Asset.ModelId,
                    IsRented = x.Asset.IsRented,
                    StartDateOfUse = x.Asset.StartDateOfUse
                },
                User = new UserBaseDto
                {
                    Id = x.User.Id,
                    Fullname = x.User.Fullname,
                    UserCode = x.User.UserCode,
                    Role = x.User.Role,
                    RoleObj = x.User.Role.GetValue(),
                    Avatar = x.User.Avatar,
                    Status = x.User.Status,
                    StatusObj = x.User.Status.GetValue(),
                    Email = x.User.Email,
                    PhoneNumber = x.User.PhoneNumber,
                    Address = x.User.Address,
                    Gender = x.User.Gender,
                    PersonalIdentifyNumber = x.User.PersonalIdentifyNumber,
                    Dob = x.User.Dob
                }
            }).ToListAsync();

            repairations = await _mapperRepository.MapCreator(repairations);

            return ApiResponses<RepairationDto>.Success(
                repairations,
                totalCount,
                queryDto.PageSize,
                queryDto.Page,
                (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
        }

        public async Task<ApiResponse> Update(Guid id, BaseRequestUpdateDto updateDto)
        {
            var existingRepair = await MainUnitOfWork.RepairationRepository.FindOneAsync(id);
            if (existingRepair == null)
            {
                throw new ApiException("Không tìm thấy yêu cầu", StatusCode.NOT_FOUND);
            }

            if (existingRepair.Status != RequestStatus.NotStarted)
            {
                throw new ApiException("Chỉ được cập nhật các yêu cầu chưa hoàn thành", StatusCode.NOT_FOUND);
            }

            //existingRepair.RequestCode = updateDto.RequestCode ?? existingRepair.RequestCode;
            existingRepair.RequestDate = updateDto.RequestDate ?? existingRepair.RequestDate;
            existingRepair.CompletionDate = updateDto.CompletionDate ?? existingRepair.CompletionDate;
            existingRepair.Status = updateDto.Status ?? existingRepair.Status;
            existingRepair.Description = updateDto.Description ?? existingRepair.Description;
            existingRepair.Notes = updateDto.Notes ?? existingRepair.Notes;
            //existingRepair.IsInternal = updateDto.IsInternal;

            if (!await MainUnitOfWork.RepairationRepository.UpdateAsync(existingRepair, AccountId, CurrentDate))
            {
                throw new ApiException("Cập nhật thông tin yêu cầu thất bại", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success("Cập nhật yêu cầu thành công");
        }
    }
}
