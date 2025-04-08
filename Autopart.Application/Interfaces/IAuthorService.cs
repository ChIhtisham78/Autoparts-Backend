using Autopart.Application.Models;
using Microsoft.AspNetCore.Http;

namespace Autopart.Application.Interfaces
{
    public interface IAuthorService
    {
        Task<AuthorDto> CreateAuthorAsync(RequestAuthorDto requestAuthorDto);
        Task<string> UploadImage(IFormFile image);
        Task<IEnumerable<AuthorDto>> GetAuthorBySlugAsync(string slug);
        Task<AuthorDto> GetAuthorByIdAsync(int id);
        Task<IEnumerable<AuthorDto>> GetAuthorsAsync();
        Task UpdateAuthorAsync(RequestAuthorDto requestAuthorDto);
        Task RemoveAuthorAsync(int id);
    }
}
