using Microsoft.AspNetCore.Mvc;

namespace Autopart.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FaqController : ControllerBase
	{
		// Static list to hold the FAQs
		private static List<Faq> faqs = new List<Faq>
		{
			new Faq
			{
				Id = 1,
				FaqTitle = "What is your return policy?",
				Slug = "what-is-your-return-policy",
				FaqDescription = "We have a flexible return policy. If you're not satisfied with your purchase, you can return most items within 30 days for a full refund or exchange. Please review our Return Policy for more details. You can check our refund policy in detail at our customer-refund-policy page.",
				FaqType = "global",
				IssuedBy = "Super Admin",
				Language = "en",
				TranslatedLanguages = new List<string> { "en" }
			},
			new Faq
			{
				Id = 2,
				FaqTitle = "Can I track my order?",
				Slug = "can-i-track-my-order",
				FaqDescription = "Yes, you can track your order's status. Once your order is shipped, you will receive a tracking number via email. You can use this tracking number to monitor the progress of your delivery.",
				FaqType = "global",
				IssuedBy = "Super Admin",
				Language = "en",
				TranslatedLanguages = new List<string> { "en" }
			},
			new Faq
			{
				Id = 3,
				FaqTitle = "How long will it take to receive my order?",
				Slug = "how-long-will-it-take-to-receive-my-order",
				FaqDescription = "Delivery times may vary depending on your location and the shipping method you choose. Typically, orders are processed and shipped within 1-2 business days. You can check the estimated delivery time during checkout.",
				FaqType = "global",
				IssuedBy = "Super Admin",
				Language = "en",
				TranslatedLanguages = new List<string> { "en" }
			},
			new Faq
			{
				Id = 4,
				FaqTitle = "Do you offer warranty on autoparts?",
				Slug = "do-you-offer-warranty-on-autoparts",
				FaqDescription = "Yes, we offer a warranty on most of our autoparts. The warranty period and terms may vary depending on the product. Please refer to the specific product page or contact our customer service for detailed warranty information.",
				FaqType = "global",
				IssuedBy = "Super Admin",
				Language = "en",
				TranslatedLanguages = new List<string> { "en" }
			},
			new Faq
			{
				Id = 5,
				FaqTitle = "How can I find the right part for my vehicle?",
				Slug = "how-can-i-find-the-right-part-for-my-vehicle",
				FaqDescription = "To find the right part for your vehicle, you can use our search tool by entering your vehicle's make, model, and year. You can also filter by part number or description to narrow down your search.",
				FaqType = "global",
				IssuedBy = "Super Admin",
				Language = "en",
				TranslatedLanguages = new List<string> { "en" }
			},
			new Faq
			{
				Id = 6,
				FaqTitle = "What should I do if the autopart doesn't fit my vehicle?",
				Slug = "what-should-i-do-if-the-autopart-doesnt-fit-my-vehicle",
				FaqDescription = "If the autopart doesn't fit your vehicle, please contact our customer support within 30 days of purchase. We will guide you through the return process and help you find the correct part.",
				FaqType = "global",
				IssuedBy = "Super Admin",
				Language = "en",
				TranslatedLanguages = new List<string> { "en" }
			},
			new Faq
			{
				Id = 7,
				FaqTitle = "Do you offer installation services for autoparts?",
				Slug = "do-you-offer-installation-services-for-autoparts",
				FaqDescription = "We do not offer installation services directly, but we can recommend certified professionals in your area who can install the autoparts for you.",
				FaqType = "global",
				IssuedBy = "Super Admin",
				Language = "en",
				TranslatedLanguages = new List<string> { "en" }
			},
			new Faq
			{
				Id = 8,
				FaqTitle = "Are all autoparts in stock ready for immediate shipping?",
				Slug = "are-all-autoparts-in-stock-ready-for-immediate-shipping",
				FaqDescription = "Most of our autoparts are in stock and ready for immediate shipping. However, some items may require additional processing time. You can check the availability status on the product page before placing your order.",
				FaqType = "global",
				IssuedBy = "Super Admin",
				Language = "en",
				TranslatedLanguages = new List<string> { "en" }
			}
		};

		[HttpGet("myfaqs")]
		public ActionResult<IEnumerable<Faq>> GetFaqs()
		{
			return Ok(faqs);
		}

		[HttpPost("faq")]
		public ActionResult AddFaq([FromBody] Faq newFaq)
		{
			newFaq.Id = faqs.Count + 1;
			faqs.Add(newFaq);
			return CreatedAtAction(nameof(GetFaqs), new { id = newFaq.Id }, newFaq);
		}
	}

	public class Faq
	{
		public int Id { get; set; }
		public string FaqTitle { get; set; }
		public string Slug { get; set; }
		public string FaqDescription { get; set; }
		public string FaqType { get; set; }
		public string IssuedBy { get; set; }
		public string Language { get; set; }
		public List<string> TranslatedLanguages { get; set; }
	}
}
