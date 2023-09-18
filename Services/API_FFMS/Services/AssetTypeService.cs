using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using System.Linq.Expressions;

namespace API_FFMS.Services
{
    public interface IAssetTypeService : IBaseService
    {
        Task<ApiResponses<AssetTypeDto>> GetAssetTypes(AssetTypeQueryDto queryDto);
        Task<ApiResponse<AssetTypeDetailDto>> GetAssetType(Guid id);
        Task<ApiResponse> Create(AssetTypeCreateDto createDto);
        public Task<ApiResponse<AssetTypeDetailDto>> Update(Guid id, AssetTypeUpdateDto updateDto);
        Task<ApiResponse> Delete(Guid id);
    }

    public class AssetTypeService : BaseService, IAssetTypeService
    {
        public AssetTypeService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
        }

        public async Task<ApiResponse> Create(AssetTypeCreateDto createDto)
        {

            var existingCategory = MainUnitOfWork.AssetTypeRepository.GetQuery()
                                   .Where(x => x.TypeName.Trim().ToLower() == createDto.TypeName.Trim().ToLower())
                                   .SingleOrDefault();

            if (existingCategory != null)
            {
                throw new ApiException("Asset category name is already in use, please choose a different name.", StatusCode.BAD_REQUEST);
            }

            var assetCategory = createDto.ProjectTo<AssetTypeCreateDto, AssetType>();

            // You can add additional validation logic here if needed before inserting the category.

            bool response = await MainUnitOfWork.AssetTypeRepository.InsertAsync(assetCategory, AccountId);

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
                var existingAssetCategory = await MainUnitOfWork.AssetTypeRepository.FindOneAsync(id);

                if (existingAssetCategory == null)
                {
                    throw new ApiException("Asset category not found", StatusCode.NOT_FOUND);
                }

                bool result = await MainUnitOfWork.AssetTypeRepository.DeleteAsync(existingAssetCategory, AccountId, CurrentDate);

                if (result)
                {
                    return ApiResponse.Success();
                }
                else
                {
                    return ApiResponse.Failed();
                }
        }

        public async Task<ApiResponses<AssetTypeDto>> GetAssetTypes(AssetTypeQueryDto queryDto)
        {
                Expression<Func<AssetType, bool>>[] conditions = new Expression<Func<AssetType, bool>>[]
            {
             x => !x.DeletedAt.HasValue
            };

                if (string.IsNullOrEmpty(queryDto.TypeCode) == false)
                {
                    conditions = conditions.Append(x => x.TypeCode.Trim().ToLower().Contains(queryDto.TypeCode.Trim().ToLower())).ToArray();
                }

                var response = await MainUnitOfWork.AssetTypeRepository.FindResultAsync<AssetTypeDto>(
                    conditions,
                    queryDto.OrderBy,
                    queryDto.Skip(),
                    queryDto.PageSize
                );

                return ApiResponses<AssetTypeDto>.Success(
                    response.Items,
                    response.TotalCount,
                    queryDto.PageSize,
                    queryDto.Skip(),
                    (int)Math.Ceiling(response.TotalCount / (double)queryDto.PageSize)
                );
        }

        public async Task<ApiResponse<AssetTypeDetailDto>> GetAssetType(Guid id)
        {
            var assetCategory = await MainUnitOfWork.AssetTypeRepository.FindOneAsync<AssetTypeDetailDto>(
            new Expression<Func<AssetType, bool>>[]
            {
                 x => !x.DeletedAt.HasValue,
                 x => x.Id == id
            });

            if (assetCategory == null)
            {
                throw new ApiException("Asset category not found", StatusCode.NOT_FOUND);
            }

            assetCategory = await _mapperRepository.MapCreator(assetCategory);

            return ApiResponse<AssetTypeDetailDto>.Success(assetCategory);
        }

        public async Task<ApiResponse<AssetTypeDetailDto>> Update(Guid id, AssetTypeUpdateDto updateDto)
        {
            var existingAssetCategory = await MainUnitOfWork.AssetTypeRepository.FindOneAsync(id);

            if (existingAssetCategory == null)
            {
                throw new ApiException("Asset category not found", StatusCode.NOT_FOUND);
            }

            var assetCategoryUpdate = existingAssetCategory;

            existingAssetCategory.TypeName = updateDto.TypeName ?? existingAssetCategory.TypeName;
            existingAssetCategory.Description = updateDto.Description ?? existingAssetCategory.Description;
            existingAssetCategory.Unit = updateDto.Unit ?? existingAssetCategory.Unit;

            //var result = updateDto.ProjectTo<AssetCategoryUpdateDto, AssetCategory>();

            if (!await MainUnitOfWork.AssetTypeRepository.UpdateAsync(assetCategoryUpdate, AccountId, CurrentDate))
            {
                throw new ApiException("Can't not update", StatusCode.SERVER_ERROR);
            }

            return await GetAssetType(id);

        }
    }
}

