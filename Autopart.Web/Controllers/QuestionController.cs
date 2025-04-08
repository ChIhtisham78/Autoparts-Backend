using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Autopart.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class QuestionController : ControllerBase
	{
		private readonly IQuestionService _questionService;

		public QuestionController(IQuestionService questionService)
		{
			_questionService = questionService;
		}

		//[Authorize]
		[HttpPost("question")]
		public async Task<ActionResult> PostQuestion(QuestionDto questionDto)
		{
			return Ok(await _questionService.AddQuestion(questionDto));
		}

		[HttpGet("questions")]
		public async Task<ActionResult> GetQuestions(int page = 1, int limit = 10, int? userId = null, int? productId = null)
		{
			var (questions, totalCount) = await _questionService.GetQuestions(userId, productId, page, limit);

			var response = new
			{
				result = questions ?? new List<GetQuestionDto>(),
				total = totalCount,
				currentPage = page,
				count = questions?.Count() ?? 0,
				lastPage = (int)Math.Ceiling((double)totalCount / limit),
				firstItem = (page - 1) * limit + 1,
				lastItem = (page - 1) * limit + (questions?.Count() ?? 0),
				perPage = limit,
			};

			return new JsonResult(response)
			{
				StatusCode = (int)HttpStatusCode.OK
			};
		}





		[HttpGet("question/{id}")]
		public async Task<ActionResult> GetQuestion(int id)
		{
			var questions = await _questionService.GetQuestion(id);
			return Ok(questions);

		}

		[HttpDelete("question/{id}")]
		public async Task<IActionResult> DeleteQuestion(int id)
		{
			var result = await _questionService.DeleteQuestionAsync(id);
			if (!result)
			{
				return NotFound(new { Message = "Question not found." });
			}

			return Ok(result);
		}
	}
}
