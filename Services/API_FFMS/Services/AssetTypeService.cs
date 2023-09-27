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
        public Task<ApiResponse> Update(Guid id, AssetTypeUpdateDto updateDto);
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
                                   .Where(x => !x!.DeletedAt.HasValue && x!.TypeCode.Trim().ToLower() == createDto.TypeCode.Trim().ToLower())
                                   .SingleOrDefault();

            if (existingCategory != null)
                throw new ApiException("Asset category name is already exists", StatusCode.ALREADY_EXISTS);

            var assetCategory = createDto.ProjectTo<AssetTypeCreateDto, AssetType>();

            if (!await MainUnitOfWork.AssetTypeRepository.InsertAsync(assetCategory, AccountId, CurrentDate))
                throw new ApiException("Insert fail!", StatusCode.SERVER_ERROR);
            
            return ApiResponse.Created("Create successfully!");
        }

        public async Task<ApiResponse> Delete(Guid id)
        {
                var existingAssetCategory = await MainUnitOfWork.AssetTypeRepository.FindOneAsync(id);

                if (existingAssetCategory == null)
                {
                    throw new ApiException("Asset category not found", StatusCode.NOT_FOUND);
                }

                if (await MainUnitOfWork.AssetTypeRepository.DeleteAsync(existingAssetCategory, AccountId, CurrentDate))
                    throw new ApiException("Delete fail", StatusCode.SERVER_ERROR);
                
                return ApiResponse.Success();
        }

        public async Task<ApiResponses<AssetTypeDto>> GetAssetTypes(AssetTypeQueryDto queryDto)
        {
            var response = await MainUnitOfWork.AssetTypeRepository.FindResultAsync<AssetTypeDto>(
                new Expression<Func<AssetType, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => string.IsNullOrEmpty(queryDto.TypeName) ||
                         x.TypeName.ToLower().Contains(queryDto.TypeName.Trim().ToLower()),
                    x => string.IsNullOrEmpty(queryDto.TypeCode) ||
                         x.TypeCode.ToLower().Equals(queryDto.TypeCode.Trim().ToLower())
                }, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);

            response.Items = await _mapperRepository.MapCreator(response.Items.ToList());

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
            var assetType = await MainUnitOfWork.AssetTypeRepository.FindOneAsync<AssetTypeDetailDto>(
            new Expression<Func<AssetType, bool>>[]
            {
                 x => !x.DeletedAt.HasValue,
                 x => x.Id == id
            });

            if (assetType == null)
            {
                throw new ApiException("Asset type not found", StatusCode.NOT_FOUND);
            }

            assetType = await _mapperRepository.MapCreator(assetType);

            return ApiResponse<AssetTypeDetailDto>.Success(assetType);
        }

        public async Task<ApiResponse> Update(Guid id, AssetTypeUpdateDto updateDto)
        {
            var existingAssetCategory = await MainUnitOfWork.AssetTypeRepository.FindOneAsync(id);

            if (existingAssetCategory == null)
            {
                throw new ApiException("Asset category not found", StatusCode.NOT_FOUND);
            }

            existingAssetCategory.TypeName = updateDto.TypeName ?? existingAssetCategory.TypeName;
            existingAssetCategory.Description = updateDto.Description ?? existingAssetCategory.Description;
            existingAssetCategory.Unit = updateDto.Unit ?? existingAssetCategory.Unit;

            if (!await MainUnitOfWork.AssetTypeRepository.UpdateAsync(existingAssetCategory, AccountId, CurrentDate))
            {
                throw new ApiException("Can't not update", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success();

        }
    }
}

