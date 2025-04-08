using Microsoft.AspNetCore.Mvc;

namespace Autopart.Api.Controllers
{
	[Route("api/Type")]
	[ApiController]
	public class TypeController : ControllerBase
	{
		[HttpGet("settings")]
		public async Task<IActionResult> GetAllSettings()
		{
			var response = new Type
			{
				id = 9,
				name = "Gadget",
				language = "en",
				translated_languages = new List<string> { "en" },
				slug = "gadget",
				banners = new List<Banner>
				{
					new Banner
					{
						id = 78,
						title = "Gadget",
						type_id = 9,
						description = "Add your banner image with title and description from here. Dimension of the banner should be 1920 x 1080 px for full screen banner and 1500 x 450 px for small banner",
						image = new Image
						{
							id = 2149,
							thumbnail = "https://i.ibb.co/M1B6XCj/Eco-Parts.png",
							original = "https://quantino.tech/wp-content/uploads/2024/05/Eco-Parts.png",
						}
					}
				},
				settings = new Settings
				{
					isHome = false,
					productCard = "neon",
					layoutType = "modern",
				},
				icon = "Gadgets"
			};

			var result = new
			{
				options = new[] { response }
			};

			return new JsonResult(result);
		}
	}

	public class Banner
	{
		public int id { get; set; }
		public string title { get; set; }
		public int type_id { get; set; }
		public string description { get; set; }
		public Image image { get; set; }
	}

	public class Image
	{
		public int id { get; set; }
		public string thumbnail { get; set; }
		public string original { get; set; }
	}

	public class Type
	{
		public int id { get; set; }
		public string name { get; set; }
		public string language { get; set; }
		public List<string> translated_languages { get; set; }
		public string slug { get; set; }
		public List<Banner> banners { get; set; }
		public Settings settings { get; set; }
		public string icon { get; set; }
	}

	public class Settings
	{
		public bool isHome { get; set; }
		public string productCard { get; set; }
		public string layoutType { get; set; }
	}
}
