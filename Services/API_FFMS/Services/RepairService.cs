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
    public interface IRepairService : IBaseService
    {
        Task<ApiResponses<RepairDto>> GetRepairs(RepairQueryDto queryDto);
        Task<ApiResponse> CreateRepair(RepairCreateDto createDto);
        Task<ApiResponse> CreateRepairs(List<RepairCreateDto> createDtos);
        Task<ApiResponse<RepairDto>> GetRepair(Guid id);
        Task<ApiResponse> Update(Guid id, BaseRequestUpdateDto updateDto);
        Task<ApiResponse> Delete(Guid id);
        Task<ApiResponse> DeleteRepairs(DeleteMutilDto deleteDto);
        Task<ApiResponse> UpdateStatus(Guid id, BaseUpdateStatusDto updateStatusDto);
    }
    public class RepairService : BaseService, IRepairService
    {
        private readonly IRepairRepository _repairRepository;
        public RepairService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor,
                                 IMapperRepository mapperRepository, IRepairRepository repairationRepository)
                                 : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
            _repairRepository = repairationRepository;
        }

        public async Task<ApiResponse> CreateRepair(RepairCreateDto createDto)
        {
            var asset = await MainUnitOfWork.AssetRepository.FindOneAsync(createDto.AssetId);

            if (asset == null)
                throw new ApiException("Không cần tồn tại trang thiết bị", StatusCode.NOT_FOUND);

            if (asset.Status != AssetStatus.Operational && asset.Status != AssetStatus.Damaged)
                throw new ApiException("Trang thiết bị đang trong một yêu cầu khác", StatusCode.BAD_REQUEST);

            var checkExist = await MainUnitOfWork.RepairRepository.FindAsync(
                    new Expression<Func<Repair, bool>>[]
                    {
                            x => !x.DeletedAt.HasValue,
                            x => x.AssetId == createDto.AssetId,
                            x => x.Status != RequestStatus.Done
                    }, null);
            if (checkExist.Any())
            {
                throw new ApiException("Đã có yêu cầu sửa chữa cho thiết bị này", StatusCode.ALREADY_EXISTS);
            }

            var repairation = createDto.ProjectTo<RepairCreateDto, Repair>();
            repairation.RequestCode = GenerateRequestCode();

            var mediaFiles = new List<MediaFile>();
            if (createDto.RelatedFiles != null)
            {
                foreach (var file in createDto.RelatedFiles)
                {
                    var newMediaFile = new MediaFile
                    {
                        FileName = file.FileName ?? "",
                        Uri = file.Uri ?? "",
                        FileType = file.FileType
                    };
                    mediaFiles.Add(newMediaFile);
                }
            }

            if (!await _repairRepository.InsertRepairV2(repairation, mediaFiles, AccountId, CurrentDate))
                throw new ApiException("Tạo yêu cầu thất bại", StatusCode.SERVER_ERROR);

            return ApiResponse.Created("Gửi yêu cầu thành công");
        }

        public async Task<ApiResponse> CreateRepairs(List<RepairCreateDto> createDtos)
        {
            var assets = await MainUnitOfWork.AssetRepository.FindAsync(
                new Expression<Func<Asset, bool>>[]
                {
                    x => !x!.DeletedAt.HasValue,
                    x => createDtos.Select(dto => dto.AssetId).Contains(x.Id)
                }, null);

            if (assets.Any(asset => asset!.Status != AssetStatus.Operational))
            {
                throw new ApiException("Trang thiết bị đang trong một yêu cầu khác", StatusCode.SERVER_ERROR);
            }

            var repairs = createDtos.Select(dto => dto.ProjectTo<RepairCreateDto, Repair>())
                                         .ToList();

            if (!await _repairRepository.InsertRepairs(repairs, AccountId, CurrentDate))
                throw new ApiException("Tạo yêu cầu thất bại", StatusCode.SERVER_ERROR);

            return ApiResponse.Created("Gửi yêu cầu thành công");
        }

        public async Task<ApiResponse> Delete(Guid id)
        {
            var existingRepair = await MainUnitOfWork.RepairRepository.FindOneAsync(
                new Expression<Func<Repair, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id
                });
            if (existingRepair == null)
            {
                throw new ApiException("Không tìm thấy yêu cầu sửa chữa này", StatusCode.NOT_FOUND);
            }

            if (!await _repairRepository.DeleteRepair(existingRepair, AccountId, CurrentDate))
            {
                throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
            }
            return ApiResponse.Success();
        }

        public async Task<ApiResponse> DeleteRepairs(DeleteMutilDto deleteDto)
        {
            var replairDeleteds = await MainUnitOfWork.RepairRepository.FindAsync(
                new Expression<Func<Repair, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => deleteDto.ListId!.Contains(x.Id)
                }, null);

            var repairs = replairDeleteds.Where(r => r != null).ToList();

            if (!await _repairRepository.DeleteRepairs(repairs, AccountId, CurrentDate))
            {
                throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
            }
            return ApiResponse.Success();
        }

        public async Task<ApiResponse<RepairDto>> GetRepair(Guid id)
        {
            var repairation = await MainUnitOfWork.RepairRepository.FindOneAsync<RepairDto>(
                new Expression<Func<Repair, bool>>[]
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
                repairation.Asset.StatusObj = repairation.Asset.Status?.GetValue();
            }

            repairation.User = await MainUnitOfWork.UserRepository.FindOneAsync<UserBaseDto>(
                new Expression<Func<User, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == repairation.AssignedTo
                });
            if (repairation.User != null)
            {
                repairation.User.StatusObj = repairation.User.Status?.GetValue();
                repairation.User.RoleObj = repairation.User.Role?.GetValue();
            }

            var mediaFileQuery = MainUnitOfWork.MediaFileRepository.GetQuery().Where(m => m!.ItemId == repairation.Id);

            repairation.MediaFile = new MediaFileDto
            {
                FileType = mediaFileQuery.Select(m => m!.FileType).FirstOrDefault(),
                Uri = mediaFileQuery.Select(m => m!.Uri).ToList(),
                Content = mediaFileQuery.Select(m => m!.Content).FirstOrDefault()
            };

            repairation.PriorityObj = repairation.Priority.GetValue();
            repairation.Status = repairation.Status;
            repairation.StatusObj = repairation.Status!.GetValue();

            repairation = await _mapperRepository.MapCreator(repairation);

            return ApiResponse<RepairDto>.Success(repairation);
        }

        public async Task<ApiResponses<RepairDto>> GetRepairs(RepairQueryDto queryDto)
        {
            var keyword = queryDto.Keyword?.Trim().ToLower();

            var repairQuery = MainUnitOfWork.RepairRepository.GetQuery()
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
                                                         || x.Notes!.ToLower().Contains(keyword)
                                                         || x.RequestCode.ToLower().Contains(keyword));
            }

            repairQuery = repairQuery.OrderByDescending(x => x!.CreatedAt);

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

            var repairations = await joinTable.Select(x => new RepairDto
            {
                Id = x.Repairation.Id,
                RequestCode = x.Repairation.RequestCode,
                RequestDate = x.Repairation.RequestDate,
                CompletionDate = x.Repairation.CompletionDate,
                Status = x.Repairation.Status,
                StatusObj = x.Repairation.Status.GetValue(),
                Description = x.Repairation.Description,
                Notes = x.Repairation.Notes,
                Checkin = x.Repairation.Checkin,
                Result = x.Repairation.Result,
                Checkout = x.Repairation.Checkout,
                Priority = x.Repairation.Priority,
                PriorityObj = x.Repairation.Priority.GetValue(),
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

            return ApiResponses<RepairDto>.Success(
                repairations,
                totalCount,
                queryDto.PageSize,
                queryDto.Page,
                (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
        }

        public async Task<ApiResponse> Update(Guid id, BaseRequestUpdateDto updateDto)
        {
            var existingRepair = await MainUnitOfWork.RepairRepository.FindOneAsync(id);
            if (existingRepair == null)
            {
                throw new ApiException("Không tìm thấy yêu cầu", StatusCode.NOT_FOUND);
            }

            if (existingRepair.Status != RequestStatus.InProgress)
            {
                throw new ApiException("Chỉ được cập nhật các yêu cầu chưa hoàn thành", StatusCode.NOT_FOUND);
            }

            existingRepair.Description = updateDto.Description ?? existingRepair.Description;
            existingRepair.Notes = updateDto.Notes ?? existingRepair.Notes;
            existingRepair.Priority = updateDto.Priority ?? existingRepair.Priority;

            if (!await MainUnitOfWork.RepairRepository.UpdateAsync(existingRepair, AccountId, CurrentDate))
            {
                throw new ApiException("Cập nhật thông tin yêu cầu thất bại", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success("Cập nhật yêu cầu thành công");
        }

        public async Task<ApiResponse> UpdateStatus(Guid id, BaseUpdateStatusDto requestStatus)
        {
            var existingRepair = MainUnitOfWork.RepairRepository.GetQuery()
                                    .Include(t => t!.Asset)
                                    .Where(t => t!.Id == id)
                                    .FirstOrDefault();
            if (existingRepair == null)
            {
                throw new ApiException("Không tìm thấy yêu cầu sửa chữa này", StatusCode.NOT_FOUND);
            }

            existingRepair.Status = requestStatus.Status ?? existingRepair.Status;

            if (!await _repairRepository.UpdateStatus(existingRepair, requestStatus.Status, AccountId, CurrentDate))
            {
                throw new ApiException("Cập nhật trạng thái yêu cầu thất bại", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success("Cập nhật yêu cầu thành công");
        }

        public string GenerateRequestCode()
        {
            var requests = MainUnitOfWork.RepairRepository.GetQueryAll().ToList();

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
