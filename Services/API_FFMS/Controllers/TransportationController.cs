using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers
{
    public class TransportationController : BaseController
    {
        private readonly ITransportationService _service;

        public TransportationController(ITransportationService transportationService)
        {
            _service = transportationService;
        }

        [HttpGet]
        [SwaggerOperation("Get list transportations")]
        public async Task<ApiResponses<TransportDto>> GetTransports([FromQuery] TransportationQueryDto queryDto)
        {
            return await _service.GetTransports(queryDto);
        }

        [HttpPost]
        [SwaggerOperation("Create new transportation")]
        public async Task<ApiResponse> Create([FromBody] TransportCreateDto createDto)
        {
            return await _service.Create(createDto);
        }

        [HttpDelete]
        [SwaggerOperation("Delete a transportation")]
        public async Task<ApiResponse> Delete(Guid id)
        {
            return await _service.Delete(id);
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation("Get a transportation")]
        public async Task<ApiResponse<TransportDto>> GetTransportation(Guid id)
        {
            return await _service.GetTransportation(id);
        }
    }
}
