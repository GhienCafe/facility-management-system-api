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
    public interface IReplacementService : IBaseService
    {
        Task<ApiResponse> Create(ReplaceCreateDto createDto);
        Task<ApiResponse<ReplaceDto>> GetReplacement(Guid id);
        Task<ApiResponse> Update(Guid id, BaseRequestUpdateDto updateDto);
        Task<ApiResponse> Delete(Guid id);
        Task<ApiResponses<ReplaceDto>> GetReplaces(ReplacementQueryDto queryDto);
        Task<ApiResponse> DeleteReplacements(DeleteMutilDto deleteDto);
        Task<ApiResponse> UpdateStatus(Guid id, BaseUpdateStatusDto requestStatus);
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

            if (newAsset.Status != AssetStatus.Operational)
            {
                throw new ApiException("Trang thiết bị dùng để thay thế đang trong một yêu cầu khác", StatusCode.BAD_REQUEST);
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
            replacement.RequestCode = GenerateRequestCode();

            var mediaFiles = new List<MediaFile>();
            if (createDto.RelatedFiles != null)
            {
                foreach (var file in createDto.RelatedFiles)
                {
                    var newMediaFile = new MediaFile
                    {
                        FileName = file.FileName ?? "",
                        Uri = file.Uri ?? ""
                    };
                    mediaFiles.Add(newMediaFile);
                }
            }

            if (!await _repository.InsertReplacementV2(replacement, mediaFiles, AccountId, CurrentDate))
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

            if (!await _repository.DeleteReplacement(existingReplace, AccountId, CurrentDate))
            {
                throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
            }
            return ApiResponse.Success();
        }

        public async Task<ApiResponse> DeleteReplacements(DeleteMutilDto deleteDto)
        {
            var replaceDeleteds = await MainUnitOfWork.ReplacementRepository.FindAsync(
                                    new Expression<Func<Replacement, bool>>[]
                                    {
                                        x => !x.DeletedAt.HasValue,
                                        x => deleteDto.ListId!.Contains(x.Id)
                                    }, null);

            var replacements = replaceDeleteds.Where(r => r != null).ToList();

            if (!await _repository.DeleteReplacements(replaceDeleteds, AccountId, CurrentDate))
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
            var mediaFileQuery = MainUnitOfWork.MediaFileRepository.GetQuery().Where(m => m!.ItemId == replacement.Id);

            replacement.MediaFile = new MediaFileDto
            {
                FileType = mediaFileQuery.Select(m => m!.FileType).FirstOrDefault(),
                Uri = mediaFileQuery.Select(m => m!.Uri).ToList(),
                Content = mediaFileQuery.Select(m => m!.Content).FirstOrDefault()
            };

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


        public async Task<ApiResponse> Update(Guid id, BaseRequestUpdateDto updateDto)
        {
            var existingReplace = await MainUnitOfWork.ReplacementRepository.FindOneAsync(id);
            if (existingReplace == null)
            {
                throw new ApiException("Không tìm thấy yêu cầu thay thế này", StatusCode.NOT_FOUND);
            }

            if (existingReplace.Status != RequestStatus.InProgress)
            {
                throw new ApiException("Chỉ được cập nhật các yêu cầu chưa hoàn thành", StatusCode.NOT_FOUND);
            }

            existingReplace.Description = updateDto.Description ?? existingReplace.Description;
            existingReplace.Notes = updateDto.Notes ?? existingReplace.Notes;
            existingReplace.Priority = updateDto.Priority ?? existingReplace.Priority;
            if (!await _repository.UpdateStatus(existingReplace, existingReplace.Status, AccountId, CurrentDate))
            {
                throw new ApiException("Cập nhật thông tin yêu cầu thất bại", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success("Cập nhật yêu cầu thành công");
        }

        public async Task<ApiResponse> UpdateStatus(Guid id, BaseUpdateStatusDto requestStatus)
        {
            var existingReplace = MainUnitOfWork.ReplacementRepository.GetQuery()
                                    .Include(r => r!.Asset)
                                    .Where(r => r!.Id == id)
                                    .FirstOrDefault();

            if (existingReplace == null)
            {
                throw new ApiException("Không tìm thấy yêu cầu thay thế này", StatusCode.NOT_FOUND);
            }

            existingReplace.Status = requestStatus.Status ?? existingReplace.Status;

            if (!await _repository.UpdateStatus(existingReplace, requestStatus.Status, AccountId, CurrentDate))
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