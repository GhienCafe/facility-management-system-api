using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API_FFMS.Services
{
    public interface IModelService : IBaseService
    {
        Task<ApiResponse> Create(ModelCreateDto createDto);
        Task<ApiResponse<ModelDto>> GetModel(Guid id);
        public Task<ApiResponse> Update(Guid id, ModelUpdateDto updateDto);
        Task<ApiResponse> Delete(Guid id);
        Task<ApiResponse> DeleteModels(DeleteMutilDto deleteDto);
        Task<ApiResponses<ModelDto>> GetModels(ModelQueryDto queryDto);
        Task<ApiResponses<ModelDto>> GetModels();
    }
    public class ModelService : BaseService, IModelService
    {
        public ModelService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
        }

        public async Task<ApiResponse> Create(ModelCreateDto createDto)
        {
            var model = createDto.ProjectTo<ModelCreateDto, Model>();
            if (!await MainUnitOfWork.ModelRepository.InsertAsync(model, AccountId, CurrentDate))
            {
                throw new ApiException("Thêm mới thất bại", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Created("Thêm mới thành công");
        }

        public async Task<ApiResponse> Delete(Guid id)
        {
            var existingModel = await MainUnitOfWork.ModelRepository.FindOneAsync(id);
            if (existingModel == null)
            {
                throw new ApiException("Không tìm thấy dòng sản phẩm", StatusCode.NOT_FOUND);
            }

            if (!await MainUnitOfWork.ModelRepository.DeleteAsync(existingModel, AccountId, CurrentDate))
            {
                throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success();
        }

        public async Task<ApiResponse> DeleteModels(DeleteMutilDto deleteDto)
        {
            var modelDeleteds = await MainUnitOfWork.ModelRepository.FindAsync(
            new Expression<Func<Model, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => deleteDto.ListId!.Contains(x.Id)
            }, null);

            if (!await MainUnitOfWork.ModelRepository.DeleteAsync(modelDeleteds, AccountId, CurrentDate))
            {
                throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success();
        }

        public async Task<ApiResponse<ModelDto>> GetModel(Guid id)
        {
            var model = await MainUnitOfWork.ModelRepository.FindOneAsync<ModelDto>(
            new Expression<Func<Model, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.Id == id
            });

            if (model == null)
            {
                throw new ApiException("Không tìm thấy dòng sản phẩm", StatusCode.NOT_FOUND);
            }

            model.Brand = await MainUnitOfWork.BrandRepository.FindOneAsync<BrandDto>(
                new Expression<Func<Brand, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == model.BrandId
                });

            model = await _mapperRepository.MapCreator(model);

            return ApiResponse<ModelDto>.Success(model);
        }

        public async Task<ApiResponses<ModelDto>> GetModels(ModelQueryDto queryDto)
        {
            var keyword = queryDto.Keyword?.Trim().ToLower();
            var modelDataSet = MainUnitOfWork.ModelRepository.GetQuery().Include(x => x!.Brand)
                               .Where(x => !x!.DeletedAt.HasValue);
            if (!string.IsNullOrEmpty(keyword))
            {
                modelDataSet = modelDataSet.Where(x => x.ModelName!.ToLower().Contains(keyword));
            }

            var totalCount = modelDataSet.Count();

            modelDataSet = modelDataSet.Skip(queryDto.Skip()).Take(queryDto.PageSize);

            var models = await modelDataSet.Select(x => new ModelDto
            {
                Id = x!.Id,
                ModelName = x.ModelName,
                ModelCode = x.ModelCode,
                Description = x.Description,
                ImageUrl = x.ImageUrl,
                Brand = x.Brand != null ? new BrandDto
                {
                    Id = x.Brand!.Id,
                    BrandName = x.Brand.BrandName,
                    Description = x.Brand.Description
                } : null
            }).ToListAsync();

            models = await _mapperRepository.MapCreator(models);

            return ApiResponses<ModelDto>.Success(
                models,
                totalCount,
                queryDto.PageSize,
                queryDto.Page,
                (int)Math.Ceiling(totalCount / (double)queryDto.PageSize)
            );
        }

        public async Task<ApiResponses<ModelDto>> GetModels()
        {
            var response = MainUnitOfWork.ModelRepository.GetQuery()
                .Where(x => !x!.DeletedAt.HasValue);
            var models = response.Select(x => new ModelDto
            {
                Id = x!.Id,
                ModelName = x.ModelName,
                ModelCode = x.ModelCode,
                Description = x.Description,
                ImageUrl = x.ImageUrl,
                CreatedAt = x.CreatedAt,
                EditedAt = x.EditedAt,
                EditorId = x.EditorId ?? Guid.Empty,
                CreatorId = x.CreatorId ?? Guid.Empty
            }).ToList();

            models = await _mapperRepository.MapCreator(models);

            return ApiResponses<ModelDto>.Success(models);
        }

        public async Task<ApiResponse> Update(Guid id, ModelUpdateDto updateDto)
        {
            var existingModel = await MainUnitOfWork.ModelRepository.FindOneAsync(id);
            if (existingModel == null)
            {
                throw new ApiException("Không tìm thấy Dòng sản phẩm", StatusCode.NOT_FOUND);
            }

            existingModel.ModelName = updateDto.ModelName ?? existingModel.ModelName;
            existingModel.ModelCode = updateDto.ModelCode ?? existingModel.ModelCode;
            existingModel.Description = updateDto.Description ?? existingModel.Description;
            existingModel.MaintenancePeriodTime =
                updateDto.MaintenancePeriodTime ?? existingModel.MaintenancePeriodTime;
            existingModel.ImageUrl = updateDto.ImageUrl ?? existingModel.ImageUrl;
            existingModel.BrandId = updateDto.BrandId ?? existingModel.BrandId;

            if (!await MainUnitOfWork.ModelRepository.UpdateAsync(existingModel, AccountId, CurrentDate))
            {
                throw new ApiException("Không thể cập nhật", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success();
        }
    }
}