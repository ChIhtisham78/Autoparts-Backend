using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Domain.Interfaces
{
    public interface IStripeRepository :IRepository<PaymentHistory>
    {
       void AddPaymentIntent(PaymentHistory paymentHistory);
        Task<List<PaymentHistory>> GetPaymentHistory();
        Task<List<PaymentHistory>> GetPaymentHistoryByUserIdAsync(int userId);
        Task<List<PaymentHistory>> GetPaymentHistoryByVendorIdAsync(string vendorId);
    }
   
}
