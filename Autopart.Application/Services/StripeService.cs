using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Autopart.Application.Services
{
	public class StripeService : IStripeService
	{
		private readonly IStripeRepository _stripeRepository;
		private readonly ITypeAdapter _typeAdapter;
		private readonly StripeSetting _stripeSetting;
		private readonly IOrdersRepository _ordersRepository;
		private readonly IShippingsRepository _shippingsRepository;
		private readonly ICategoryRepository _categoryRepository;

		private readonly IUserRepository _userRepository;
		private readonly ILogger<StripeService> _logger;
		public StripeService(ILogger<StripeService> logger, IOptions<StripeSetting> stripeSetting, IOrdersRepository ordersRepository, IUserRepository userRepository, IStripeRepository stripeRepository, ITypeAdapter typeAdapter, ICategoryRepository categoryRepository, IShippingsRepository shippingsRepository)
		{
			_stripeSetting = stripeSetting.Value;
			_ordersRepository = ordersRepository;
			_typeAdapter = typeAdapter;
			_categoryRepository = categoryRepository;
			_shippingsRepository = shippingsRepository;
			_logger = logger;
			_stripeRepository = stripeRepository;
			_userRepository = userRepository;
		}

		public async Task<Stripe.Checkout.Session> CreateCheckoutSessionAsync(int OrderId, int CouponId)
		{
			try
			{
				StripeConfiguration.ApiKey = _stripeSetting.SecretKey;
				var order = await _ordersRepository.GetOrderById(OrderId);
				if (order == null) throw new Exception("Order not found.");
				var currency = "usd";
				long orderTotal = 0;

				var lineItems = new List<SessionLineItemOptions>();
				foreach (var orderLine in order.OrderLines)
				{
					var product = await _ordersRepository.GetProductById(orderLine.ProductId);
					if (product == null) throw new Exception("Product not found.");
					var category = await _ordersRepository.GetCategoryByIdAsync(product.CategoryId ?? 0);
					decimal svcPricePerItem = 0;
					if (category != null && product.ShopId.HasValue)
					{
						var svcRelation = await _shippingsRepository
							.GetSVCRelationByShopIdAndSizeAsync(product.ShopId.Value, category.Size);
						if (svcRelation != null)
						{
							svcPricePerItem = svcRelation.Price.HasValue ? svcRelation.Price.Value : 0;
						}
					}
					long lineItemTotal = (long)((orderLine.Amount + svcPricePerItem) * orderLine.Quantity);
					orderTotal += lineItemTotal;
					var shippingAddress = await _ordersRepository.GetShippingAddressById(order.Id);
					decimal taxRate = await _shippingsRepository.GetTaxRateForUserAddressAsync(shippingAddress);
					decimal taxAmount = orderTotal * taxRate;
					long taxAmountCents = (long)taxAmount * 100;

					lineItems.Add(new SessionLineItemOptions
					{
						PriceData = new SessionLineItemPriceDataOptions
						{
							UnitAmount = (long)((orderLine.Amount + svcPricePerItem + taxAmount / orderLine.Quantity) * 100),
							Currency = currency,
							ProductData = new SessionLineItemPriceDataProductDataOptions
							{
								Name = product.Name
							},
						},
						Quantity = orderLine.Quantity,
					});
				}
				long discountAmount = 0;
				if (CouponId > 0)
				{
					var coupon = await _ordersRepository.GetCouponByOrderId(CouponId);
					if (coupon != null)
					{
						discountAmount = (long)(coupon.Amount.Value * 100);
						orderTotal -= discountAmount;
					}
				}

				decimal shippingCharges = 0m;
				var products = _ordersRepository.GetAllProductWithImages();
				long shippingChargesCents = (long)(shippingCharges * 100);
				long applicationFeeAmount = (long)(orderTotal * 0.10);
				var options = new Stripe.Checkout.SessionCreateOptions
				{
					PaymentMethodTypes = new List<string> { "card" },
					PaymentIntentData = new SessionPaymentIntentDataOptions
					{
						Metadata = new Dictionary<string, string>
				{
					{ "userId", order.CustomerId.ToString() },
					{ "orderId", OrderId.ToString() },
					{ "couponId", CouponId.ToString() }
				},
						ApplicationFeeAmount = applicationFeeAmount,
						TransferData = new SessionPaymentIntentDataTransferDataOptions
						{
							Destination = "acct_1PQiPTRriyJykoz1"
						}
					},
					LineItems = lineItems,
					Mode = "payment",
					SuccessUrl = $"https://www.easypartshub.com/orders/{OrderId}/thank-you",
					CancelUrl = "https://www.easypartshub.com",
				};
				var service = new Stripe.Checkout.SessionService();
				Stripe.Checkout.Session session = await service.CreateAsync(options);
				session.AmountTotal = orderTotal;
				return session;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating checkout session");
				throw new Exception("Error creating checkout session: " + ex.Message);
			}
		}

		public async Task<string> CreateVendorAccountAsync(int userId)
		{
			var user = await _userRepository.GetUserById(userId);
			StripeConfiguration.ApiKey = _stripeSetting.SecretKey;

			var options = new AccountCreateOptions
			{
				Type = "express",
				Country = "US",
				Email = user.Email,
				Capabilities = new AccountCapabilitiesOptions
				{
					CardPayments = new AccountCapabilitiesCardPaymentsOptions { Requested = true },
					Transfers = new AccountCapabilitiesTransfersOptions { Requested = true },
				},
				BusinessType = "individual",
			};

			var service = new Stripe.AccountService();
			var account = await service.CreateAsync(options);

			user.StripeVendorId = account.Id;
			_userRepository.UpdateUser(user);
			await _userRepository.UnitOfWork.SaveChangesAsync();

			return account.Id;
		}

		public async Task<string> GenerateAccountLinkAsync(string accountId)
		{
			StripeConfiguration.ApiKey = _stripeSetting.SecretKey;

			var options = new AccountLinkCreateOptions
			{
				Account = accountId,
				RefreshUrl = _stripeSetting.SUrl,
				ReturnUrl = "https://admin.easypartshub.com",
				Type = "account_onboarding",
			};

			var service = new AccountLinkService();
			var accountLink = await service.CreateAsync(options);

			var user = await _userRepository.GetUserByStripeVendorId(accountId);
			if (user != null)
			{
				user.StripeDashboardAccess = accountLink.Url;
				_userRepository.UpdateUser(user);
				await _userRepository.UnitOfWork.SaveChangesAsync();
			}

			return accountLink.Url;
		}



		public async Task SavePaymentIntentAsync(PaymentIntent paymentIntent)
		{
			// Create a new PaymentHistory entry
			var paymentHistory = new PaymentHistory
			{
				UserId = int.Parse(paymentIntent.Metadata["userId"]),
				VendorId = paymentIntent.Metadata["vendorId"],
				PaymentId = paymentIntent.Id,
				Status = paymentIntent.Status,
				ChargedAmount = paymentIntent.AmountReceived / 100,
				OrderId = int.Parse(paymentIntent.Metadata["orderId"])
			};

			_stripeRepository.AddPaymentIntent(paymentHistory);
			await _stripeRepository.UnitOfWork.SaveChangesAsync();

			var orderId = paymentHistory.OrderId;
			var order = await _ordersRepository.GetOrderById(orderId.Value);

			if (order != null)
			{
				if (paymentIntent.Status == "succeeded")
				{
					order.PaymentStatus = "payment-success";
				}
				else if (paymentIntent.Status == "requires_payment_method" || paymentIntent.Status == "canceled")
				{
					order.PaymentStatus = "payment-failed";
				}
				else
				{
					order.PaymentStatus = "payment-pending";
				}

				await _ordersRepository.UpdateOrder(order);
				await _ordersRepository.UnitOfWork.SaveChangesAsync();
			}
		}



		public async Task<List<PaymentHistoryDto>> GetPaymentHistory()
		{
			var payment = await _stripeRepository.GetPaymentHistory();
			var paymentHistoryDtos = _typeAdapter.Adapt<List<PaymentHistoryDto>>(payment);
			return paymentHistoryDtos;
		}
		public async Task<List<PaymentHistoryDto>> GetPaymentHistoryByUserIdAsync(int userId)
		{
			var paymentHistories = await _stripeRepository.GetPaymentHistoryByUserIdAsync(userId);
			var paymentHistoryDtos = _typeAdapter.Adapt<List<PaymentHistory>, List<PaymentHistoryDto>>(paymentHistories);
			return paymentHistoryDtos;
		}
		public async Task<List<PaymentHistoryDto>> GetPaymentHistoryByVendorIdAsync(string vendorId)
		{
			var paymentHistories = await _stripeRepository.GetPaymentHistoryByVendorIdAsync(vendorId);
			var paymentHistoryDtos = _typeAdapter.Adapt<List<PaymentHistory>, List<PaymentHistoryDto>>(paymentHistories);
			return paymentHistoryDtos;
		}
		public async Task<bool> CreatePayoutToBank(string vendorStripeAccountId, decimal amount)
		{
			StripeConfiguration.ApiKey = _stripeSetting.SecretKey;
			var options = new PayoutCreateOptions
			{
				Amount = (long)(amount), // Amount in cents
				Currency = "usd"
			};

			var requestOptions = new RequestOptions
			{
				StripeAccount = vendorStripeAccountId
			};

			var service = new PayoutService();

			try
			{
				// Create the payout
				var payout = await service.CreateAsync(options, requestOptions);

				// Check if the payout is created successfully
				if (payout != null && !string.IsNullOrEmpty(payout.Id) && payout.Status == "paid")
				{
					return true;
				}
			}
			catch (StripeException ex)
			{
				// Log the exception or handle it as needed
				Console.WriteLine($"Stripe Error: {ex.Message}");
			}

			return false;
		}


		public async Task<AccountStatusResponse> CheckAccountStatus(string vendorStripeAccountId)
		{
			StripeConfiguration.ApiKey = _stripeSetting.SecretKey;
			var service = new Stripe.AccountService();
			var account = service.Get(vendorStripeAccountId);
			var status = new AccountStatusResponse
			{
				ChargesEnabled = account.ChargesEnabled,
				PayoutsEnabled = account.PayoutsEnabled,
				CurrentlyDueRequirements = account.Requirements.CurrentlyDue
			};
			return status;
		}

		public class AccountStatusResponse
		{
			public bool ChargesEnabled { get; set; }
			public bool PayoutsEnabled { get; set; }
			public List<string> CurrentlyDueRequirements { get; set; }
		}
	}
}
