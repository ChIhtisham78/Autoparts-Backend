using Autopart.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Autopart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlashSaleController : ControllerBase
    {
        private readonly IFlashSaleService _flashSaleService;
        public FlashSaleController(IFlashSaleService flashSaleService)
        {
            _flashSaleService = flashSaleService;
        }


        [HttpDelete("delete-image/{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var result = await _flashSaleService.DeleteImageAsync(id);
            if (!result) return NotFound();
            return Ok();
        }

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            var imageUrl = await _flashSaleService.UploadImage(file);
            if (string.IsNullOrEmpty(imageUrl))
            {
                return NotFound();
            }
            return Ok(imageUrl);
        }
        [HttpGet("get-images")]
        public async Task<IActionResult> GetAllImages()
        {
            var images = await _flashSaleService.GetAllImagesAsync();
            if (images == null || !images.Any())
            {
                Console.WriteLine("No images found or an issue with the repository query.");
                return NotFound();
            }
            return Ok(images);
        }

    }
}
