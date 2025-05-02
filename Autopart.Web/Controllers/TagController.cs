using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Autopart.Api.Controllers
{
	[Route("api/Tag")]
	[ApiController]
	public class TagController : ControllerBase
	{
		private readonly ITagService _tagService;

		public TagController(ITagService tagService)
		{
			_tagService = tagService;
		}

		[Authorize]
		[HttpPost("tag")]
		public async Task<ActionResult<Tag>> CreateTag([FromBody] TagDto tagDto)
		{
			var createdTag = await _tagService.CreateTagAsync(tagDto);
			return Ok(createdTag);
		}

		[HttpGet("tag/{id}")]
		public async Task<ActionResult<TagDto>> GetTagById(int id)
		{
			var tag = await _tagService.GetTagByIdAsync(id);
			if (tag == null)
			{
				return Ok();
			}
			return Ok(tag);
		}

		[HttpGet("tag/slug/{slug}")]
		public async Task<ActionResult<TagDto>> GetTagBySlug(string slug)
		{
			var tag = await _tagService.GetTagBySlugAsync(slug);
			if (tag == null)
			{
				return Ok();
			}
			return Ok(tag);
		}

		[HttpGet("tags")]
		public async Task<ActionResult<IEnumerable<TagDto>>> GetAllTags(string? name, int page = 1, int limit = 10)
		{
			var (tags, totalCount) = await _tagService.GetTagsAsync(name!, page, limit);

			if (tags == null || !tags.Any())
			{
				return (new List<TagDto>());
			}

			var response = new
			{
				result = tags,
				total = totalCount,
				currentPage = page,
				count = tags.Count(),
				lastPage = (int)Math.Ceiling((double)totalCount / limit),
				firstItem = (page - 1) * limit + 1,
				lastItem = (page - 1) * limit + tags.Count(),
				perPage = limit,
			};

			return new JsonResult(response)
			{
				StatusCode = (int)HttpStatusCode.OK
			};
		}


		[HttpGet("tags/{param}")]
		public async Task<ActionResult<IEnumerable<TagDto>>> GetTagsByParam(string param, string language)
		{
			var tags = await _tagService.GetTagsByParamAsync(param, language);
			return Ok(tags);
		}

		[HttpGet("tagbyname/{param}")]
		public async Task<ActionResult<IEnumerable<TagDto>>> GetTagsByName(string param)
		{
			var tags = await _tagService.GetTagsByName(param);
			return Ok(tags);
		}

		[Authorize]
		[HttpPut("tag/{id}")]
		public async Task<IActionResult> UpdateTag([FromBody] UpdateTagDto tagDto)
		{
			//tagDto.Id = id;
			await _tagService.UpdateTagAsync(tagDto);
			return Ok();
		}

		[Authorize]
		[HttpDelete("tag/{id}")]
		public async Task<IActionResult> DeleteTag(int id)
		{
			await _tagService.RemoveTagAsync(id);
			return Ok($"Tag with Id = {id} deleted successfully");
		}
	}
}
