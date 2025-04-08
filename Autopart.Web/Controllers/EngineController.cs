using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Autopart.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EngineController : ControllerBase
	{
		private readonly IEngineService _engineService;
		public EngineController(IEngineService engineService)
		{
			_engineService = engineService;
		}


		[HttpPost("CreateEngine")]
		public async Task<IActionResult> CreateSubCategory([FromBody] AddEngineDto engineDto)
		{
			if (engineDto == null)
			{
				return BadRequest("Engine data is required.");
			}
			try
			{
				var engine = await _engineService.CreateEngineAsync(engineDto);
				return Ok(engine);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred while creating the engine.");
			}
		}


		[HttpGet("GetEngines")]
		public async Task<IActionResult> GetEngines([FromQuery] int? categoryId, [FromQuery] int? subcategoryId, [FromQuery] int? modelId, [FromQuery] int? manufacturerId)
		{
			try
			{
				var engines = await _engineService.GetEnginesAsync(categoryId, subcategoryId, modelId, manufacturerId);
				return Ok(engines);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred while retrieving the engines.");
			}
		}

		[HttpGet("GetEnginesByParams")]
		public async Task<IActionResult> GetEnginesByParams([FromQuery] int? year, [FromQuery] int? categoryId, [FromQuery] int? subcategoryId, [FromQuery] int? manufacturerId, [FromQuery] int? modelId)
		{
			try
			{
				var engines = await _engineService.GetEnginesByParamsAsync(year, categoryId, subcategoryId, manufacturerId, modelId);
				return Ok(engines);
			}
			catch (Exception)
			{
				return StatusCode(500, "An error occurred while retrieving engines.");
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteEngine(int id)
		{
			await _engineService.DeleteEngineByIdAsync(id);
			return Ok(new { message = "Engine deleted successfully." });
		}

	}
}
