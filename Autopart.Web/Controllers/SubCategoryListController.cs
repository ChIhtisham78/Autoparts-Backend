using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Autopart.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SubCategoryListController : ControllerBase
	{
		private readonly ISubCategoryListService _subCategoryListService;
		public SubCategoryListController(ISubCategoryListService subCategoryListService)
		{
			_subCategoryListService = subCategoryListService;
		}

		[HttpPost("CreateSubCategory")]
		public async Task<IActionResult> CreateSubCategory([FromBody] AddSubCategoryListDto dto)
		{
			try
			{
				var createdSubCategory = await _subCategoryListService.CreateSubCategoryAsync(dto);
				return Ok(createdSubCategory);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred while creating the subcategory.");
			}
		}

		[HttpGet("GetSubCategories")]
		public async Task<IActionResult> GetSubCategories([FromQuery] int? categoryId)
		{
			try
			{
				var subCategories = await _subCategoryListService.GetSubCategoriesByCategoryIdAsync(categoryId);
				return Ok(subCategories);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred while retrieving the subcategories.");
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteSubCategory(int id)
		{
			await _subCategoryListService.DeleteSubCategoryByIdAsync(id);
			return Ok(new { message = "subcategories deleted successfully." });
		}

	}
}
