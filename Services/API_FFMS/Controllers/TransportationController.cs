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

        [HttpPost]
        [SwaggerOperation("Create new transportation")]
        public async Task<ApiResponse> Create([FromBody] TransportCreateDto createDto)
        {
            return await _transportationService.Create(createDto);
        }

        [HttpPut("{id:guid}")]
        [SwaggerOperation("Update transportation")]
        public async Task<ApiResponse> UpdateTransportation(Guid id, TransportUpdateDto updateDto)
        {
            return await _transportationService.UpdateTransport(id, updateDto);
        }

        [HttpPut]
        [SwaggerOperation("Update asset in transportation")]
        public async Task<ApiResponse> UpdateTransportationDetail(Guid id, List<TransportDetailUpdateDto> updateDto)
        {
            return await _transportationService.UpdateTransportDetail(id, updateDto);
        }
    }
}
