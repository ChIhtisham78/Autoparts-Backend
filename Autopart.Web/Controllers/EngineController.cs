using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Domain.CommonDTO;
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
		public async Task<IActionResult> GetEngines([FromQuery] GetEnginesDTO enginesDTO)
		{
			try
			{
				var engines = await _engineService.GetEnginesAsync(enginesDTO);
				return Ok(engines);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred while retrieving the engines.");
			}
		}

		[HttpGet("GetEnginesByParams")]
		public async Task<IActionResult> GetEnginesByParams([FromQuery] GetEnginesDTO enginesDTO)
		{
			try
			{
				var engines = await _engineService.GetEnginesByParamsAsync(enginesDTO);
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
