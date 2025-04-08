using Autopart.Application.Models;

namespace Autopart.Application.Interfaces
{
	public interface ITagService
	{
		Task<TagDto> CreateTagAsync(TagDto tagDto);
		Task<TagDto> GetTagByIdAsync(int id);
		Task<(IEnumerable<TagDto> Tags, int TotalCount)> GetTagsAsync(string param, int pageNumber = 1, int pageSize = 10);
		Task<IEnumerable<TagDto>> GetTagsByParamAsync(string param, string language);
		Task<IEnumerable<TagDto>> GetTagsByName(string name);
		Task<TagDto> GetTagBySlugAsync(string sulg);
		Task UpdateTagAsync(UpdateTagDto tagDto);
		Task RemoveTagAsync(int id);
	}
}
