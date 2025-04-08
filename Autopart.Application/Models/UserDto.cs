namespace Autopart.Application.Models
{
	public class UserDto
	{
		public int Id { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public string UserName { get; set; }
		public string StripeVendorId { get; set; }
		public string StripeDashboardAccess { get; set; }
		public string StripeVendorStatus { get; set; }
		public bool? IsActive { get; set; }
		public string Role { get; set; }
		public string[] Permissions { get; set; }
		public string ProfileImage { get; set; }
		public DateTime? CreatedAt { get; set; }
		public List<AddressFormatDto> Address { get; set; }
		public ProfileDto Profile { get; set; } // Single profile
		public List<AdminShopsDto>? Shops { get; set; }
	}
}
