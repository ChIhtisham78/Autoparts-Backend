using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autopart.Application.Models;
using Stripe;
using Stripe.Checkout;
using static Autopart.Application.Services.StripeService;

namespace Autopart.Application.Interfaces
{
    public interface IStripeService
    {
        Task<Session> CreateCheckoutSessionAsync(int OrderId, int CouponId);
        Task<string> CreateVendorAccountAsync(int userId);
        Task<string> GenerateAccountLinkAsync(string accountId);
        Task SavePaymentIntentAsync(PaymentIntent paymentIntent);
        Task<List<PaymentHistoryDto>> GetPaymentHistory();
        Task<List<PaymentHistoryDto>> GetPaymentHistoryByUserIdAsync(int userId);
        Task<List<PaymentHistoryDto>> GetPaymentHistoryByVendorIdAsync(string vendorId);
        Task<bool> CreatePayoutToBank(string vendorStripeAccountId, decimal amount);
        Task<AccountStatusResponse> CheckAccountStatus(string vendorStripeAccountId);
    }
}
