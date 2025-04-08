using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Domain.Interfaces
{
    public interface IFlashSaleRepository : IRepository<FlashSaleImage>
    {
        Task DeleteImageAsync(int id);
        void AddImage(FlashSaleImage image);
        Task<IEnumerable<FlashSaleImage>> GetAllImagesAsync();
        Task<FlashSaleImage> GetImageByIdAsync(int id);
    }
}
