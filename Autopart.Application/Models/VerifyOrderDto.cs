using Autopart.Domain.Models;

namespace Autopart.Application.Models
{
	public class VerifyOrderDto
	{
		public decimal? Amount { get; set; }
		public ShippingAddress? ShippingAddress { get; set; }
	}
}




