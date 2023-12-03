using API_FFMS.Dtos;
using API_FFMS.Repositories;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace API_FFMS.Services
{
    public interface IRepairService : IBaseService
    {
        Task<ApiResponses<RepairDto>> GetRepairs(RepairQueryDto queryDto);
        Task<ApiResponse> CreateRepair(RepairCreateDto createDto);
        Task<ApiResponse> CreateMulti(List<RepairCreateDto> createDtos);
        Task<ApiResponse<RepairDto>> GetRepair(Guid id);
        Task<ApiResponse> Update(Guid id, BaseRequestUpdateDto updateDto);
        Task<ApiResponse> Delete(Guid id);
        Task<ApiResponse> DeleteMulti(DeleteMutilDto deleteDto);
        Task<ApiResponse> ConfirmOrReject(Guid id, BaseUpdateStatusDto confirmOrRejectDto);
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

            if (asset.RequestStatus != RequestType.Operational)
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
            repairation.Description = createDto.Description ?? "Yêu cầu sửa chữa";
            repairation.RequestCode = GenerateRequestCode();

            // For storing json in column
            var mediaFiles = new List<Report>();
            if (createDto.RelatedFiles != null)
            {
                var listUrisJson = JsonConvert.SerializeObject(createDto.RelatedFiles);
                var report = new Report
                {
                    FileName = string.Empty,
                    Uri = listUrisJson,
                    Content = string.Empty,
                    FileType = FileType.File,
                    ItemId = repairation.Id,
                    IsVerified = false,
                    IsReported = false,
                };
        
                mediaFiles.Add(report);
            }

            if (!await _repairRepository.InsertRepair(repairation, mediaFiles, AccountId, CurrentDate))
                throw new ApiException("Tạo yêu cầu thất bại", StatusCode.SERVER_ERROR);

            return ApiResponse.Created("Gửi yêu cầu thành công");
        }

        public async Task<ApiResponse> CreateMulti(List<RepairCreateDto> createDtos)
        {
            var assets = await MainUnitOfWork.AssetRepository.FindAsync(
                new Expression<Func<Asset, bool>>[]
                {
                    x => !x!.DeletedAt.HasValue,
                    x => createDtos.Select(dto => dto.AssetId).Contains(x.Id)
                }, null);

            if (assets.Any(asset => asset!.RequestStatus != RequestType.Operational))
            {
                throw new ApiException("Trang thiết bị đang trong một yêu cầu khác", StatusCode.SERVER_ERROR);
            }

            var repairs = new List<Repair>();
            var relatedFiles = new List<Report>();

            foreach (var create in createDtos)
            {
                var repair = create.ProjectTo<RepairCreateDto, Repair>();
                repair.Description = create.Description ?? "Yêu cầu sửa chữa";
                repair.Id = Guid.NewGuid();
                if (create.RelatedFiles != null)
                {
                    foreach (var file in create.RelatedFiles)
                    {
                        var relatedFile = new Report
                        {
                            Id = Guid.NewGuid(),
                            FileName = file.FileName ?? "",
                            Uri = file.Uri ?? "",
                            CreatedAt = CurrentDate,
                            CreatorId = AccountId,
                            ItemId = repair.Id
                        };
                        relatedFiles.Add(relatedFile);
                    }
                }
                repairs.Add(repair);
            }

            if (!await _repairRepository.CreateMulti(repairs, relatedFiles, AccountId, CurrentDate))
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

            if(existingRepair.Status != RequestStatus.Done &&
               existingRepair.Status != RequestStatus.NotStart &&
               existingRepair.Status != RequestStatus.Cancelled)
            {
                throw new ApiException($"Không thể xóa yêu cầu đang có trạng thái: {existingRepair.Status?.GetDisplayName()}", StatusCode.BAD_REQUEST);
            }

            if (!await _repairRepository.DeleteRepair(existingRepair, AccountId, CurrentDate))
            {
                throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
            }
            return ApiResponse.Success();
        }

        public async Task<ApiResponse> DeleteMulti(DeleteMutilDto deleteDto)
        {
            var replairDeleteds = await MainUnitOfWork.RepairRepository.FindAsync(
                new Expression<Func<Repair, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => deleteDto.ListId!.Contains(x.Id)
                }, null);

            var repairs = replairDeleteds.Where(r => r != null).ToList();

            foreach (var repair in repairs)
            {
                if (repair!.Status != RequestStatus.Done &&
                    repair.Status != RequestStatus.NotStart &&
                    repair.Status != RequestStatus.Cancelled)
                {
                    throw new ApiException($"Không thể xóa yêu cầu đang có trạng thái: {repair.Status?.GetDisplayName()}" +
                                           $"kiểm tra yêu cầu: {repair.RequestCode}", StatusCode.BAD_REQUEST);
                }
            }

            if (!await _repairRepository.DeleteMulti(repairs, AccountId, CurrentDate))
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
                repairation.Asset.Status= repairation.Asset.Status;
                repairation.Asset.StatusObj = repairation.Asset.Status?.GetValue();
                repairation.Asset.RequestStatus = repairation.Asset.RequestStatus;
                repairation.Asset.RequestStatusObj = repairation.Asset.RequestStatus?.GetValue();
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

            //Related file
            var relatedMediaFiles = await MainUnitOfWork.MediaFileRepository.GetQuery()
                .Where(m => m!.ItemId == id && !m.IsReported).FirstOrDefaultAsync();

            if (relatedMediaFiles != null)
            {
                repairation.RelatedFiles = JsonConvert.DeserializeObject<List<MediaFileDetailDto>>(relatedMediaFiles.Uri);
            }

            var reports = await MainUnitOfWork.MediaFileRepository.GetQuery()
                .Where(m => m!.ItemId == id && m.IsReported).OrderByDescending(x => x!.CreatedAt).ToListAsync();

            repairation.Reports = new List<MediaFileDto>();
            foreach (var report in reports)
            {
                // Deserialize the URI string back into a List<string>
                var uriList = new List<string>();
                if (report?.Uri != null)
                {
                    uriList = JsonConvert.DeserializeObject<List<string>>(report.Uri);
                }
                
                repairation.Reports.Add(new MediaFileDto
                {
                    ItemId = report.ItemId,
                    Uri = uriList,
                    FileType = report.FileType,
                    Content = report.Content,
                    IsReject = report.IsReject,
                    RejectReason = report.RejectReason
                });
            }

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

            if (queryDto.Priority != null)
            {
                repairQuery = repairQuery.Where(x => x!.Priority == queryDto.Priority);
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
                    RequestStatus = x.Asset.RequestStatus,
                    RequestStatusObj = x.Asset.RequestStatus.GetValue(),
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
            try
            {
                var existingRepair = await MainUnitOfWork.RepairRepository.FindOneAsync(id);
                if (existingRepair == null)
                {
                    throw new ApiException("Không tìm thấy yêu cầu", StatusCode.NOT_FOUND);
                }

                if (existingRepair.Status != RequestStatus.NotStart)
                {
                    throw new ApiException("Chỉ được cập nhật yêu cầu đang có trạng thái chưa bắt đầu", StatusCode.NOT_FOUND);
                }

                existingRepair.Description = updateDto.Description ?? existingRepair.Description;
                existingRepair.Notes = updateDto.Notes ?? existingRepair.Notes;
                existingRepair.CategoryId = updateDto.CategoryId ?? existingRepair.CategoryId;
                existingRepair.IsInternal = updateDto.IsInternal ?? existingRepair.IsInternal;
                existingRepair.AssetTypeId = updateDto.AssetTypeId ?? existingRepair.AssetTypeId;
                existingRepair.AssignedTo = updateDto.AssignedTo ?? existingRepair.AssignedTo;
                existingRepair.Priority = updateDto.Priority ?? existingRepair.Priority;
                existingRepair.AssetId = updateDto.AssetId ?? existingRepair.AssetId;

                var mediaFileQuery = MainUnitOfWork.MediaFileRepository.GetQuery().Where(x => x!.ItemId == id).ToList();

                var newMediaFile = updateDto.RelatedFiles.Select(dto => new Report
                {
                    FileName = dto.FileName,
                    Uri = dto.Uri,
                    CreatedAt = CurrentDate,
                    CreatorId = AccountId,
                    ItemId = id,
                    FileType = FileType.File
                }).ToList() ?? new List<Report>();

                var additionMediaFiles = newMediaFile.Except(mediaFileQuery).ToList();
                var removalMediaFiles = mediaFileQuery.Except(newMediaFile).ToList();

                if (!await _repairRepository.UpdateRepair(existingRepair, additionMediaFiles, removalMediaFiles, AccountId, CurrentDate))
                {
                    throw new ApiException("Cập nhật thông tin yêu cầu thất bại", StatusCode.SERVER_ERROR);
                }

                return ApiResponse.Success("Cập nhật yêu cầu thành công");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ApiResponse> ConfirmOrReject(Guid id, BaseUpdateStatusDto confirmOrRejectDto)
        {
            var existingRepair = MainUnitOfWork.RepairRepository.GetQuery()
                                    .Include(t => t!.Asset)
                                    .Where(t => t!.Id == id)
                                    .FirstOrDefault();
            if (existingRepair == null)
            {
                throw new ApiException("Không tìm thấy yêu cầu sửa chữa này", StatusCode.NOT_FOUND);
            }

            existingRepair.Status = confirmOrRejectDto.Status ?? existingRepair.Status;

            if (!await _repairRepository.ConfirmOrReject(existingRepair, confirmOrRejectDto, AccountId, CurrentDate))
            {
                throw new ApiException("Xác nhận trạng thái yêu cầu thất bại", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success("Xác nhận yêu cầu thành công");
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
