using Autopart.Application.Models.Settings;
using Microsoft.AspNetCore.Mvc;

namespace Autopart.Api.Controllers
{
	[Route("api/Settings")]
	[ApiController]
	//[Authorize]
	public class SettingsController : ControllerBase
	{
		[HttpGet("settings")]
		public async Task<IActionResult> GetAllSettings()
		{
			var response = new SettingsResponse
			{
				DeliveryTime = new List<DeliveryTime>
		{
			new DeliveryTime { Title = "Express Delivery", Description = "90 min express delivery" },
			new DeliveryTime { Title = "Morning", Description = "8.00 AM - 11.00 AM" },
			new DeliveryTime { Title = "Noon", Description = "11.00 AM - 2.00 PM" },
			new DeliveryTime { Title = "Afternoon", Description = "2.00 PM - 5.00 PM" },
			new DeliveryTime { Title = "Evening", Description = "5.00 PM - 8.00 PM" }
		},
				IsProductReview = false,
				UseGoogleMap = false,
				EnableTerms = true,
				EnableCoupons = true,
				EnableReviewPopup = true,
				ReviewSystem = new ReviewSystem
				{
					Value = "review_single_time",
					Name = "Give purchased product a review only for one time. (By default)"
				},
				Seo = new Seo
				{
					OgImage = null,
					OgTitle = null,
					MetaTags = null,
					MetaTitle = null,
					MetaDescription = null,
					CanonicalUrl = null,
					OgDescription = null,
					TwitterCardType = null,
					TwitterHandle = null,
				},
				paymentGateway = new List<PaymentGateway>
		{
			new PaymentGateway { Name = "stripe", Title = "Stripe" }
		},
				Logo = new Logo
				{
					Thumbnail = "https://app.easypartshub.com/uploads/d0efa217-fbf9-4dd3-a8d7-a2db3342240f_EASY_PARTS-removebg-preview.png",
					Original = "https://app.easypartshub.com/uploads/d0efa217-fbf9-4dd3-a8d7-a2db3342240f_EASY_PARTS-removebg-preview.png",
					Id = 2298,
					FileName = "Logo-new.png"
				},
				CollapseLogo = new Logo
				{
					Thumbnail = "https://app.easypartshub.com/uploads/9d7d6582-8836-496b-a734-4535b5ada47d_Easy_Parts_Hub-01[1].png",
					Original = "https://app.easypartshub.com/uploads/9d7d6582-8836-496b-a734-4535b5ada47d_Easy_Parts_Hub-01[1].png",
					Id = 2286,
					FileName = "Pickbazar.png"
				},
				UseOtp = false,
				Currency = "USD",
				TaxClass = "1",
				SiteTitle = "EasyPartsHub",
				FreeShipping = false,
				SignupPoints = 100,
				SiteSubtitle = "Your next Autoparts Hub",
				ShippingClass = "1",
				ContactDetails = new ContactDetails
				{
					Contact = "+129290122122",
					Socials = new List<Social>
			{
				new Social { Url = "https://www.facebook.com/redqinc", Icon = "FacebookIcon" },
				new Social { Url = "https://twitter.com/RedqTeam", Icon = "TwitterIcon" },
				new Social { Url = "https://www.instagram.com/redqteam", Icon = "InstagramIcon" }
			},
					Website = "https://redq.io",
					EmailAddress = "demo@demo.com",
					Location = new Location
					{
						Lat = 42.9585979,
						Lng = -76.9087202,
						Zip = null,
						City = null,
						State = "NY",
						Country = "United States",
						FormattedAddress = "NY State Thruway, New York, USA"
					}
				},
				UseEnableGateway = true,
				UseCashOnDelivery = false
			};

			var result = new
			{
				options = response
			};

			return new JsonResult(result);
		}
















	}
}
