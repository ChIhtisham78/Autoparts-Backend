namespace Autopart.Application.Models
{
    public class AddUserResponse
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<AddressDto> Address { get; set; }

    }
}
