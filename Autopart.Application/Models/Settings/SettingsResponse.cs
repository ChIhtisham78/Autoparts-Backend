using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Application.Models.Settings
{
    public class SettingsResponse
    {
        public List<DeliveryTime> DeliveryTime { get; set; }
        public bool IsProductReview { get; set; }
        public bool UseGoogleMap { get; set; }
        public bool EnableTerms { get; set; }
        public bool EnableCoupons { get; set; }
        public bool EnableReviewPopup { get; set; }
        public ReviewSystem ReviewSystem { get; set; }
        public Seo Seo { get; set; }
        public Logo Logo { get; set; }
        public Logo CollapseLogo { get; set; }
        public bool UseOtp { get; set; }
        public string Currency { get; set; }
        public string TaxClass { get; set; }
        public string SiteTitle { get; set; }
        public bool FreeShipping { get; set; }
        public int SignupPoints { get; set; }
        public string SiteSubtitle { get; set; }
        public string ShippingClass { get; set; }
        public bool UseEnableGateway { get; set; }
        public bool UseCashOnDelivery { get; set; }
        public ContactDetails ContactDetails { get; set; }
        public List<PaymentGateway> paymentGateway { get; set; }
        // Add other properties as needed
    }
}
