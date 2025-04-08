using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Domain.Interfaces
{
    public interface ICouponRepository : IRepository<Coupon>
    {
        Task<Coupon> CreateCouponAsync(Coupon coupon);
        Task<Coupon> GetCouponByCodeAsync(string code);
        Task<Coupon> GetCouponByIdAsync(int id);
        Task<IEnumerable<Coupon>> GetCouponsAsync();
        Task<IEnumerable<Coupon>> GetCouponByParamAsync(string param, string language);
        Task UpdateCouponAsync(Coupon coupon);
        Task RemoveCouponAsync(int id);
        Task<List<Coupon>> GetCouponsByShopIdAsync(int shopId);
    }
}
