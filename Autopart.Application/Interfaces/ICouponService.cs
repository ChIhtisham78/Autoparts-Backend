using Autopart.Application.Models;

namespace Autopart.Application.Interfaces
{
    public interface ICouponService
    {
        Task<CouponDto> CreateCouponAsync(CouponDto couponDto);
        Task<CouponDto> GetCouponByIdAsync(int id);
        Task<IEnumerable<CouponDto>> GetCouponsAsync();
        Task<IEnumerable<CouponDto>> GetCouponsByParamAsync(string param, string language);
        Task UpdateCouponAsync(CouponDto couponDto);
        Task RemoveCouponAsync(int id);
        Task<CouponDto> GetCouponByCodeAsync(string code, int shopId);
        Task<List<CouponDto>> GetCouponsByShopIdAsync(int shopId);
        Task ApproveCouponAsync(int id);
        Task DisapproveCouponAsync(int id);
    }
}
