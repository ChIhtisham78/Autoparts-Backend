using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Domain.Interfaces
{
    public interface IAuthorReposiory : IRepository<Author>
    {
        Task<Author> CreateAuthorAsync(Author author);
        Task AddImage(Image image);
        Task<IEnumerable<Author>> GetAuthorBySlugAsync(string slug);
        Task<Author> GetAuthorByIdAsync(int id);
        Task<IEnumerable<Author>> GetAuthorsAsync();
        Task UpdateAuthorAsync(Author author);
        Task<Image> UpdateImage(Image image);
        Task RemoveAuthorAsync(int id);
        Task DeleteImages(int authorId);
    }
}
