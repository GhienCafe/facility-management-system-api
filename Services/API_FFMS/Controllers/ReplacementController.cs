﻿using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers
{
    public class ReplacementController : BaseController
    {
        private readonly IReplacementService _service;
        public ReplacementController(IReplacementService service)
        {
            _service = service;
        }

        [HttpPost]
        [SwaggerOperation("Create new replacement")]
        public async Task<ApiResponse> Create([FromBody] ReplaceCreateDto createDto)
        {
            return await _service.Create(createDto);
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation("Get a replacement")]
        public async Task<ApiResponse<ReplaceDto>> GetReplacement(Guid id)
        {
            return await _service.GetReplacement(id);
        }

        [HttpDelete]
        [SwaggerOperation("Delete a replacement")]
        public async Task<ApiResponse> Delete(Guid id)
        {
            return await _service.Delete(id);
        }

        [HttpGet]
        [SwaggerOperation("Get list replacement")]
        public async Task<ApiResponses<ReplaceDto>> GetReplaces([FromQuery] ReplacementQueryDto queryDto)
        {
            return await _service.GetReplaces(queryDto);
        }

        [HttpPut("{id:guid}")]
        [SwaggerOperation("Update a replacement")]
        public async Task<ApiResponse> Update(Guid id, BaseRequestUpdateDto updateDto)
        {
            return await _service.Update(id, updateDto);
        }
    }
}
