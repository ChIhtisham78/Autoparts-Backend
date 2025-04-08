using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZikApp.API.Utilities;

namespace Autopart.Api.Controllers
{
    [Route("api/Attribute")]

    [ApiController]
    public class AttributeController : ControllerBase
    {
        private readonly IAttributeService _attributeService;

        public AttributeController(IAttributeService attributeService)
        {
            _attributeService = attributeService;
        }


        [Authorize]
        [HttpPost("attribute")]
        public async Task<ActionResult<Domain.Models.Attribute>> CreateAttribute([FromBody] AttributeDto attributeDto)
        {
            var userId = this.GetCurrentUserId();
            attributeDto.UserId = userId;

            var createdAttribute = await _attributeService.CreateAttributeAsync(attributeDto);
            return CreatedAtAction(
                    nameof(GetAttributeById),
                    new { id = createdAttribute.Id },
                    createdAttribute
            );
        }


        [HttpGet("attribute/{id}")]
        public async Task<ActionResult<AttributeDto>> GetAttributeById(int id)
        {
            var attribute = await _attributeService.GetAttributeByIdAsync(id);
            if (attribute == null)
            {
                return NotFound();
            }
            return Ok(attribute);
        }



        [HttpGet("attribute")]
        public async Task<ActionResult<IEnumerable<AttributeDto>>> GetAllAttributes(int? shopId = null)
        {
            var attributes = await _attributeService.GetAttributesAsync(shopId);
            if (attributes == null || !attributes.Any())
            {
                return NotFound(new { Message = "No Attributes Found For The Given Shop Id." });
            }
            return Ok(attributes);
        }




        [HttpGet("getattribute/{param}")]
        public async Task<ActionResult<IEnumerable<AttributeDto>>> GetAttributesByParam(string param)
        {
            var attributes = await _attributeService.GetAttributesByParamAsync(param);
            return Ok(attributes);
        }


        [Authorize]
        [HttpPut("attribute")]
        public async Task<IActionResult> UpdateAttribute(int id, [FromBody] AttributeDto attributeDto)
        {
            attributeDto.Id = id;
            await _attributeService.UpdateAttributeAsync(attributeDto);
            return Ok();
        }


        [Authorize]
        [HttpDelete("attribute/{id}")]
        public async Task<IActionResult> DeleteAttibute(int id)
        {
            await _attributeService.RemoveAttributeAsync(id);
            return Ok();
        }
    }
}
