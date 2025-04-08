using Autopart.Domain.Models;

namespace Autopart.Domain.CommonModel
{
	public class GetOrdersDTO
	{
		public Order orders { get; set; }
		public Shipping shipping { get; set; }
		public Billing billing { get; set; }
		public AspNetUser user { get; set; }
		public Profile userProfile { get; set; }
		public Address address { get; set; }
		public ShippingAddress shippingAddress { get; set; }
		public BillingAddress billingAddress { get; set; }
		public Status status { get; set; }
		public Tax tax { get; set; }
		public Coupon coupon { get; set; }


	}
}
