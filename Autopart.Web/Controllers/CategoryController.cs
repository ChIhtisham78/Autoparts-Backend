using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Autopart.Api.Controllers
{
	[Route("api/Category")]
	[ApiController]

	public class CategoryController : ControllerBase
	{
		private readonly ICategoryService _categoryService;

		public CategoryController(ICategoryService categoryService)
		{
			_categoryService = categoryService;
		}
		[Authorize]
		[HttpPost("category")]
		public async Task<ActionResult<Category>> CreateCategory([FromBody] CategoryDto categoryDto)
		{
			var createdCategory = await _categoryService.CreateCategoryAsync(categoryDto);
			return CreatedAtAction(
					nameof(GetCategoryById),
					new { id = createdCategory.Id },
					createdCategory
			);
		}

		[HttpGet("categoryById/{id}")]
		public async Task<ActionResult<CategoryDto>> GetCategoryById(int id)
		{
			var category = await _categoryService.GetCategoryByIdAsync(id);
			if (category == null)
			{
				return Ok();
			}
			return Ok(category);
		}

		[HttpGet("getcategory/{slug}")]
		public async Task<ActionResult<CategoryDto>> GetCategoryBySlug(string slug)
		{
			var category = await _categoryService.GetCategoryBySlugAsync(slug);
			if (category == null)
			{
				return Ok();
			}
			return Ok(category);
		}

		[HttpGet("Category")]
		public async Task<ActionResult> GetAllCategories(int page = 1, int limit = 10)
		{
			var (categories, totalCount) = await _categoryService.GetAllCategoriesAsync(page, limit);

			var response = new
			{
				result = categories,
				total = totalCount,
				currentPage = page,
				count = categories.Count(),
				lastPage = (int)Math.Ceiling((double)totalCount / limit),
				firstItem = (page - 1) * limit + 1,
				lastItem = (page - 1) * limit + categories.Count(),
				perPage = limit,
			};

			return new JsonResult(response)
			{
				StatusCode = (int)HttpStatusCode.OK
			};
		}


		[HttpGet("category/{param}/{language}")]
		public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategoriesByParam(string param, string language)
		{
			var categories = await _categoryService.GetCategoriesByParamAsync(param, language);
			return Ok(categories);
		}


		[HttpGet("category/{param}")]
		public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategoriesByName(string param)
		{
			var categories = await _categoryService.GetCategoriesByName(param);
			return Ok(categories);
		}

		[Authorize]
		[HttpPut("category/{id}")]
		public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDto categoryDto)
		{
			categoryDto.Id = id;
			await _categoryService.UpdateCategoryAsync(categoryDto);
			return Ok();
		}
		[Authorize]
		[HttpDelete("category/{id}")]
		public async Task<IActionResult> DeleteCategory(int id)
		{
			try
			{
				await _categoryService.DeleteCategoryAsync(id);
				return Ok();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

	}
}
