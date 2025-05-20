using Autopart.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Autopart.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]

	public class LookUpsController : ControllerBase
	{
		private readonly IProductService _context;
		private readonly IOrdersService _ordersService;
		public LookUpsController(IProductService context, IOrdersService ordersService)
		{
			_context = context;
			_ordersService = ordersService;
		}

		[HttpGet("product/getshops/lookup")]
		public async Task<ActionResult> GetShopsByProduct()
		{
			var shopsbyproducts = await _context.GetShopsByProduct();
			return Ok(shopsbyproducts);
		}

		[HttpGet("product/getcategories/lookup")]
		public async Task<ActionResult> GetCategoriesByProduct()
		{
			var categoryproduct = await _context.GetCategoriesByProduct();
			return Ok(categoryproduct);
		}


		public async Task<IActionResult> UploadFile(IFormFile file)
		{
			if (file == null && file.Length == 0)
				return NotFound("File Not Found");
			long maxFileSize = 10 * 1024 * 1024;
			var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest("Invalid file type. Only .jpg, .jpeg, .png, and .gif are allowed.");
            }
            if (file.Length > maxFileSize)
            {
                return BadRequest($"File size exceeds the limit of {maxFileSize / (1024 * 1024)} MB.");

            }
            var filePath = Path.Combine("uploads", file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return Ok(new { filePath });
        }


        [HttpGet("product/gettags/lookup")]
		public async Task<ActionResult> GetTagsByProduct()
		{
			var tagsbyprducts = await _context.GetTagsByProduct();
			return Ok(tagsbyprducts);
		}

		[HttpGet("product/getauthors/lookup")]
		public async Task<ActionResult> GetAuthorsByProduct()
		{
			var authorsbyproduct = await _context.GetAuthorsByProduct();
			return Ok(authorsbyproduct);
		}

		[HttpGet("product/getmanufacturers/lookup")]
		public async Task<ActionResult> GetManufacturersByProduct()
		{
			var manufacturerbyproduct = await _context.GetManufacturersByProduct();
			return Ok(manufacturerbyproduct);
		}

		[HttpGet("GetOrderStatuses")]
		public async Task<ActionResult> GetOrderStatuses()
		{
			var orderstatus = await _ordersService.OrderStatuesLookup();
			return Ok(orderstatus);
		}
	}
}
