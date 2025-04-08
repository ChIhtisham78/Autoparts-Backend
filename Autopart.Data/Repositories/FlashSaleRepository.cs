using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Autopart.Data.Repositories
{
    public class FlashSaleRepository : IFlashSaleRepository
    {
        private readonly autopartContext _context;
        public IUnitOfWork UnitOfWork => _context;
        public FlashSaleRepository(autopartContext context)
        {
            _context = context;
        }
        public async Task DeleteImageAsync(int id)
        {
            var flashSaleImage = await _context.FlashSaleImages.FindAsync(id);
            if (flashSaleImage != null)
            {
                _context.FlashSaleImages.Remove(flashSaleImage);
                await _context.SaveChangesAsync();
            }
        }
        public void AddImage(FlashSaleImage image)
        {
            _context.FlashSaleImages.Add(image);
            _context.SaveChanges();
        }

        public async Task<IEnumerable<FlashSaleImage>> GetAllImagesAsync()
        {
            return await _context.FlashSaleImages.ToListAsync();
        }
        public async Task<FlashSaleImage> GetImageByIdAsync(int id)
        {
            return await _context.FlashSaleImages.FindAsync(id) ?? new FlashSaleImage();
        }
    }
}
