namespace Autopart.Application.Models.Products
{
    public class ShippingsDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int? Amount { get; set; }
        public string Type { get; set; }
        public string TrackingNo { get; set; }
        public bool? Global { get; set; }
        public DateTime? CreatedAt { get; set; }
        public virtual ShippingAddressDto? ShippingAddressDto { get; set; }
    }
}
