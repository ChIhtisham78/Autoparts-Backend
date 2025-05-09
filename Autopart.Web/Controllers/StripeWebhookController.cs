using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace Autopart.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class StripeWebhookController : ControllerBase
	{
		private readonly IStripeService _stripeService;
		private readonly StripeSetting _stripeSetting;
		public StripeWebhookController(IStripeService stripeService, IOptions<StripeSetting> stripeSetting)
		{
			_stripeService = stripeService;
			_stripeSetting = stripeSetting.Value;
		}

		[HttpPost("StripeWebhook")]

		public async Task<IActionResult> StripeWebhook()
		{
			var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
			var stripeEvent = EventUtility.ConstructEvent(json,
				Request.Headers["Stripe-Signature"], _stripeSetting.EndPointKey);
			if (stripeEvent.Type == Events.PaymentIntentSucceeded)
			{
				var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
				await _stripeService.SavePaymentIntentAsync(paymentIntent);

			}
			else if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
			{
				var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

			}
			else if (stripeEvent.Type == Events.AccountUpdated)
			{
			}
			else
			{
				Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
			}

			return Ok();
		}
	}
}
