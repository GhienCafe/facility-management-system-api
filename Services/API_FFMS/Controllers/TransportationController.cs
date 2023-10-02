using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers
{
    public class TransportationController : BaseController
    {
        private readonly ITransportationService _transportationService;
        public TransportationController(ITransportationService transportationService)
        {
            _transportationService = transportationService;
        }
        //
        // [HttpGet]
        // [SwaggerOperation("Get all transportations")]
        // public async Task<ApiResponses<TransportDto>> GetTransports([FromQuery] TransportQueryDto queryDto)
        // {
        //     return await _transportationService.GetTransports(queryDto);
        // }

        [HttpGet("{id:guid}")]
        [SwaggerOperation("Get detail transportation")]
        public async Task<ApiResponse<TransportRequestDto>> GetTransport(Guid id)
        {
            return await _transportationService.GetTransport(id);
        }

        [HttpPost]
        [SwaggerOperation("Create new transportation")]
        public async Task<ApiResponse> Create([FromBody] TransportCreateDto createDto)
        {
            return await _transportationService.Create(createDto);
        }

        [HttpPut]
        [SwaggerOperation("Update transportation")]
        public async Task<ApiResponse> Update(Guid id, TransportUpdateDto updateDto)
        {
            return await _transportationService.Update(id, updateDto);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation("Delete transportation")]
        public async Task<ApiResponse> Delete(Guid id)
        {
            return await _transportationService.Delete(id);
        }

        [HttpPut("{id:guid}")]
        [SwaggerOperation("Update transportation's status")]
        public async Task<ApiResponse> UpdateStatus(Guid id, TransportUpdateStatusDto updateDto)
        {
            return await _transportationService.UpdateStatus(id, updateDto);
        }

    }
}
