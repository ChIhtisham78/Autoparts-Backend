using Autopart.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Autopart.Application.Interfaces
{
    public interface IFlashSaleService
    {
        Task<bool> DeleteImageAsync(int id);
        Task<string> UploadImage(IFormFile image);
        Task<IEnumerable<FlashSaleImage>> GetAllImagesAsync();

    }
}
