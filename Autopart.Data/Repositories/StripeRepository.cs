using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Data.Repositories
{
    public class StripeRepository:IStripeRepository
    {
        private readonly autopartContext _context;
        public IUnitOfWork UnitOfWork => _context;

        public StripeRepository(autopartContext context) 
        {
            _context = context;
        }


        public void AddPaymentIntent(PaymentHistory paymentHistory)
        {
            _context.PaymentHistories.Add(paymentHistory);
        }
        public async Task<List<PaymentHistory>> GetPaymentHistory()
        {
            return await _context.PaymentHistories.ToListAsync();
        }
        public async Task<List<PaymentHistory>> GetPaymentHistoryByUserIdAsync(int userId)
        {
            return await _context.PaymentHistories.Where(ph => ph.UserId == userId).ToListAsync();
        } 
        public async Task<List<PaymentHistory>> GetPaymentHistoryByVendorIdAsync(string vendorId)
        {
            return await _context.PaymentHistories.Where(ph => ph.VendorId == vendorId).ToListAsync();
        }

    }
}
