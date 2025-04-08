namespace Autopart.Application.Models
{
	public class OrderUserDto
	{
		public int Id { get; set; }

		public string Email { get; set; }

		public string PhoneNumber { get; set; }

		public string UserName { get; set; }
		public string Roles { get; set; }

		public bool IsActive { get; set; }
		public int? ShopId { get; set; }
		public bool? EmailConfirmed { get; set; }
		public DateTime? CreatedAt { get; set; }

		public virtual AddressDto AddressDto { get; set; }
	}
}
