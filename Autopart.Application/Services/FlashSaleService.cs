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
    public class FlashSaleService : IFlashSaleService
    {
        private readonly IFlashSaleRepository _flashSaleRepository;
        private readonly RootFolder _rootFolder;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ITypeAdapter _typeAdapter;

        public FlashSaleService(IFlashSaleRepository flashSaleRepository, ITypeAdapter typeAdapter, IOptions<RootFolder> rootFolder, IWebHostEnvironment webHostEnvironment)
        {
            _flashSaleRepository = flashSaleRepository;
            _typeAdapter = typeAdapter;
            _rootFolder = rootFolder.Value;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<bool> DeleteImageAsync(int id)
        {
            var image = await _flashSaleRepository.GetImageByIdAsync(id);
            if (image == null) return false;

            await _flashSaleRepository.DeleteImageAsync(id);
            return true;
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

            var flashSaleImage = new FlashSaleImage
            {
                Images = $"{_rootFolder.ApplicationUrl}uploads/{uniqueFileName}"
            };
            _flashSaleRepository.AddImage(flashSaleImage);
            await _flashSaleRepository.UnitOfWork.SaveChangesAsync();

            return flashSaleImage.Images;
        }
        public async Task<IEnumerable<FlashSaleImage>> GetAllImagesAsync()
        {
            return await _flashSaleRepository.GetAllImagesAsync();
        }



    }
}
