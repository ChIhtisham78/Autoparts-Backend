using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Autopart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SVCRelationController : ControllerBase
    {
        private readonly ISVCRelationService _svcRelationService;

        public SVCRelationController(ISVCRelationService svcRelationService)
        {
            _svcRelationService = svcRelationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SVCRelationDto>>> GetSVCRelation([FromQuery] int? shopId = null, [FromQuery] string? size = null)
        {
            var result = await _svcRelationService.GetSVCRelationAsync(shopId, size);
            if (result == null || !result.Any())
            {
                return Ok(new List<SVCRelationDto>());
            }

            return Ok(result);
        }



        [HttpPost]
        public async Task<IActionResult> CreateSVCRelation([FromBody] SVCRelationDto dtos)
        {
            var result = await _svcRelationService.CreateSVCRelationAsync(dtos);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSVCRelation(int id, [FromBody] SVCRelationDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("Mismatched SVCRelation ID");
            }

            var result = await _svcRelationService.UpdateSVCRelationAsync(dto);

            if (result == null)
            {
                return Ok(new List<SVCRelationDto>());
            }

            return Ok(result);
        }


    }
}
