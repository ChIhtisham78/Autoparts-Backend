using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Application.Services
{
	public class CouponService : ICouponService
	{
		private readonly ICouponRepository _couponRepository;
		private readonly ITypeAdapter _typeAdapter;

		public CouponService(ICouponRepository couponRepository, ITypeAdapter typeAdapter)
		{
			_couponRepository = couponRepository;
			_typeAdapter = typeAdapter;
		}

		public async Task<CouponDto> CreateCouponAsync(CouponDto couponDto)
		{
			try
			{
				var coupon = new Coupon
				{
					Id = couponDto.Id,
					Code = couponDto.Code,
					Language = couponDto.Language,
					Amount = couponDto.Amount,
					MinimumCartAmount = couponDto.MinimumCartAmount,
					IsActive = couponDto.IsActive,
					ShopId = couponDto.ShopId,
				};
				var createdCoupon = await _couponRepository.CreateCouponAsync(coupon);
				var createdCouponDto = _typeAdapter.Adapt<CouponDto>(createdCoupon);
				return createdCouponDto;
			}
			catch (Exception ex)
			{

				throw ex;
			}

		}

		public async Task<CouponDto> GetCouponByIdAsync(int id)
		{
			var coupon = await _couponRepository.GetCouponByIdAsync(id);
			return _typeAdapter.Adapt<CouponDto>(coupon);
		}

		public async Task<List<CouponDto>> GetCouponsByShopIdAsync(int shopId)
		{
			var coupons = await _couponRepository.GetCouponsByShopIdAsync(shopId);
			return _typeAdapter.Adapt<List<CouponDto>>(coupons);
		}

		public async Task<CouponDto> GetCouponByCodeAsync(string code, int shopId)
		{
			var coupon = await _couponRepository.GetCouponByCodeAsync(code);
			if (coupon == null)
			{
				return null;

			}
			if (coupon.ShopId != shopId)
			{
				throw new UnauthorizedAccessException("Coupon does not belong to the specified shop.");
			}

			if (coupon.IsActive)
			{
				var couponDto = new CouponDto
				{
					Id = coupon.Id,
					Code = coupon.Code,
					Language = coupon.Language,
					Amount = coupon.Amount,
					MinimumCartAmount = coupon.MinimumCartAmount,
					IsActive = coupon.IsActive,
					ShopId = coupon.ShopId,
				};

				return _typeAdapter.Adapt<CouponDto>(couponDto);
			}
			else
			{
				throw new ApplicationException("Coupon is not active.");
			}
		}





		public async Task<IEnumerable<CouponDto>> GetCouponsAsync()
		{
			var coupons = await _couponRepository.GetCouponsAsync();
			return _typeAdapter.Adapt<IEnumerable<CouponDto>>(coupons);
		}

		public async Task<IEnumerable<CouponDto>> GetCouponsByParamAsync(string param, string language)
		{
			var coupons = await _couponRepository.GetCouponByParamAsync(param, language);
			return _typeAdapter.Adapt<IEnumerable<CouponDto>>(coupons);
		}

		public async Task RemoveCouponAsync(int id)
		{
			await _couponRepository.RemoveCouponAsync(id);
		}

		public async Task UpdateCouponAsync(CouponDto couponDto)
		{
			var existingCoupon = await _couponRepository.GetCouponByIdAsync(couponDto.Id);

			if (existingCoupon == null)
			{
				throw new KeyNotFoundException("Coupon not found.");
			}

			existingCoupon.Code = couponDto.Code;
			existingCoupon.Language = couponDto.Language;
			existingCoupon.Amount = couponDto.Amount;
			existingCoupon.MinimumCartAmount = couponDto.MinimumCartAmount;
			existingCoupon.IsActive = couponDto.IsActive;

			await _couponRepository.UpdateCouponAsync(existingCoupon);
		}

		public async Task ApproveCouponAsync(int id)
		{
			var coupon = await _couponRepository.GetCouponByIdAsync(id);
			if (coupon == null)
			{
				throw new KeyNotFoundException("Coupon not found.");
			}

			coupon.IsActive = true;
			await _couponRepository.UpdateCouponAsync(coupon);
		}

		public async Task DisapproveCouponAsync(int id)
		{
			var coupon = await _couponRepository.GetCouponByIdAsync(id);
			if (coupon == null)
			{
				throw new KeyNotFoundException("Coupon not found.");
			}

			coupon.IsActive = false;
			await _couponRepository.UpdateCouponAsync(coupon);
		}
	}
}
