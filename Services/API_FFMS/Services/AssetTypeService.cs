using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Services
{
    public interface IAssetTypeService : IBaseService
    {
        Task<ApiResponses<AssetTypeDto>> GetAssetTypes(AssetTypeQueryDto queryDto);
        Task<ApiResponses<AssetTypeSheetDto>> GetAssetTypes();
        Task<ApiResponse<AssetTypeDetailDto>> GetAssetType(Guid id);
        Task<ApiResponse> Create(AssetTypeCreateDto createDto);
        public Task<ApiResponse> Update(Guid id, AssetTypeUpdateDto updateDto);
        Task<ApiResponse> Delete(Guid id);
        Task<ApiResponse> DeleteAssetTypes(List<Guid> ids);
    }

    public class AssetTypeService : BaseService, IAssetTypeService
    {
        public AssetTypeService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
        }

        public async Task<ApiResponse> Create(AssetTypeCreateDto createDto)
        {

            var existingType = MainUnitOfWork.AssetTypeRepository.GetQuery()
                                   .Where(x => !x!.DeletedAt.HasValue && x!.TypeCode.Trim().ToLower() == createDto.TypeCode.Trim().ToLower())
                                   .SingleOrDefault();

            if (existingType != null)
                throw new ApiException("Đã tồn tại mã loại trang thiết bị", StatusCode.ALREADY_EXISTS);

            var assetCategory = createDto.ProjectTo<AssetTypeCreateDto, AssetType>();

            if (!await MainUnitOfWork.AssetTypeRepository.InsertAsync(assetCategory, AccountId, CurrentDate))
                throw new ApiException("Thêm thất bại", StatusCode.SERVER_ERROR);

            return ApiResponse.Created("Thêm thành công");
        }

        public async Task<ApiResponse> Delete(Guid id)
        {
            var existingType = await MainUnitOfWork.AssetTypeRepository.FindOneAsync(id);

            if (existingType == null)
            {
                throw new ApiException("Không tìm thấy loại trang thiết bị", StatusCode.NOT_FOUND);
            }

            if (await MainUnitOfWork.AssetTypeRepository.DeleteAsync(existingType, AccountId, CurrentDate))
                throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);

            return ApiResponse.Success("Xóa thành công");
        }

        public async Task<ApiResponses<AssetTypeDto>> GetAssetTypes(AssetTypeQueryDto queryDto)
        {
            var keyword = queryDto.Keyword?.Trim().ToLower();
            var response = MainUnitOfWork.AssetTypeRepository.GetQuery()
                .Where(x => !x!.DeletedAt.HasValue);

            if (!string.IsNullOrEmpty(keyword))
            {
                response = response.Where(x => x!.TypeCode.ToLower().Contains(keyword)
                    || x.TypeName.ToLower().Contains(keyword)
                    || x.Description!.ToLower().Contains(keyword));
            }

            if (queryDto.Unit != null)
            {
                response = response.Where(x => x!.Unit == queryDto.Unit);
            }
            
            if (queryDto.CategoryId != null)
            {
                response = response.Where(x => x!.CategoryId == queryDto.CategoryId);
            }

            var totalCount = await response.CountAsync();
            var assetTypes = await response.Select(x => new AssetTypeDto
            {
                Id = x!.Id,
                Description = x.Description,
                TypeCode = x.TypeCode,
                TypeName = x.TypeName,
                Unit = x.Unit,
                UnitObj = x.Unit.GetValue(),
                ImageUrl = x.ImageUrl,
                CategoryId = x.CategoryId,
                CreatedAt = x.CreatedAt,
                EditedAt = x.EditedAt,
                EditorId = x.EditorId ?? Guid.Empty,
                CreatorId = x.CreatorId ?? Guid.Empty
            }).ToListAsync();

            assetTypes = await _mapperRepository.MapCreator(assetTypes);

            return ApiResponses<AssetTypeDto>.Success(
                assetTypes,
                totalCount,
                queryDto.PageSize,
                queryDto.Page,
                (int)Math.Ceiling(totalCount / (double)queryDto.PageSize)
            );
        }

        public async Task<ApiResponse<AssetTypeDetailDto>> GetAssetType(Guid id)
        {
            var assetType = MainUnitOfWork.AssetTypeRepository.GetQuery()
                .Where(x => !x!.DeletedAt.HasValue && x.Id == id)
                .Select(x => new AssetTypeDetailDto
                {
                    Id = x!.Id,
                    Description = x.Description,
                    TypeCode = x.TypeCode,
                    TypeName = x.TypeName,
                    Unit = x.Unit,
                    UnitObj = x.Unit.GetValue(),
                    ImageUrl = x.ImageUrl,
                    CategoryId = x.CategoryId,
                    CreatedAt = x.CreatedAt,
                    EditedAt = x.EditedAt,
                    EditorId = x.EditorId ?? Guid.Empty,
                    CreatorId = x.CreatorId ?? Guid.Empty
                }).FirstOrDefault();

            if (assetType == null)
            {
                throw new ApiException("Không tìm thấy loại trang thiết bị", StatusCode.NOT_FOUND);
            }

            assetType = await _mapperRepository.MapCreator(assetType);

            return ApiResponse<AssetTypeDetailDto>.Success(assetType);
        }

        public async Task<ApiResponse> Update(Guid id, AssetTypeUpdateDto updateDto)
        {
            var existingTpye = await MainUnitOfWork.AssetTypeRepository.FindOneAsync(id);

            if (existingTpye == null)
            {
                throw new ApiException("Không tìm thấy loại trang thiết bị", StatusCode.NOT_FOUND);
            }

            existingTpye.TypeName = updateDto.TypeName ?? existingTpye.TypeName;
            existingTpye.Description = updateDto.Description ?? existingTpye.Description;
            existingTpye.Unit = updateDto.Unit ?? existingTpye.Unit;
            existingTpye.CategoryId = updateDto.CategoryId ?? existingTpye.CategoryId;
            existingTpye.TypeCode = updateDto.TypeCode ?? existingTpye.TypeCode;
            existingTpye.ImageUrl = updateDto.ImageUrl ?? existingTpye.ImageUrl;

            if (!await MainUnitOfWork.AssetTypeRepository.UpdateAsync(existingTpye, AccountId, CurrentDate))
            {
                throw new ApiException("Cập nhật thất bại", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success("Cập nhật thành công");
        }

        public async Task<ApiResponse> DeleteAssetTypes(List<Guid> ids)
        {
            var assetTypeDeleteds = new List<AssetType>();
            foreach (var id in ids)
            {
                var existingType = await MainUnitOfWork.AssetTypeRepository.FindOneAsync(id);

                if (existingType == null)
                {
                    throw new ApiException("Không tìm thấy loại trang thiết bị", StatusCode.NOT_FOUND);
                }
                assetTypeDeleteds.Add(existingType);
            }
            if (await MainUnitOfWork.AssetTypeRepository.DeleteAsync(assetTypeDeleteds, AccountId, CurrentDate))
            {
                throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success();
        }

        public async Task<ApiResponses<AssetTypeSheetDto>> GetAssetTypes()
        {
            var response = MainUnitOfWork.AssetTypeRepository.GetQuery()
                .Where(x => !x!.DeletedAt.HasValue);
            var assetTypes = response.Select(x => new AssetTypeSheetDto
            {
                Id = x!.Id,
                Description = x.Description,
                TypeCode = x.TypeCode,
                TypeName = x.TypeName,
                Unit = x.Unit,
                UnitObj = x.Unit.GetValue(),
                ImageUrl = x.ImageUrl,
                CategoryId = x.CategoryId,
                CreatedAt = x.CreatedAt,
                EditedAt = x.EditedAt,
                EditorId = x.EditorId ?? Guid.Empty,
                CreatorId = x.CreatorId ?? Guid.Empty
            }).ToList();

            assetTypes = await _mapperRepository.MapCreator(assetTypes);

            return ApiResponses<AssetTypeSheetDto>.Success(assetTypes);
        }
    }
}

