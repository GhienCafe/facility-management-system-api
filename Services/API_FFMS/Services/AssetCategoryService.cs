using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using System.Linq.Expressions;

namespace API_FFMS.Services
{
    public interface IAssetCategoryService : IBaseService
    {
        Task<ApiResponses<AssetCategoryDto>> GetAssetCategories(AssetCategoryQueryDto queryDto);
        Task<ApiResponse<AssetCategoryDetailDto>> GetAssetCategory(Guid id);
        Task<ApiResponse> Create(AssetCategoryCreateDto createDto);
        public Task<ApiResponse<AssetCategoryDetailDto>> Update(Guid id, AssetCategoryUpdateDto updateDto);
        Task<ApiResponse> Delete(Guid id);
    }

    public class AssetCategoryService : BaseService, IAssetCategoryService
    {
        public AssetCategoryService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
        }

        public async Task<ApiResponse> Create(AssetCategoryCreateDto createDto)
        {
            if (!createDto.CategoryName.IsBetweenLength(1, 255))
            {
                throw new ApiException("Cannot create asset category when the name is null or must have a length between 1 and 255 characters", StatusCode.ALREADY_EXISTS);
            }

            var existingCategory = MainUnitOfWork.AssetCategoryRepository.GetQuery()
                                   .Where(x => x.CategoryName.Trim().ToLower() == createDto.CategoryName.Trim().ToLower())
                                   .SingleOrDefault();

            if (existingCategory != null)
            {
                throw new ApiException("Asset category name is already in use, please choose a different name.", StatusCode.BAD_REQUEST);
            }

            var assetCategory = createDto.ProjectTo<AssetCategoryCreateDto, AssetCategory>();

            // You can add additional validation logic here if needed before inserting the category.

            bool response = await MainUnitOfWork.AssetCategoryRepository.InsertAsync(assetCategory, AccountId);

            if (response)
            {
                return ApiResponse.Success();
            }
            else
            {
                return ApiResponse.Failed();
            }
        }

        public async Task<ApiResponse> Delete(Guid id)
        {
            var existingAssetCategory = await MainUnitOfWork.AssetCategoryRepository.FindOneAsync(id);

            if (existingAssetCategory == null)
            {
                throw new ApiException("Asset category not found", StatusCode.NOT_FOUND);
            }

            bool result = await MainUnitOfWork.AssetCategoryRepository.DeleteAsync(existingAssetCategory, AccountId, CurrentDate);

            if (result)
            {
                return ApiResponse.Success();
            }
            else
            {
                return ApiResponse.Failed();
            }
        }

        public async Task<ApiResponses<AssetCategoryDto>> GetAssetCategories(AssetCategoryQueryDto queryDto)
        {
            Expression<Func<AssetCategory, bool>>[] conditions = new Expression<Func<AssetCategory, bool>>[]
        {
            x => !x.DeletedAt.HasValue
        };

            if (string.IsNullOrEmpty(queryDto.CategoryCode) == false)
            {
                conditions = conditions.Append(x => x.CategoryCode.Trim().ToLower() == queryDto.CategoryCode.Trim().ToLower()).ToArray();
            }

            var response = await MainUnitOfWork.AssetCategoryRepository.FindResultAsync<AssetCategoryDto>(
                conditions,
                queryDto.OrderBy,
                queryDto.Skip(),
                queryDto.PageSize
            );

            return ApiResponses<AssetCategoryDto>.Success(
                response.Items,
                response.TotalCount,
                queryDto.PageSize,
                queryDto.Skip(),
                (int)Math.Ceiling(response.TotalCount / (double)queryDto.PageSize)
            );
        }

        public async Task<ApiResponse<AssetCategoryDetailDto>> GetAssetCategory(Guid id)
        {
            var assetCategory = await MainUnitOfWork.AssetCategoryRepository.FindOneAsync<AssetCategoryDetailDto>(
            new Expression<Func<AssetCategory, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.Id == id
            });

            if (assetCategory == null)
            {
                throw new ApiException("Asset category not found", StatusCode.NOT_FOUND);
            }

            assetCategory = await _mapperRepository.MapCreator(assetCategory);

            return ApiResponse<AssetCategoryDetailDto>.Success(assetCategory);
        }

        public async Task<ApiResponse<AssetCategoryDetailDto>> Update(Guid id, AssetCategoryUpdateDto updateDto)
        {
            var existingAssetCategory = await MainUnitOfWork.AssetCategoryRepository.FindOneAsync(id);

            if (existingAssetCategory == null)
            {
                throw new ApiException("Asset category not found", StatusCode.NOT_FOUND);
            }

            var assetCategoryUpdate = existingAssetCategory;

            existingAssetCategory.CategoryName = updateDto.CategoryName ?? existingAssetCategory.CategoryName;
            existingAssetCategory.Description = updateDto.Description ?? existingAssetCategory.Description;
            existingAssetCategory.Unit = updateDto.Unit ?? existingAssetCategory.Unit;

            //var result = updateDto.ProjectTo<AssetCategoryUpdateDto, AssetCategory>();

            if (!await MainUnitOfWork.AssetCategoryRepository.UpdateAsync(assetCategoryUpdate, AccountId, CurrentDate))
            {
                throw new ApiException("Can't not update", StatusCode.SERVER_ERROR);
            }

            return await GetAssetCategory(id);

        }
    }
}

