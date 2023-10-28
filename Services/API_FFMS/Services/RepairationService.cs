using API_FFMS.Dtos;
using API_FFMS.Repositories;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API_FFMS.Services
{
    public interface IRepairationService : IBaseService
    {
        Task<ApiResponses<RepairationDto>> GetRepairations(RepairationQueryDto queryDto);
        Task<ApiResponse> CreateRepairation(RepairationCreateDto createDto);
        Task<ApiResponse> CreateRepairations(List<RepairationCreateDto> createDtos);
        Task<ApiResponse<RepairationDto>> GetRepairation(Guid id);
        Task<ApiResponse> Update(Guid id, BaseRequestUpdateDto updateDto);
        Task<ApiResponse> Delete(Guid id);
        Task<ApiResponse> DeleteReplairations(List<Guid> ids);
        Task<ApiResponse> UpdateStatus(Guid id, BaseUpdateStatusDto updateStatusDto);
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
            repairation.RequestCode = GenerateRequestCode();

            if (!await _repairationRepository.InsertRepairation(repairation, AccountId, CurrentDate))
                throw new ApiException("Tạo yêu cầu thất bại", StatusCode.SERVER_ERROR);

            return ApiResponse.Created("Gửi yêu cầu thành công");
        }

        public async Task<ApiResponse> CreateRepairations(List<RepairationCreateDto> createDtos)
        {
            var assets = await MainUnitOfWork.AssetRepository.FindAsync(
                new Expression<Func<Asset, bool>>[]
                {
                    x => !x!.DeletedAt.HasValue,
                    x => createDtos.Select(dto => dto.AssetId).Contains(x.Id)
                }, null);

            foreach (var asset in assets)
            {
                if (asset!.Status != AssetStatus.Operational)
                {
                    throw new ApiException("Trang thiết bị đang trong một yêu cầu khác", StatusCode.SERVER_ERROR);
                }
            }

            var repairations = new List<Repairation>();
            foreach( var createDto in createDtos)
            {
                var repairation = createDto.ProjectTo<RepairationCreateDto, Repairation>();
            }

            if (repairations != null)
            {
                if (!await _repairationRepository.InsertRepairations(repairations, AccountId, CurrentDate))
                    throw new ApiException("Tạo yêu cầu thất bại", StatusCode.SERVER_ERROR);

                return ApiResponse.Created("Gửi yêu cầu thành công");
            }

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

            var mediaFileQuery = MainUnitOfWork.MediaFileRepository.GetQuery().Where(m => m!.ItemId == repairation.Id);

            repairation.MediaFile = new MediaFileDto
            {
                FileType = mediaFileQuery.Select(m => m!.FileType).FirstOrDefault(),
                Uri = mediaFileQuery.Select(m => m!.Uri).ToList(),
                Content = mediaFileQuery.Select(m => m!.Content).FirstOrDefault()
            };

            repairation.Status = repairation.Status;
            repairation.StatusObj = repairation.Status!.GetValue();

            repairation = await _mapperRepository.MapCreator(repairation);

            return ApiResponse<RepairationDto>.Success(repairation);
        }

        public async Task<ApiResponses<RepairationDto>> GetRepairations(RepairationQueryDto queryDto)
        {
            var keyword = queryDto.Keyword?.Trim().ToLower();

            var repairQuery = MainUnitOfWork.RepairationRepository.GetQuery()
                              .Where(x => !x!.DeletedAt.HasValue);

            if (queryDto.IsInternal != null)
            {
                repairQuery = repairQuery.Where(x => x!.IsInternal == queryDto.IsInternal);
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

            if (!string.IsNullOrEmpty(keyword))
            {
                repairQuery = repairQuery.Where(x => x!.Description!.ToLower().Contains(keyword)
                                                                   || x.Notes!.ToLower().Contains(keyword) ||
                                                                   x.RequestCode.ToLower().Contains(keyword));
            }

            var assetTypeQueryable = MainUnitOfWork.AssetTypeRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);
            var categoryQueryable = MainUnitOfWork.CategoryRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);

            var joinTable = from repair in repairQuery
                            join user in MainUnitOfWork.UserRepository.GetQuery() on repair.AssignedTo equals user.Id
                            join asset in MainUnitOfWork.AssetRepository.GetQuery() on repair.AssetId equals asset.Id
                            join assetType in assetTypeQueryable on asset.TypeId equals assetType.Id into assetTypeGroup
                            from assetType in assetTypeGroup.DefaultIfEmpty()
                            join category in categoryQueryable on assetType.CategoryId equals category.Id into categoryGroup
                            from category in categoryGroup.DefaultIfEmpty()
                            select new
                            {
                                Repairation = repair,
                                Asset = asset,
                                User = user,
                                AssetType = assetType,
                                Category = category
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
                StatusObj = x.Repairation.Status!.GetValue(),
                Description = x.Repairation.Description,
                Notes = x.Repairation.Notes,
                Checkin = x.Repairation.Checkin,
                Checkout = x.Repairation.Checkout,
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
                },
                AssetType = x.AssetType.ProjectTo<AssetType, AssetTypeDto>(),
                Category = x.Category.ProjectTo<Category, CategoryDto>()
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

            if (existingRepair.Status != RequestStatus.InProgress)
            {
                throw new ApiException("Chỉ được cập nhật các yêu cầu chưa hoàn thành", StatusCode.NOT_FOUND);
            }

            existingRepair.RequestDate = updateDto.RequestDate ?? existingRepair.RequestDate;
            existingRepair.CompletionDate = updateDto.CompletionDate ?? existingRepair.CompletionDate;
            existingRepair.Description = updateDto.Description ?? existingRepair.Description;
            existingRepair.Notes = updateDto.Notes ?? existingRepair.Notes;
            existingRepair.Piority = updateDto.Piority ?? existingRepair.Piority;

            if (!await MainUnitOfWork.RepairationRepository.UpdateAsync(existingRepair, AccountId, CurrentDate))
            {
                throw new ApiException("Cập nhật thông tin yêu cầu thất bại", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success("Cập nhật yêu cầu thành công");
        }

        public async Task<ApiResponse> UpdateStatus(Guid id, BaseUpdateStatusDto requestStatus)
        {
            var existingRepair = MainUnitOfWork.RepairationRepository.GetQuery()
                                    .Include(t => t!.Asset)
                                    .Where(t => t!.Id == id)
                                    .FirstOrDefault();
            if (existingRepair == null)
            {
                throw new ApiException("Không tìm thấy yêu cầu sửa chữa này", StatusCode.NOT_FOUND);
            }

            existingRepair.Status = requestStatus.Status ?? existingRepair.Status;

            if (!await _repairationRepository.UpdateStatus(existingRepair, requestStatus.Status, AccountId, CurrentDate))
            {
                throw new ApiException("Cập nhật trạng thái yêu cầu thất bại", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success("Cập nhật yêu cầu thành công");
        }

        public string GenerateRequestCode()
        {
            var requests = MainUnitOfWork.RepairationRepository.GetQueryAll().ToList();

            var numbers = new List<int>();
            foreach (var t in requests)
            {
                int.TryParse(t!.RequestCode[3..], out int lastNumber);
                numbers.Add(lastNumber);
            }

            string newRequestCode = "REP1";

            if (requests.Any())
            {
                var lastCode = numbers.AsQueryable().OrderDescending().FirstOrDefault();
                if (requests.Any(x => x!.RequestCode.StartsWith("REP")))
                {
                    newRequestCode = $"REP{lastCode + 1}";
                }
            }
            return newRequestCode;
        }
    }
}
