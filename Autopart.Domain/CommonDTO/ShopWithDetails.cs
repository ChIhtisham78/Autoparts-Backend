using Autopart.Domain.Models;

namespace Autopart.Domain.CommonDTO
{
	public class ShopWithDetails
	{
		public Shop Shop { get; set; }
		public string LogoUrl { get; set; }
		public string CoverUrl { get; set; }
		public Address ShopAddress { get; set; }
		public Setting Setting { get; set; }
		public Social Social { get; set; }
		public Balance Balance { get; set; }
		public AspNetUser Owner { get; set; }

		public int OrdersCount { get; set; }
		public int ProductCount { get; set; }
		public List<string> Roles { get; set; }
	}
}
