namespace Autopart.Application.Models.Dto
{
    public class ShopAddressDto
    {
        public int Id { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public int? ShopId { get; set; }
        public string StreetAddress { get; set; }
    }
}
