using Autopart.Application.Interfaces;
using Autopart.Application.Models.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Autopart.Api.Controllers
{
    [Route("api/Shipping")]
    [ApiController]
    [Authorize]
    public class ShippingsController : ControllerBase
    {
        private readonly IShippingsService _shippingsService;

        public ShippingsController(IShippingsService shippingsService)
        {
            _shippingsService = shippingsService;
        }

        [Authorize]
        [HttpPost("shipping")]
        public async Task<ActionResult> PostShippings([FromBody] ShippingsDto shippingsDto)
        {
            var shippings = await _shippingsService.AddShippings(shippingsDto);

            if (shippings == null)
                return NotFound();

            return Ok(shippings);

        }


        [HttpGet("shippings")]
        public async Task<ActionResult> GetShippings()
        {
            var result = await _shippingsService.GetShippings();
            return Ok(new { result });
        }


        [HttpGet("shipping/{id}")]
        public async Task<ActionResult> GetShippingsById(int id)
        {
            var shipping = await _shippingsService.GetShippingsById(id);
            if (shipping == null)
            {
                return NotFound();
            }
            return Ok(shipping);
        }

        [Authorize]
        [HttpPut("shipping")]
        public async Task<ActionResult> PutShippings(int id, [FromBody] ShippingsDto shippingsDto)
        {
           var shipping = await _shippingsService.UpdateShippings(id, shippingsDto);
            if (shipping == null)
            {
                return NotFound();
            }
            return Ok(shipping);

        }

        [Authorize]
        [HttpDelete("shipping/{id}")]
        public async Task<ActionResult> DeleteShippings(int id)
        {
            await _shippingsService.RemoveShippings(id);
            return Ok($"Shipping with Id = {id} deleted successfully");
        }

    }
}
