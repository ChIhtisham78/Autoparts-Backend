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
				//var paymentMethodService = new PaymentMethodService();
				//var paymentMethod = paymentMethodService.Get(paymentIntent.PaymentMethodId);
				// need to manage the payment succeeded process here
				await _stripeService.SavePaymentIntentAsync(paymentIntent);

			}
			else if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
			{
				var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
				//var paymentMethodService = new PaymentMethodService();
				//var paymentMethod = paymentMethodService.Get(paymentIntent.PaymentMethodId);
				//need to manage here the payment failed process here
			}
			else if (stripeEvent.Type == Events.AccountUpdated)
			{
				//need to manage the account details for the vendor 
			}
			else
			{
				Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
			}

			return Ok();
		}
	}
}
