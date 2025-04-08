namespace Autopart.Application.Models
{
	public class AuthenticatedUserResponse
	{
		public int Id { get; set; }
		public string? Email { get; set; }
		public string? UserName { get; set; }
		public string? PhoneNumber { get; set; }
		public string? Role { get; set; }
		public string[]? Permissions { get; set; }
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }

		public AuthenticatedUser User { get; set; }
		public List<AddressDto> Address { get; set; }
	}
}
