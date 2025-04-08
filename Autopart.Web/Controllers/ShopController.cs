using Autopart.Application.Interfaces;
using Autopart.Application.Models.Dto;
using Autopart.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZikApp.API.Utilities;

namespace Autopart.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]

	public class ShopController : ControllerBase
	{
		private readonly IShopService _shopService;

		public ShopController(IShopService shopService)
		{
			_shopService = shopService;
		}
		[Authorize]
		[HttpPost("shop")]
		public async Task<ActionResult<ShopDto>> AddShop([FromBody] ShopDtoRequest request)
		{
			var userId = this.GetCurrentUserId();
			if (request == null)
			{
				return BadRequest();
			}
			var createdShop = await _shopService.AddShop(request, userId);
			return Ok(createdShop);
		}

		[HttpGet("shop")]
		public async Task<ActionResult<List<Shop>>> GetShops()
		{
			var shops = await _shopService.GetShops();
			return Ok(shops);
		}

		[HttpGet("Slug/{slug}")]
		public async Task<ActionResult<ShopDto>> GetShopBySlug(string slug)
		{
			var shop = await _shopService.GetShopBySlug(slug);
			if (shop == null)
			{
				return Ok();
			}
			return Ok(shop);
		}

		[HttpGet("{Id}")]
		public async Task<ActionResult<ShopDto>> GetShop(int Id)
		{
			var shop = await _shopService.GetShop(Id);
			if (shop == null)
			{
				return NotFound();
			}
			return Ok(shop);
		}

		//[HttpGet("slug/{slug}")]
		//public async Task<ActionResult<ShopDto>> GetShopBySlug(string slug)
		//{
		//    var shop = await _shopService.GetShopBySlug(slug);
		//    if (shop == null)
		//    {
		//        return NotFound();
		//    }
		//    return Ok(shop);
		//}
		[Authorize]
		[HttpPut("shop/{id}")]
		public async Task<ActionResult<ShopDto>> UpdateShop([FromBody] ShopDto shopDto)
		{
			if (shopDto.Id == null)
			{
				return BadRequest();
			}
			var updatedShop = await _shopService.UpdateShop(shopDto);
			if (updatedShop == null)
			{
				return NotFound();
			}

			return Ok(updatedShop);
		}
		[Authorize]
		[HttpDelete("shop/{id}")]
		public async Task<ActionResult> DeleteShop(int id)
		{
			var result = await _shopService.DeleteShop(id);
			if (!result)
			{
				return NotFound();
			}

			return Ok();
		}
		[Authorize]
		[HttpPost("shop/approve")]
		public async Task<ActionResult<ShopDto>> ApproveShop(int id)
		{
			var approvedShop = await _shopService.ApproveShop(id);
			if (approvedShop == null)
			{
				return NotFound();
			}

			return Ok(approvedShop);
		}
		[Authorize]
		[HttpPost("shop/disapprove")]
		public async Task<ActionResult<ShopDto>> DisapproveShop(int id)
		{
			var disapprovedShop = await _shopService.DisapproveShop(id);
			if (disapprovedShop == null)
			{
				return NotFound();
			}

			return Ok(disapprovedShop);
		}


		//[HttpPost("upload/image")]
		//public async Task<ActionResult> UploadImage(IFormFile file)
		//{
		//        var image = await _shopService.UploadImage(file);
		//        if (image == null)
		//        {
		//            return NotFound();
		//        }

		//[HttpPost("shop/upload/image")]
		//public async Task<ActionResult> UploadImage(IFormFile file)
		//{
		//    var image = await _shopService.UploadImage(file);
		//    if (image == null)
		//    {
		//        return NotFound();
		//    }

		//    return Ok(image);
		//}
		[HttpGet("owner/{ownerId}")]
		public async Task<IActionResult> GetShopsByOwnerId(int ownerId)
		{
			var shops = await _shopService.GetShopsByOwnerIdAsync(ownerId);
			return Ok(shops);
		}

	}
}
