using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Autopart.Data.Repositories
{
    public class AuthorRepository : IAuthorReposiory
    {
        private readonly autopartContext _context;

        public IUnitOfWork UnitOfWork => _context;

        public AuthorRepository(autopartContext context)
        {
            _context = context;
        }

        public async Task<Author> CreateAuthorAsync(Author author)
        {
            try
            {
                await _context.Authors.AddAsync(author);
                await _context.SaveChangesAsync();
                return author;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<string> GetImageById(int? imageId)
        {
            var image = await _context.Images.FirstOrDefaultAsync(x => x.Id == imageId);
            return image!.OriginalUrl;
        }

        public async Task<IEnumerable<Author>> GetAuthorBySlugAsync(string slug)
        {
            return await _context.Authors.Where(x => x.Slug.Contains(slug)).ToListAsync();
        }

        public async Task<IEnumerable<Author>> GetAuthorsAsync()
        {
            return await _context.Authors.ToListAsync();
        }

        public async Task UpdateAuthorAsync(Author author)
        {
            _context.Authors.Update(author);
            await _context.SaveChangesAsync();
        }

        public async Task<Image> UpdateImage(Image image)
        {
            var images = _context.Images.Update(image);
            return images.Entity;
            
        }

        public async Task RemoveAuthorAsync(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author != null)
            {
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Author> GetAuthorByIdAsync(int id)
        {
            return await _context.Authors.FindAsync(id) ?? new Author();
        }

        public async Task AddImage(Image image)
        {
            await _context.Images.AddAsync(image);
        }

        public async Task DeleteImages(int authorId)
        {
            var author = await _context.Authors.Where(i => i.Id == authorId).FirstOrDefaultAsync();
            var imagesToDelete = new List<Image>();
            var coverImage = await _context.Images.Where(x => x.Id == author!.CoverImageId).FirstOrDefaultAsync();
            if (coverImage != null)
            {
                imagesToDelete.Add(coverImage);
            }
        }
    }
}
