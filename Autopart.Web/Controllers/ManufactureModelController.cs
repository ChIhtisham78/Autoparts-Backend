using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Autopart.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ManufactureModelController : ControllerBase
	{
		private readonly IManufactureModelService _manufactureModelService;
		public ManufactureModelController(IManufactureModelService manufactureModelService)
		{
			_manufactureModelService = manufactureModelService;
		}


		[HttpPost("CreateManufactureModel")]
		public async Task<IActionResult> CreateManufactureModel([FromBody] AddManufactureModelDto manufacturerModel)
		{
			if (manufacturerModel == null)
			{
				return BadRequest("ManufacturerModel data is required.");
			}
			try
			{
				var createdManufacturerModel = await _manufactureModelService.CreateManufactureModelAsync(manufacturerModel);
				return Ok(createdManufacturerModel);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred while creating the ManufacturerModel.");
			}
		}


		[HttpGet("GetManufactureModel")]
		public async Task<IActionResult> GetSubCategories([FromQuery] int? manufacturerId)
		{
			try
			{
				var manufacture = await _manufactureModelService.GetManufactureByManufacturerIdAsync(manufacturerId);
				return Ok(manufacture);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred while retrieving the ManufacturerModel.");
			}
		}


		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteManufactureModel(int id)
		{
			await _manufactureModelService.DeleteByManufacturerIdAsync(id);
			return Ok(new { message = "Manufacture model deleted successfully." });
		}

	}
}
