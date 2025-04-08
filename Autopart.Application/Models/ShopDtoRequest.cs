namespace Autopart.Application.Models.Dto
{
    public class ShopDtoRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public string? CertificateUrl { get; set; }


        //public ShopDto shopDto { get; set; }
        public ShopAddressDto? shopAddressDto { get; set; }
        public SocialDto socialDtos { get; set; }
        public SettingDto? settingDto { get; set; }
        public BalanceDto? balanceDto { get; set; }

        public ShopImage? imageDto { get; set; }
    }
    public class ShopImage
    {
        public string Logo { get; set; }
        public string CoverImage { get; set; }
    }

    public class InvoiceRequest
    {
        public int OrderId { get; set; }
        public bool IsRtl { get; set; } = false;
        public string Language { get; set; } = "en";
        public TranslatedText TranslatedText { get; set; }
    }

    public class TranslatedText
    {
        public decimal? SalesTax { get; set; }
        public int? Quantity { get; set; }
        public int InvoiceNo { get; set; }
        public decimal? Total { get; set; }
        public string Product { get; set; }
        public decimal? PaidTotal { get; set; }
        public decimal? Discount { get; set; }
        public decimal? DeliveryFee { get; set; }
    }

}


