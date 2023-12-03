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
    public interface IReplacementService : IBaseService
    {
        Task<ApiResponse> Create(ReplaceCreateDto createDto);
        Task<ApiResponse<ReplaceDto>> GetReplacement(Guid id);
        Task<ApiResponse> Update(Guid id, ReplaceUpdateDto updateDto);
        Task<ApiResponse> Delete(Guid id);
        Task<ApiResponses<ReplaceDto>> GetReplaces(ReplacementQueryDto queryDto);
        Task<ApiResponse> DeleteMulti(DeleteMutilDto deleteDto);
        Task<ApiResponse> ConfirmOrReject(Guid id, BaseUpdateStatusDto confirmOrRejectDto);
    }
    public class ReplacementService : BaseService, IReplacementService
    {
        private readonly IReplacementRepository _repository;
        public ReplacementService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor,
                                  IMapperRepository mapperRepository, IReplacementRepository repository)
                                  : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse> Create(ReplaceCreateDto createDto)
        {
            var asset = await MainUnitOfWork.AssetRepository.FindOneAsync(createDto.AssetId);
            if (asset == null)
            {
                throw new ApiException("Không tìm thấy trang thiết bị cần thay thế", StatusCode.NOT_FOUND);
            }

            var newAsset = await MainUnitOfWork.AssetRepository.FindOneAsync(createDto.NewAssetId);
            if (newAsset == null)
            {
                throw new ApiException("Không tìm thấy trang thiết bị để thay thế", StatusCode.NOT_FOUND);
            }

            if (newAsset.RequestStatus != RequestType.Operational)
            {
                throw new ApiException($"Trang thiết bị {newAsset.AssetCode} đang trong một yêu cầu khác", StatusCode.BAD_REQUEST);
            }

            var checkExist = await MainUnitOfWork.ReplacementRepository.FindAsync(
                new Expression<Func<Replacement, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.AssetId == createDto.AssetId,
                    x => x.Status != RequestStatus.Done
                }, null);
            if (checkExist.Any())
            {
                throw new ApiException("Đã có yêu cầu thay thế cho thiết bị này", StatusCode.ALREADY_EXISTS);
            }

            var replacement = createDto.ProjectTo<ReplaceCreateDto, Replacement>();
            replacement.Description = createDto.Description ?? "Yêu cầu thay thế";
            replacement.RequestCode = GenerateRequestCode();

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
                    ItemId = replacement.Id,
                    IsVerified = false,
                    IsReported = false,
                };
        
                mediaFiles.Add(report);
            }

            if (!await _repository.InsertReplacement(replacement, mediaFiles, AccountId, CurrentDate))
            {
                throw new ApiException("Tạo yêu cầu thất bại", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Created("Gửi yêu cầu thành công");
        }

        public async Task<ApiResponse> Delete(Guid id)
        {
            var existingReplace = await MainUnitOfWork.ReplacementRepository.FindOneAsync(
                new Expression<Func<Replacement, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id
                });
            if (existingReplace == null)
            {
                throw new ApiException("Không tìm thấy yêu cầu thay thế này", StatusCode.NOT_FOUND);
            }

            if (existingReplace.Status != RequestStatus.Done &&
               existingReplace.Status != RequestStatus.NotStart &&
               existingReplace.Status != RequestStatus.Cancelled)
            {
                throw new ApiException($"Không thể xóa yêu cầu đang có trạng thái: {existingReplace.Status?.GetDisplayName()}", StatusCode.NOT_FOUND);
            }

            if (!await _repository.DeleteReplacement(existingReplace, AccountId, CurrentDate))
            {
                throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
            }
            return ApiResponse.Success();
        }

        public async Task<ApiResponse> DeleteMulti(DeleteMutilDto deleteDto)
        {
            var replaceDeleteds = await MainUnitOfWork.ReplacementRepository.FindAsync(
                                    new Expression<Func<Replacement, bool>>[]
                                    {
                                        x => !x.DeletedAt.HasValue,
                                        x => deleteDto.ListId!.Contains(x.Id)
                                    }, null);

            var replacements = replaceDeleteds.Where(r => r != null).ToList();

            foreach (var replacement in replacements)
            {
                if (replacement!.Status != RequestStatus.Done &&
                    replacement.Status != RequestStatus.NotStart &&
                    replacement.Status != RequestStatus.Cancelled)
                {
                    throw new ApiException($"Không thể xóa yêu cầu đang có trạng thái: {replacement.Status?.GetDisplayName()}" +
                                           $"kiểm tra yêu cầu: {replacement.RequestCode}", StatusCode.BAD_REQUEST);
                }
            }

            if (!await _repository.DeleteMulti(replaceDeleteds, AccountId, CurrentDate))
            {
                throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
            }
            return ApiResponse.Success();
        }

        public async Task<ApiResponse<ReplaceDto>> GetReplacement(Guid id)
        {
            var replacement = await MainUnitOfWork.ReplacementRepository.FindOneAsync<ReplaceDto>(
                new Expression<Func<Replacement, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id
                });

            if (replacement == null)
            {
                throw new ApiException("Không tìm thấy yêu cầu thay thế", StatusCode.NOT_FOUND);
            }

            var assetQueryable = MainUnitOfWork.AssetRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);

            replacement.Asset = await MainUnitOfWork.AssetRepository.FindOneAsync<AssetBaseDto>(
                new Expression<Func<Asset, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == replacement.AssetId
                });
            if (replacement.Asset != null)
            {
                replacement.Asset.StatusObj = replacement.Asset.Status?.GetValue();
                replacement.Asset.RequestStatusObj = replacement.Asset.RequestStatus.GetValue();
                var location = await MainUnitOfWork.RoomAssetRepository.FindOneAsync<RoomAsset>(
                    new Expression<Func<RoomAsset, bool>>[]
                    {
                        x => !x.DeletedAt.HasValue,
                        x => x.AssetId == replacement.Asset.Id
                    });
                if (location != null)
                {
                    replacement.AssetLocation = await MainUnitOfWork.RoomRepository.FindOneAsync<AssetLocation>(
                            new Expression<Func<Room, bool>>[]
                            {
                                x => !x.DeletedAt.HasValue,
                                x => x.Id == location.RoomId
                            });
                    replacement.StatusBefore = location.Status;
                    replacement.StatusBeforeObj = location.Status.GetValue();
                }
            }

            replacement.NewAsset = await MainUnitOfWork.AssetRepository.FindOneAsync<AssetBaseDto>(
                new Expression<Func<Asset, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == replacement.NewAssetId
                });
            if (replacement.NewAsset != null)
            {
                replacement.NewAsset.StatusObj = replacement.NewAsset.Status?.GetValue();
                replacement.NewAsset.RequestStatusObj = replacement.NewAsset.RequestStatus.GetValue();

                var location = await MainUnitOfWork.RoomAssetRepository.FindOneAsync<RoomAsset>(
                    new Expression<Func<RoomAsset, bool>>[]
                    {
                        x => !x.DeletedAt.HasValue,
                        x => x.AssetId == replacement.NewAsset.Id
                    });
                if (location != null)
                {
                    replacement.NewAssetLocation = await MainUnitOfWork.RoomRepository.FindOneAsync<AssetLocation>(
                            new Expression<Func<Room, bool>>[]
                            {
                                x => !x.DeletedAt.HasValue,
                                x => x.Id == location.RoomId
                            });
                }
            }

            replacement.AssetType = await MainUnitOfWork.AssetTypeRepository.FindOneAsync<AssetTypeDto>(
                new Expression<Func<AssetType, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == replacement.Asset!.TypeId
                });

            replacement.Category = await MainUnitOfWork.CategoryRepository.FindOneAsync<CategoryDto>(
                new Expression<Func<Category, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == replacement.AssetType!.CategoryId
                });

            var relatedMediaFiles = await MainUnitOfWork.MediaFileRepository.GetQuery()
                .Where(m => m!.ItemId == id && !m.IsReported).FirstOrDefaultAsync();

            if (relatedMediaFiles != null)
            {
                replacement.RelatedFiles = JsonConvert.DeserializeObject<List<MediaFileDetailDto>>(relatedMediaFiles.Uri);
            }

            var reports = await MainUnitOfWork.MediaFileRepository.GetQuery()
                .Where(m => m!.ItemId == id && m.IsReported).OrderByDescending(x => x!.CreatedAt).ToListAsync();

            //TODO: orderby
            replacement.Reports = new List<MediaFileDto>();
            foreach (var report in reports)
            {
                // Deserialize the URI string back into a List<string>
                var uriList = JsonConvert.DeserializeObject<List<string>>(report.Uri);
            
                replacement.Reports.Add(new MediaFileDto
                {
                    ItemId = report.ItemId,
                    Uri = uriList,
                    FileType = report.FileType,
                    Content = report.Content,
                    IsReject = report.IsReject,
                    RejectReason = report.RejectReason
                });
            }

            replacement.AssignTo = await MainUnitOfWork.UserRepository.FindOneAsync<UserBaseDto>(
            new Expression<Func<User, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.Id == replacement.AssignedTo
            });

            replacement.PriorityObj = replacement.Priority.GetValue();
            replacement.Status = replacement.Status;
            replacement.StatusObj = replacement.Status!.GetValue();

            replacement = await _mapperRepository.MapCreator(replacement);

            return ApiResponse<ReplaceDto>.Success(replacement);
        }

        public async Task<ApiResponses<ReplaceDto>> GetReplaces(ReplacementQueryDto queryDto)
        {
            var keyword = queryDto.Keyword?.Trim().ToLower();
            var replaceQuery = MainUnitOfWork.ReplacementRepository.GetQuery()
                               .Where(x => !x!.DeletedAt.HasValue);

            if (queryDto.IsInternal != null)
            {
                replaceQuery = replaceQuery.Where(x => x!.IsInternal == queryDto.IsInternal);
            }

            if (queryDto.AssignedTo != null)
            {
                replaceQuery = replaceQuery.Where(x => x!.AssignedTo == queryDto.AssignedTo);
            }

            if (queryDto.AssetId != null)
            {
                replaceQuery = replaceQuery.Where(x => x!.AssetId == queryDto.AssetId);
            }

            if (queryDto.Status != null)
            {
                replaceQuery = replaceQuery.Where(x => x!.Status == queryDto.Status);
            }

            if (queryDto.Priority != null)
            {
                replaceQuery = replaceQuery.Where(x => x!.Priority == queryDto.Priority);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                replaceQuery = replaceQuery.Where(x => x!.Description!.ToLower().Contains(keyword)
                                                                   || x.Notes!.ToLower().Contains(keyword) ||
                                                                   x.RequestCode.ToLower().Contains(keyword));
            }

            replaceQuery = replaceQuery.OrderByDescending(x => x!.CreatedAt);

            var assetTypeQueryable = MainUnitOfWork.AssetTypeRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);

            var categoryQueryable = MainUnitOfWork.CategoryRepository.GetQuery()
                .Where(x => !x!.DeletedAt.HasValue);

            var joinTables = from replace in replaceQuery
                             join asset in MainUnitOfWork.AssetRepository.GetQuery() on replace.AssetId equals asset.Id
                             join newAsset in MainUnitOfWork.AssetRepository.GetQuery() on replace.NewAssetId equals newAsset.Id
                             join assetType in assetTypeQueryable on asset.TypeId equals assetType.Id into assetTypeGroup
                             from assetType in assetTypeGroup.DefaultIfEmpty()
                             join category in categoryQueryable on assetType.CategoryId equals category.Id into categoryGroup
                             from category in categoryGroup.DefaultIfEmpty()
                             select new
                             {
                                 Replacement = replace,
                                 Asset = asset,
                                 NewAsset = newAsset,
                                 AssetType = assetType,
                                 Category = category
                             };

            var totalCount = await joinTables.CountAsync();
            joinTables = joinTables.Skip(queryDto.Skip()).Take(queryDto.PageSize);

            var replacements = await joinTables.Select(x => new ReplaceDto
            {
                Id = x.Replacement.Id,
                RequestCode = x.Replacement.RequestCode,
                RequestDate = x.Replacement.RequestDate,
                CompletionDate = x.Replacement.CompletionDate,
                Status = x.Replacement.Status,
                StatusObj = x.Replacement.Status!.GetValue(),
                Description = x.Replacement.Description,
                Notes = x.Replacement.Notes,
                IsInternal = x.Replacement.IsInternal,
                AssignedTo = x.Replacement.AssignedTo,
                Checkout = x.Replacement.Checkout,
                PriorityObj = x.Replacement.Priority.GetValue(),
                Checkin = x.Replacement.Checkin,
                Result = x.Replacement.Result,
                AssetId = x.Replacement.AssetId,
                NewAssetId = x.Replacement.NewAssetId,
                CreatedAt = x.Replacement.CreatedAt,
                EditedAt = x.Replacement.EditedAt,
                CreatorId = x.Replacement.CreatorId ?? Guid.Empty,
                EditorId = x.Replacement.EditorId ?? Guid.Empty,
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
                NewAsset = new AssetBaseDto
                {
                    Id = x.NewAsset.Id,
                    AssetName = x.NewAsset.AssetName,
                    AssetCode = x.NewAsset.AssetCode,
                    IsMovable = x.NewAsset.IsMovable,
                    Status = x.NewAsset.Status,
                    StatusObj = x.NewAsset.Status.GetValue(),
                    RequestStatus = x.NewAsset.RequestStatus,
                    RequestStatusObj = x.NewAsset.RequestStatus.GetValue(),
                    ManufacturingYear = x.NewAsset.ManufacturingYear,
                    SerialNumber = x.NewAsset.SerialNumber,
                    Quantity = x.NewAsset.Quantity,
                    Description = x.NewAsset.Description,
                    LastCheckedDate = x.NewAsset.LastCheckedDate,
                    LastMaintenanceTime = x.NewAsset.LastMaintenanceTime,
                    TypeId = x.NewAsset.TypeId,
                    ModelId = x.NewAsset.ModelId,
                    IsRented = x.NewAsset.IsRented,
                    StartDateOfUse = x.NewAsset.StartDateOfUse
                },
                AssetType = x.AssetType.ProjectTo<AssetType, AssetTypeDto>(),
                Category = x.Category.ProjectTo<Category, CategoryDto>()
            }).ToListAsync();

            replacements = await _mapperRepository.MapCreator(replacements);

            return ApiResponses<ReplaceDto>.Success(
                replacements,
                totalCount,
                queryDto.PageSize,
                queryDto.Page,
                (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
        }


        public async Task<ApiResponse> Update(Guid id, ReplaceUpdateDto updateDto)
        {
            var existingReplace = await MainUnitOfWork.ReplacementRepository.FindOneAsync(id);
            if (existingReplace == null)
            {
                throw new ApiException("Không tìm thấy yêu cầu thay thế này", StatusCode.NOT_FOUND);
            }

            if (existingReplace.Status != RequestStatus.NotStart)
            {
                throw new ApiException("Chỉ được cập nhật yêu cầu đang có trạng thái chưa bắt đầu", StatusCode.BAD_REQUEST);
            }

            existingReplace.Description = updateDto.Description ?? existingReplace.Description;
            existingReplace.Notes = updateDto.Notes ?? existingReplace.Notes;
            existingReplace.Priority = updateDto.Priority ?? existingReplace.Priority;
            existingReplace.AssetId = updateDto.AssetId ?? existingReplace.AssetId;
            existingReplace.NewAssetId = updateDto.NewAssetId ?? existingReplace.NewAssetId;

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

            if (!await _repository.UpdateReplacement(existingReplace, additionMediaFiles, removalMediaFiles, AccountId, CurrentDate))
            {
                throw new ApiException("Cập nhật thông tin yêu cầu thất bại", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success("Cập nhật yêu cầu thành công");
        }

        public async Task<ApiResponse> ConfirmOrReject(Guid id, BaseUpdateStatusDto confirmOrRejectDto)
        {
            var existingReplace = MainUnitOfWork.ReplacementRepository.GetQuery()
                                    .Include(r => r!.Asset)
                                    .Where(r => r!.Id == id)
                                    .FirstOrDefault();

            if (existingReplace == null)
            {
                throw new ApiException("Không tìm thấy yêu cầu thay thế này", StatusCode.NOT_FOUND);
            }

            existingReplace.Status = confirmOrRejectDto.Status ?? existingReplace.Status;

            if (!await _repository.ConfirmOrReject(existingReplace, confirmOrRejectDto, AccountId, CurrentDate))
            {
                throw new ApiException("Cập nhật trạng thái yêu cầu thất bại", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success("Cập nhật yêu cầu thành công");
        }

        public string GenerateRequestCode()
        {
            var requests = MainUnitOfWork.ReplacementRepository.GetQueryAll().ToList();

            var numbers = new List<int>();
            foreach (var t in requests)
            {
                int.TryParse(t!.RequestCode[3..], out int lastNumber);
                numbers.Add(lastNumber);
            }

            string newRequestCode = "RPL1";

            if (requests.Any())
            {
                var lastCode = numbers.AsQueryable().OrderDescending().FirstOrDefault();
                if (requests.Any(x => x!.RequestCode.StartsWith("RPL")))
                {
                    newRequestCode = $"RPL{lastCode + 1}";
                }
            }
            return newRequestCode;
        }
    }
}