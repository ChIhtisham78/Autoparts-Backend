namespace Autopart.Application.Models
{
	public class AddUserDto
	{
		public int Id { get; set; }
		public string Email { get; set; }
		public string UserName { get; set; }
		public string PhoneNumber { get; set; }
		public string ProfileImage { get; set; }
		public string Password { get; set; }
		public DateTime? CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public List<AddressFormatDto> Address { get; set; }




	}
}
