using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers
{
    public class ModelController : BaseController
    {
        private readonly IModelService _service;

        public ModelController(IModelService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation("Get list models")]
        public async Task<ApiResponses<ModelDto>> GetModels([FromQuery] ModelQueryDto queryDto)
        {
            return await _service.GetModels(queryDto);
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation("Get details an model")]
        public async Task<ApiResponse<ModelDto>> GetModel(Guid id)
        {
            return await _service.GetModel(id);
        }

        [HttpPost]
        [SwaggerOperation("Create new model")]
        public async Task<ApiResponse> Create([FromBody] ModelCreateDto createDto)
        {
            return await _service.Create(createDto);
        }

        [HttpPut("{id:guid}")]
        [SwaggerOperation("Update a model")]
        public async Task<ApiResponse> Update(Guid id, ModelUpdateDto updateDto)
        {
            return await _service.Update(id, updateDto);
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation("Delete model")]
        public async Task<ApiResponse> Delete(Guid id)
        {
            return await _service.Delete(id);
        }
    }
}
