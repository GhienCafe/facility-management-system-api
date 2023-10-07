using API_FFMS.Dtos;
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
    }
}
