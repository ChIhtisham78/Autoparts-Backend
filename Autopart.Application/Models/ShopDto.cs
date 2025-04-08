namespace Autopart.Application.Models.Dto
{
    public class ShopDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? OwnerId { get; set; }
        public bool? IsActive { get; set; }
        public int ProductCount { get; set; }
        public int OrdersCount { get; set; }
        public string OwnerName { get; set; }
        public string CertificateUrl { get; set; }
        public bool? Status { get; set; }
        public string Logo { get; set; }
        public string CoverImage { get; set; }
        public string Slug { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Description { get; set; }
        public ShopAddressDto? shopAddressDto { get; set; }
        public SettingDto? settingDto { get; set; }
        public SocialDto? socialDto { get; set; }
        public BalanceDto? balanceDto { get; set; }
        public ShopImage? imageDto { get; set; }
        ShopDetailDto shopDetailDto { get; set; }
        public OrderUserDto? OrderUserDto { get; set; }
    }
}
