using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
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
                throw new ApiException("Không tìm thấy nhãn hiệu", StatusCode.NOT_FOUND);
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
                throw new ApiException("Không tìm thấy nhãn hiệu", StatusCode.NOT_FOUND);
            }

            model = await _mapperRepository.MapCreator(model);

            return ApiResponse<ModelDto>.Success(model);
        }

        public async Task<ApiResponses<ModelDto>> GetModels(ModelQueryDto queryDto)
        {
            var response = await MainUnitOfWork.ModelRepository.FindResultAsync<ModelDto>(
                new Expression<Func<Model, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => string.IsNullOrEmpty(queryDto.ModelName) ||
                         x.ModelName.ToLower().Contains(queryDto.ModelName.Trim().ToLower())
                }, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);

            response.Items = await _mapperRepository.MapCreator(response.Items.ToList());

            return ApiResponses<ModelDto>.Success(
                response.Items,
                response.TotalCount,
                queryDto.PageSize,
                queryDto.Page,
                (int)Math.Ceiling(response.TotalCount / (double)queryDto.PageSize)
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
                Description = x.Description,
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
                throw new ApiException("Không tìm thấy nhãn hiệu", StatusCode.NOT_FOUND);
            }

            existingModel.ModelName = updateDto.ModelName ?? existingModel.ModelName;
            existingModel.Description = updateDto.Description ?? existingModel.Description;

            if (!await MainUnitOfWork.ModelRepository.UpdateAsync(existingModel, AccountId, CurrentDate))
            {
                throw new ApiException("Không thể cập nhật", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success();
        }
    }
}