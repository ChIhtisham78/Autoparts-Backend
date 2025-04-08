using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Autopart.Api.Controllers
{
    [Route("api/Coupon")]
   
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [Authorize]
        [HttpPost("coupon")]
        public async Task<ActionResult<Coupon>> CreateCoupon([FromBody] CouponDto couponDto)
        {
            if (couponDto == null || couponDto.ShopId == 0)
            {
                return BadRequest("CouponDto or ShopId is null or invalid");
            }

            //var shopId = this.GetCurrentUserId();
            //couponDto.ShopId = shopId;
            var createdCoupon = await _couponService.CreateCouponAsync(couponDto);
            return CreatedAtAction(
                    nameof(GetCouponById),
                    new { id = createdCoupon.Id },
                    createdCoupon
            );
        }

        [HttpGet("coupon/{id}")]
        public async Task<ActionResult<CouponDto>> GetCouponById(int id)
        {
            var coupon = await _couponService.GetCouponByIdAsync(id);
            if (coupon == null)
            {
                return Ok();
            }
            return Ok(coupon);
        }

        [HttpGet("code/{code}")]
        public async Task<ActionResult<CouponDto>> GetCouponByCode(string code, int shopId)
        {
            var coupon = await _couponService.GetCouponByCodeAsync(code, shopId);
            if (coupon == null)
            {
                return NotFound();
            }
            return Ok(coupon);
        }

        [HttpGet("coupon")]
        public async Task<ActionResult<IEnumerable<CouponDto>>> GetAllCoupons()
        {
            var coupons = await _couponService.GetCouponsAsync();
            return Ok(coupons);
        }

        [HttpGet("getcoupon/{param}")]
        public async Task<ActionResult<IEnumerable<CouponDto>>> GetCouponsByParam(string param, string language)
        {
            var coupons = await _couponService.GetCouponsByParamAsync(param, language);
            return Ok(coupons);
        }

        [HttpGet("coupons-by-shop/{shopId}")]
        public async Task<ActionResult<IEnumerable<CouponDto>>> GetCouponsByShopId(int shopId)
        {
            var coupons = await _couponService.GetCouponsByShopIdAsync(shopId);
            if (coupons == null || !coupons.Any())
            {
                return NotFound(new { Message = "No coupons found for the specified shop." });
            }
            return Ok(coupons);
        }

        [Authorize]
        [HttpPut("coupon/{id}")]
        public async Task<IActionResult> UpdateCoupon(int id, [FromBody] CouponDto couponDto)
        {
            couponDto.Id = id;
            await _couponService.UpdateCouponAsync(couponDto);
            return Ok();
        }

        [Authorize]
        [HttpDelete("coupons/{id}")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            await _couponService.RemoveCouponAsync(id);
            return Ok();
        }

        [Authorize]
        [HttpPut("approve-coupon/{id}")]
        public async Task<IActionResult> ApproveCoupon(int id)
        {
            await _couponService.ApproveCouponAsync(id);
            return Ok();
        }

        [Authorize]
        [HttpPut("disapprove-coupon/{id}")]
        public async Task<IActionResult> DisapproveCoupon(int id)
        {
            await _couponService.DisapproveCouponAsync(id);
            return Ok();
        }
    }
}
