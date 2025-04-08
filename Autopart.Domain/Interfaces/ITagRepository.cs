using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Domain.Interfaces
{
	public interface ITagRepository : IRepository<Tag>
	{
		Task<Tag> CreateTagAsync(Tag tag);
		Task<Tag> GetTagByIdAsync(int id);
		Task<(IEnumerable<Tag> Tags, int TotalCount)> GetTagsAsync(string name, int pageNumber = 1, int pageSize = 10);
		Task<IEnumerable<Tag>> GetTagByParamAsync(string param, string language);
		Task<IEnumerable<Tag>> GetTagsByName(string param);
		Task<Tag> GetTagBySlug(string slug);

		Task UpdateTagAsync(Tag tag);
		Task RemoveTagAsync(int id);
		Task<bool> SlugExistsAsync(string slug);
	}
}
