﻿using API_FFMS.Dtos;
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
        Task<ApiResponse> DeleteReplacements(List<Guid> ids);
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

            if(newAsset.Status != AssetStatus.Operational)
            {
                throw new ApiException("Trang thiết bị cần thay thế đang trong một yêu cầu khác", StatusCode.BAD_REQUEST);
            }

            var replacement = createDto.ProjectTo<ReplaceCreateDto, Replacement>();

            if (!await _repository.InsertReplacement(replacement, AccountId, CurrentDate))
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

            existingReplace.Status = RequestStatus.Cancelled;

            if (!await MainUnitOfWork.ReplacementRepository.DeleteAsync(existingReplace, AccountId, CurrentDate))
            {
                throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
            }
            return ApiResponse.Success();
        }

        public async Task<ApiResponse> DeleteReplacements(List<Guid> ids)
        {
            var replaceDeleteds = await MainUnitOfWork.ReplacementRepository.FindAsync(
            new Expression<Func<Replacement, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => ids.Contains(x.Id)
            }, null);

            if (!await MainUnitOfWork.ReplacementRepository.DeleteAsync(replaceDeleteds, AccountId, CurrentDate))
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

            replacement.Asset = await MainUnitOfWork.AssetRepository.FindOneAsync<AssetBaseDto>(
                new Expression<Func<Asset, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == replacement.AssetId
                });

            replacement.NewAsset = await MainUnitOfWork.AssetRepository.FindOneAsync<AssetBaseDto>(
                new Expression<Func<Asset, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == replacement.NewAssetId
                });

            replacement = await _mapperRepository.MapCreator(replacement);

            return ApiResponse<ReplaceDto>.Success(replacement);
        }

        public async Task<ApiResponses<ReplaceDto>> GetReplaces(ReplacementQueryDto queryDto)
        {
            var keyword = queryDto.Keyword?.Trim().ToLower();
            var replaceQuery = MainUnitOfWork.ReplacementRepository.GetQuery()
                               .Where(x => !x!.DeletedAt.HasValue);

            if (keyword != null)
            {
                replaceQuery = replaceQuery.Where(x => x!.RequestCode.ToLower().Contains(keyword));
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

            if (queryDto.RequestDate != null)
            {
                replaceQuery = replaceQuery.Where(x => x!.RequestDate == queryDto.RequestDate);
            }

            if (queryDto.CompletionDate != null)
            {
                replaceQuery = replaceQuery.Where(x => x!.CompletionDate == queryDto.CompletionDate);
            }
            var joinTables = from replace in replaceQuery
                             join asset in MainUnitOfWork.AssetRepository.GetQuery() on replace.AssetId equals asset.Id
                             join newAsset in MainUnitOfWork.AssetRepository.GetQuery() on replace.NewAssetId equals newAsset.Id
                             select new
                             {
                                 Replacement = replace,
                                 Asset = asset,
                                 NewAsset = newAsset
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
                Description = x.Replacement.Description,
                Notes = x.Replacement.Notes,
                IsInternal = x.Replacement.IsInternal,
                AssignedTo = x.Replacement.AssignedTo,
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
                }
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
            
            if (updateDto.Status == RequestStatus.InProgress)
            {
                if (!await _repository.UpdateStatus(existingReplace, existingReplace.Status, AccountId, CurrentDate))
                {
                    throw new ApiException("Cập nhật thông tin yêu cầu thất bại", StatusCode.SERVER_ERROR);
                }
            }
            existingReplace.RequestDate = updateDto.RequestDate ?? existingReplace.RequestDate;
            existingReplace.CompletionDate = updateDto.CompletionDate ?? existingReplace.CompletionDate;
            existingReplace.Status = updateDto.Status ?? existingReplace.Status;
            existingReplace.Description = updateDto.Description ?? existingReplace.Description;
            existingReplace.Notes = updateDto.Notes ?? existingReplace.Notes;
            if (!await _repository.UpdateStatus(existingReplace, existingReplace.Status, AccountId, CurrentDate))
            {
                throw new ApiException("Cập nhật thông tin yêu cầu thất bại", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success("Cập nhật yêu cầu thành công");
        }

    }
}
