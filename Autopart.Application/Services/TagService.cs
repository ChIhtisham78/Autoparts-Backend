using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Application.Services
{
	public class TagService : ITagService
	{
		private readonly ITagRepository _tagRepository;
		private readonly ITypeAdapter _typeAdapter;

		public TagService(ITagRepository tagRepository, ITypeAdapter typeAdapter)
		{
			_tagRepository = tagRepository;
			_typeAdapter = typeAdapter;
		}

		public async Task<TagDto> CreateTagAsync(TagDto tagDto)
		{
			try
			{
				var slug = await EnsureUniqueSlugAsync(tagDto.Name, tagDto.Slug);
				var tag = new Tag
				{
					Id = tagDto.Id,
					Name = tagDto.Name,
					Slug = slug,
					Language = tagDto.Language,
					Icon = tagDto.Icon,
					Details = tagDto.Details

				};
				var createdTag = await _tagRepository.CreateTagAsync(tag);
				var createdTagDto = _typeAdapter.Adapt<TagDto>(createdTag);
				return createdTagDto;


				
			}
			catch (Exception ex)
			{

				throw;
			}

		}

		public async Task<IEnumerable<TagDto>> GetTagsByParamAsync(string param, string language)
		{
			var tags = await _tagRepository.GetTagByParamAsync(param, language);
			return _typeAdapter.Adapt<IEnumerable<TagDto>>(tags);
		}


		public async Task<IEnumerable<TagDto>> GetTagsByName(string param)
		{
			var tags = await _tagRepository.GetTagsByName(param);
			return _typeAdapter.Adapt<IEnumerable<TagDto>>(tags);
		}


		public async Task<TagDto> GetTagByIdAsync(int id)
		{
			var tag = await _tagRepository.GetTagByIdAsync(id);
			return _typeAdapter.Adapt<TagDto>(tag);
		}
		public async Task<TagDto> GetTagBySlugAsync(string sulg)
		{
			var tag = await _tagRepository.GetTagBySlug(sulg);
			return _typeAdapter.Adapt<TagDto>(tag);
		}

		public async Task<(IEnumerable<TagDto> Tags, int TotalCount)> GetTagsAsync(string param, int pageNumber = 1, int pageSize = 10)
		{
			var (tags, totalCount) = await _tagRepository.GetTagsAsync(param, pageNumber, pageSize);
			var tagDtos = _typeAdapter.Adapt<IEnumerable<TagDto>>(tags);
			return (tagDtos, totalCount);
		}

		public async Task RemoveTagAsync(int id)
		{
			await _tagRepository.RemoveTagAsync(id);
		}

		public async Task UpdateTagAsync(UpdateTagDto tagDto)
		{
			var existingTag = await _tagRepository.GetTagByIdAsync(tagDto.Id);

			if (existingTag == null)
			{
				throw new KeyNotFoundException("Category not found.");
			}

			existingTag.Name = tagDto.Name;
			existingTag.Slug = tagDto.Slug;
			existingTag.Details = tagDto.Details;

			await _tagRepository.UpdateTagAsync(existingTag);
		}

		private string GenerateSlug(string name)
		{
			return name.ToLower().Replace(" ", "-");
		}

		private async Task<string> EnsureUniqueSlugAsync(string name, string requestedSlug)
		{
			// Generate the initial slug from the requested name
			string slug = GenerateSlug(requestedSlug);

			// If the requested slug is not found in the database, return it directly
			if (!await _tagRepository.SlugExistsAsync(slug))
			{
				return slug;
			}

			// Otherwise, append a counter until a unique slug is found
			int counter = 1;
			string originalSlug = slug;

			while (await _tagRepository.SlugExistsAsync(slug))
			{
				slug = $"{originalSlug}-{counter}";
				counter++;
			}

			return slug;
		}
	}
}
