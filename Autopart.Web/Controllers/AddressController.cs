using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Autopart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet("addresses")]
        public async Task<ActionResult> GetAddresses()
        {

            return Ok(await _addressService.GetAddresses());

        }

        [HttpGet("address/{id}")]
        public async Task<ActionResult> GetAddress(int id)
        {
            return Ok(await _addressService.GetAddressById(id));
        }


        [Authorize]
        [HttpDelete("address/{id}")]
        public async Task<ActionResult> DeleteAddress(int id)
        {
            await _addressService.RemoveAddress(id);
            return Ok($"User with Id = {id} deleted successfully");
        }


        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult> PutAddress(int id, [FromBody] AddressDto addressDto)
        {
            addressDto.Id = id;
            if (id != addressDto.Id)
            {
                return BadRequest("User ID in the route and body do not match.");
            }
            try
            {
                await _addressService.UpdateAddress(addressDto);
                return Ok(addressDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating user: {ex.Message}");
            }

        }


        [Authorize]
        [HttpPost("address")]
        public async Task<ActionResult> PostAddress([FromQuery] AddressDto addressDto)
        {
            return Ok(await _addressService.AddAddress(addressDto));
        }
    }
}
