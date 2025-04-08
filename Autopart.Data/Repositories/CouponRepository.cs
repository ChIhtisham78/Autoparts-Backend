using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Autopart.Data.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly autopartContext _context;

        public IUnitOfWork UnitOfWork => _context;

        public CouponRepository(autopartContext context)
        {
            _context = context;
        }

        public async Task<Coupon> CreateCouponAsync(Coupon coupon)
        {
            try
            {
                await _context.Coupons.AddAsync(coupon);
                await _context.SaveChangesAsync();
                return coupon;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<Coupon> GetCouponByIdAsync(int id)
        {
            return await _context.Coupons.FindAsync(id);
        }
        public async Task<Coupon> GetCouponByCodeAsync(string code)
        {
            return await _context.Coupons.FirstOrDefaultAsync(c => c.Code == code);
        }


        public async Task<List<Coupon>> GetCouponsByShopIdAsync(int shopId)
        {
            return await _context.Coupons.Where(c => c.ShopId == shopId).ToListAsync();
        }

        public async Task<IEnumerable<Coupon>> GetCouponsAsync()
        {
            return await _context.Coupons.ToListAsync();
        }

        public async Task<IEnumerable<Coupon>> GetCouponByParamAsync(string param, string language)
        {
            return await _context.Coupons.Where(x => x.Code.Contains(param) && x.Language.Contains(language)).ToListAsync();
        }

        public async Task UpdateCouponAsync(Coupon coupon)
        {
            _context.Coupons.Update(coupon);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveCouponAsync(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon != null)
            {
                _context.Coupons.Remove(coupon);
                await _context.SaveChangesAsync();
            }
        }
    }
}
