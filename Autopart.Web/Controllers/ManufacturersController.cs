using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Autopart.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]

	public class ManufacturersController : ControllerBase
	{
		private readonly IManufacturersService _manufacturersService;

		public ManufacturersController(IManufacturersService manufacturersService)
		{
			_manufacturersService = manufacturersService;
		}

		[HttpGet("manufacturers/{slug}")]
		public async Task<ActionResult<object>> GetManufacturerBySlug(string slug)
		{
			var manufacturer = await _manufacturersService.GetManufacturerBySlugAsync(slug);

			if (manufacturer == null)
			{
				return Ok(new List<object>());
			}

			return Ok(manufacturer);
		}


		[HttpGet("manufacturers")]
		public async Task<ActionResult> GetManufacturers(int page = 1, int limit = 10)
		{
			var (manufacturers, totalCount) = await _manufacturersService.GetManufacturers(page, limit);

			var response = new
			{
				result = manufacturers,
				total = totalCount,
				currentPage = page,
				count = manufacturers.Count,
				lastPage = (int)Math.Ceiling((double)totalCount / limit),
				firstItem = (page - 1) * limit + 1,
				lastItem = (page - 1) * limit + manufacturers.Count,
				perPage = limit,
			};
			return new JsonResult(response)
			{
				StatusCode = (int)HttpStatusCode.OK
			};
		}



		[HttpGet("manufacturer/{id}")]
		public async Task<ActionResult> GetManufacturerById(int id)
		{
			return Ok(await _manufacturersService.GetManufacturerById(id));
		}

		[Authorize]
		[HttpDelete("manufacturer/{id}")]
		public async Task<ActionResult> DeleteManufacturer(int id)
		{
			await _manufacturersService.RemoveManufacturer(id);
			return Ok($"Manufacturer with Id = {id} deleted successfully");
		}

		[Authorize]
		[HttpPost("manufacturer")]
		public async Task<ActionResult> PostManufacturer([FromBody] ManufacturerRequest request)
		{
			return Ok(await _manufacturersService.AddManufacturers(request.manufacturersDto, request.imageDto, request.socialDto/*, request.typeDto, request.bannerDto*/));
		}

		[Authorize]
		[HttpPut("manufacturer/{id}")]
		public async Task<ActionResult> PutManufacturer([FromBody] ManufacturerRequest request)
		{
			if (request.manufacturersDto.Id == 0)
			{
				return BadRequest("Manufacturer id is required.");
			}
			try
			{
				await _manufacturersService.UpdateManufacturers(request.manufacturersDto, request.socialDto, request.imageDto/*, request.typeDto, request.bannerDto*/);
				return Ok(new { updated = true });
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating user: {ex.Message}");
			}

		}
	}
}
