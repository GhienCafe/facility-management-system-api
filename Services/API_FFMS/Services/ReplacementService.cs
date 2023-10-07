using API_FFMS.Dtos;
using API_FFMS.Repositories;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;

namespace API_FFMS.Services
{
    public interface IReplacementService : IBaseService
    {
        Task<ApiResponse> Create(ReplaceCreateDto createDto);
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

            var replacement = createDto.ProjectTo<ReplaceCreateDto, Replacement>();

            if (!await _repository.InsertReplacement(replacement, AccountId, CurrentDate))
            {
                throw new ApiException("Tạo yêu cầu thất bại", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Created("Gửi yêu cầu thành công");
        }
    }
}
