using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;

namespace Autopart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class StripePaymentGatewayController : Controller
    {
        private readonly IStripeService _stripeService;
        private readonly StripeSetting _stripeSetting;

        public StripePaymentGatewayController(IStripeService stripeService, IOptions<StripeSetting> stripeSetting)
        {
            _stripeService = stripeService;
            _stripeSetting = stripeSetting.Value;
        }
        [HttpPost("createcheckoutsession")]
        public async Task<IActionResult> CreateCheckoutSession(int orderId,int CouponId)
        {
            var session = await _stripeService.CreateCheckoutSessionAsync(orderId, CouponId);
            if (session == null)
            {
                return BadRequest("Failed to create checkout session");
            }
            return Ok(session.Url);
        }
        [HttpPost("createstripevendor")]
        public async Task<IActionResult> CreateStripeVendor(int userId)
        {
            var accountId = await _stripeService.CreateVendorAccountAsync(userId);
            if (string.IsNullOrEmpty(accountId))
            {
                return BadRequest("Failed to create vendor account");
            }

            var accountLinkUrl = await _stripeService.GenerateAccountLinkAsync(accountId);
            if (string.IsNullOrEmpty(accountLinkUrl))
            {
                return BadRequest("Failed to generate account link");
            }

            return Ok(new {accountId = accountId, accountLinkUrl = accountLinkUrl, success = true });
        }

       
        [HttpGet("payment-history")]
        public async Task<ActionResult> GetPaymentHistory()
        {
            var paymentHistory = await _stripeService.GetPaymentHistory();
            return Ok(paymentHistory);

        }
        [HttpGet("payment-history/user/{userId}")]
        public async Task<ActionResult> GetPaymentHistoryByUserIdAsync(int userId)
        {
            var paymentHistoryDtos = await _stripeService.GetPaymentHistoryByUserIdAsync(userId);

            if (paymentHistoryDtos == null || !paymentHistoryDtos.Any())
            {
                return NotFound(new { Message = $"No payment history found for user ID {userId}." });
            }

            return Ok(paymentHistoryDtos);
        }
        [HttpGet("payment-history/vendor/{vendorId}")]
        public async Task<ActionResult> GetPaymentHistoryByVendorIdAsync(string vendorId)
        {
            var paymentHistoryDtos = await _stripeService.GetPaymentHistoryByVendorIdAsync(vendorId);

            if (paymentHistoryDtos == null || !paymentHistoryDtos.Any())
            {
                return NotFound(new { Message = $"No payment history found for vendor ID {vendorId}." });
            }

            return Ok(paymentHistoryDtos);
        }
        [HttpPost("VendorPayout")]
        public async Task<ActionResult> CreatePayout(string VendorId, decimal amount)
        {
            var payout = await _stripeService.CreatePayoutToBank(VendorId, amount);
            return Ok(payout);
        }
        [HttpGet("checkVendorAccountStatus")]
        public IActionResult CheckAccountStatus(string accountId)
        {
           var res = _stripeService.CheckAccountStatus(accountId);
            return Json(res);
        }
    }
}
