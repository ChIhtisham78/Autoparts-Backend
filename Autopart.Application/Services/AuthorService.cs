using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Autopart.Application.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorReposiory _authorReposiory;
        private readonly ITypeAdapter _typeAdapter;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly RootFolder _rootFolder;

        public AuthorService(IAuthorReposiory authorReposiory, ITypeAdapter typeAdapter, IOptions<RootFolder> rootFolder, IWebHostEnvironment webHostEnvironment)
        {
            _authorReposiory = authorReposiory;
            _typeAdapter = typeAdapter;
            _rootFolder = rootFolder.Value;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<AuthorDto> CreateAuthorAsync(RequestAuthorDto requestAuthorDto)
        {
            try
            {
                var CImage = await UploadImage(requestAuthorDto.authorImageDto.CoverImage);
                var coverImage = new Image
                {
                    OriginalUrl = CImage,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                    UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                };

                await _authorReposiory.AddImage(coverImage);
                await _authorReposiory.UnitOfWork.SaveChangesAsync();

                var author = new Author
                {
                    Id = requestAuthorDto.authorDto.Id,
                    Name = requestAuthorDto.authorDto.Name,
                    Bio = requestAuthorDto.authorDto.Bio,
                    Quote = requestAuthorDto.authorDto.Quote,
                    IsApproved = requestAuthorDto.authorDto.IsApproved,
                    Slug = requestAuthorDto.authorDto.Slug,
                    ProductsCount = requestAuthorDto.authorDto.ProductsCount,
                    Born = requestAuthorDto.authorDto.Born,
                    Death = requestAuthorDto.authorDto.Death,
                    Languages = requestAuthorDto.authorDto.Languages,
                    ImageId = coverImage.Id,
                };
                var createdAuthor = await _authorReposiory.CreateAuthorAsync(author);
                var createdAuthorDto = _typeAdapter.Adapt<AuthorDto>(createdAuthor);
                return createdAuthorDto;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> UploadImage(IFormFile image)
        {
            var uploadsFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{image.FileName}";
            var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }
            return $"uploads/{uniqueFileName}";
        }

        public async Task<AuthorDto> GetAuthorByIdAsync(int id)
        {
            try
            {
                var author = await _authorReposiory.GetAuthorByIdAsync(id);
                return _typeAdapter.Adapt<AuthorDto>(author);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<AuthorDto>> GetAuthorBySlugAsync(string slug)
        {
            try
            {
                var author = await _authorReposiory.GetAuthorBySlugAsync(slug);
                return _typeAdapter.Adapt<IEnumerable<AuthorDto>>(author);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<AuthorDto>> GetAuthorsAsync()
        {
            try
            {
                var authors = await _authorReposiory.GetAuthorsAsync();
                return _typeAdapter.Adapt<IEnumerable<AuthorDto>>(authors);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task RemoveAuthorAsync(int id)
        {
            try
            {
                await _authorReposiory.RemoveAuthorAsync(id);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task UpdateAuthorAsync(RequestAuthorDto requestAuthorDto)
        {
            try
            {
                var existingAuthor = await _authorReposiory.GetAuthorByIdAsync(requestAuthorDto.authorDto.Id);

                if (existingAuthor == null)
                {
                    throw new KeyNotFoundException("Author not found.");
                }

                var coverImageId = 0;

                if (requestAuthorDto.authorImageDto != null && requestAuthorDto.authorImageDto.CoverImage != null)
                {
                    var CImage = await UploadImage(requestAuthorDto.authorImageDto.CoverImage);

                    var coverImage = new Image
                    {
                        OriginalUrl = CImage,
                        CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                        UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                    };

                    await _authorReposiory.AddImage(coverImage);
                    await _authorReposiory.UnitOfWork.SaveChangesAsync();
                    coverImageId = coverImage.Id;
                }

                existingAuthor.Name = requestAuthorDto.authorDto.Name;
                existingAuthor.Bio = requestAuthorDto.authorDto.Bio;
                existingAuthor.Quote = requestAuthorDto.authorDto.Quote;
                existingAuthor.IsApproved = requestAuthorDto.authorDto.IsApproved;
                existingAuthor.Slug = requestAuthorDto.authorDto.Slug;
                existingAuthor.ProductsCount = requestAuthorDto.authorDto.ProductsCount;
                existingAuthor.Born = requestAuthorDto.authorDto.Born;
                existingAuthor.Death = requestAuthorDto.authorDto.Death;
                existingAuthor.Languages = requestAuthorDto.authorDto.Languages;
                existingAuthor.ImageId = coverImageId;

                await _authorReposiory.UpdateAuthorAsync(existingAuthor);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
