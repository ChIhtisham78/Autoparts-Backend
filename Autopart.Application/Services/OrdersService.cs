using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Application.Models.Products;
using Autopart.Application.Options;
using Autopart.Domain.Exceptions;
using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Autopart.Application.Services
{
	public class OrdersService : IOrdersService
	{
		private readonly IShippingsRepository _shippingrepository;
		private readonly IOrdersRepository _ordersRepository;
		private readonly ICouponRepository _couponRepository;
		private readonly ITypeAdapter _typeAdapter;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly IProductRepository _productRepository;
		private readonly RootFolder _rootFolder;

		public OrdersService(IOrdersRepository ordersRepository, ICouponRepository couponRepository, ITypeAdapter typeAdapter, IOptions<RootFolder> rootFolder, IWebHostEnvironment webHostEnvironment, IShippingsRepository shippingsRepository, IProductRepository productRepository)
		{
			_ordersRepository = ordersRepository;
			_couponRepository = couponRepository;
			_shippingrepository = shippingsRepository;
			_typeAdapter = typeAdapter;
			_rootFolder = rootFolder.Value;
			_webHostEnvironment = webHostEnvironment;
			_productRepository = productRepository;
		}
		public async Task<Order> GetOrderByIdAsync(int orderId)
		{
			return await _ordersRepository.GetOrderByIdAsync(orderId);
		}
		public async Task<VerifyOrderResponse> VerifyOrder(VerifyOrderDto verifyOrderDto)
		{
			try
			{
				var shippingAddress = new ShippingAddress
				{
					Country = verifyOrderDto.ShippingAddress.Country,
					City = verifyOrderDto.ShippingAddress.City,
					State = verifyOrderDto.ShippingAddress.State,
					Zip = verifyOrderDto.ShippingAddress.Zip,
					StreetAddress = verifyOrderDto.ShippingAddress.StreetAddress
				};

				var taxRate = await _shippingrepository.GetTaxRateForUserAddressAsync(shippingAddress);

				decimal totalTax = 0;
				if (verifyOrderDto.Amount.HasValue)
				{
					decimal taxAmount = verifyOrderDto.Amount.Value * (taxRate);
					totalTax = taxAmount;
				}

				return new VerifyOrderResponse
				{
					TotalTax = totalTax,
					ShippingCharge = 0,
					UnavailableProducts = new List<string>()
				};
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred while the order.", ex);
			}
		}
		public async Task<(List<OrdersDto> Orders, int TotalCount)> GetOrders(
		int? customerId = null, int? orderNumber = null,
		string? search = null, int pageNumber = 1, int pageSize = 10)
		{
			try
			{
				var (orders, totalCount) = await _ordersRepository.GetOrders(
					customerId, orderNumber, null!, search!, pageNumber, pageSize);

				var uniqueOrders = orders.Select(x => new { x.orders, x.coupon, x.tax }).Distinct().ToList();
				var orderIds = uniqueOrders.Select(c => c.orders.Id).ToArray();

				var orderLines = _ordersRepository.GetAllOrderLines().ToList();
				var products = _ordersRepository.GetAllProductWithImages().ToList();
				var categories = await _ordersRepository.GetAllCategoriesAsync();

				var ordersLines = (from orderLine in orderLines.Where(x => orderIds.Contains(x.OrderId!.Value))
								   join product in products on orderLine.ProductId equals product.Id into productGrouped
								   from product in productGrouped.DefaultIfEmpty()
								   let category = product.CategoryId.HasValue ? categories.FirstOrDefault(c => c.Id == product.CategoryId) : null
								   let shippingCharges = category != null && product.ShopId.HasValue
									   ? _shippingrepository.GetSVCRelationByShopIdAndSizeAsync(product.ShopId.Value, category.Size).Result?.Price ?? 0
									   : 0
								   let totalPrice = (product.Price ?? 0) * (orderLine.Quantity ?? 0) + shippingCharges
								   select new OrdersProductResponse
								   {
									   OrderId = orderLine.OrderId,
									   Id = product.Id,
									   Name = product.Name,
									   Slug = product.Slug,
									   Description = product.Description,
									   Price = product.Price,
									   SalePrice = product.SalePrice,
									   Language = product.Language,
									   Sku = product.Sku,
									   MinPrice = product.MinPrice,
									   MaxPrice = product.MaxPrice,
									   InStock = product.InStock,
									   IsTaxable = product.IsTaxable,
									   ShippingClassId = product.ShippingClassId,
									   Status = product.Status,
									   ProductType = product.ProductType,
									   Unit = product.Unit,
									   Height = product.Height,
									   Width = product.Width,
									   Length = product.Length,
									   CreatedAt = product.CreatedAt,
									   UpdatedAt = product.UpdatedAt,
									   DeletedAt = product.DeletedAt,
									   IsDigital = product.IsDigital,
									   IsExternal = product.IsExternal,
									   ExternalProductUrl = product.ExternalProductUrl,
									   ExternalProductButtonText = product.ExternalProductButtonText,
									   Ratings = product.Ratings,
									   TotalReviews = product.TotalReviews,
									   MyReview = product.MyReview,
									   InWishlist = product.InWishlist,
									   ShopId = product.ShopId,
									   ManufacturerId = product.ManufacturerId,
									   AuthorId = product.AuthorId,
									   CategoryId = product.CategoryId,
									   OrderQuantity = orderLine.Quantity ?? 0,
									   Total = (orderLine.Amount ?? 0m) * (orderLine.Quantity ?? 0),
									   Amount = (orderLine.Amount ?? 0m) + shippingCharges,
									   Discount = orderLine.Discount ?? 0m,
									   DeliveryTime = orderLine.DeliveryTime,
									   DeliveryFee = shippingCharges,
									   TotalPrice = totalPrice,
									   ImageDto = new ImageDto
									   {
										   Id = product.Image?.Id ?? 0,
										   OriginalUrl = product.Image?.OriginalUrl!,
										   ThumbnailUrl = product.Image?.ThumbnailUrl!,
										   CreatedAt = product.Image?.CreatedAt,
										   UpdatedAt = product.Image?.UpdatedAt
									   }
								   }).ToList();

				var result = uniqueOrders.Select(x => new
				{
					Id = x.orders.Id,
					CustomerId = x.orders.CustomerId,
					CreatedAt = x.orders.CreatedAt,
					CouponsId = x.orders.CouponsId,
					StatusId = x.orders.StatusId,
					PaymentStatus = x.orders.PaymentStatus,
					CustomerName = x.orders.Customer?.UserName,
					CustomerContact = x.orders.Customer?.PhoneNumber,
					OrderStatus = x.orders.Status?.Name,
					Amount = ordersLines.Where(c => c.OrderId == x.orders.Id).Sum(c => c.Amount * c.OrderQuantity / c.SalesTax),
					Coupon = x.coupon,
					Discount = x.coupon == null ? 0 : (x.coupon.IsActive && ordersLines.Where(c => c.OrderId == x.orders.Id).Sum(c => c.Amount * c.OrderQuantity) >= x.coupon.MinimumCartAmount ? x.coupon.Amount : 0),
					Tax = x.tax == null ? 0 : ((x.tax.Rate ?? 0) * ordersLines.Where(c => c.OrderId == x.orders.Id).Sum(c => c.Amount * c.OrderQuantity)),

					TotalAmount = ordersLines.Where(c => c.OrderId == x.orders.Id).Sum(c => (c.Amount * c.OrderQuantity)) -
								  (x.coupon == null ? 0 : (x.coupon.IsActive && ordersLines.Where(c => c.OrderId == x.orders.Id).Sum(c => c.Amount * c.OrderQuantity) >= x.coupon.MinimumCartAmount ? x.coupon.Amount : 0)) +
								  (x.tax == null ? 0 : ((x.tax.Rate ?? 0) * ordersLines.Where(c => c.OrderId == x.orders.Id).Sum(c => c.Amount * c.OrderQuantity))),

					OrderLines = ordersLines.Where(c => c.OrderId == x.orders.Id).ToList(),
					ShippingsDto = orders.Where(c => c.shipping?.OrderId == x.orders.Id).Select(c => new ShippingsDto
					{
						Id = c.shipping.Id,
						TrackingNo = c.shipping.TrackingNo,
						OrderId = c.shipping.OrderId,
						CreatedAt = c.shipping.CreatedAt ?? DateTime.UtcNow,
						Amount = c.shipping.Amount,
						ShippingAddressDto = c.shippingAddress == null ? null : new ShippingAddressDto
						{
							Id = c.shippingAddress.Id,
							StreetAddress = c.shippingAddress.StreetAddress,
							City = c.shippingAddress.City,
							State = c.shippingAddress.State,
							Country = c.shippingAddress.Country,
							Zip = c.shippingAddress.Zip
						}
					}).FirstOrDefault(),
					BillingsDto = orders.Where(c => c.billing?.OrderId == x.orders.Id).Select(c => new BillingsDto
					{
						Id = c.billing.Id,
						TrackingNo = c.billing.TrackingNo,
						OrderId = c.billing.OrderId,
						CreatedAt = c.billing.CreatedAt ?? DateTime.UtcNow,
						Amount = c.billing.Amount,
						BillingsAddressDto = c.billingAddress == null ? null : new BillingsAddressDto
						{
							Id = c.billingAddress.Id,
							StreetAddress = c.billingAddress.StreetAddress,
							City = c.billingAddress.City,
							State = c.billingAddress.State,
							Country = c.billingAddress.Country,
							Zip = c.billingAddress.Zip
						}
					}).FirstOrDefault(),
					CustomerDto = orders.Where(c => c.user.Id == x.orders.CustomerId).Select(c => new CustomerDto
					{
						Id = c.user.Id,
						UserName = c.user.UserName,
						Email = c.user.Email,
						PhoneNumber = c.user.PhoneNumber,
						IsActive = c.user.IsActive,
						AddressDto = c.address == null ? null : new AddressDto
						{
							Id = c.address.Id,
							StreetAddress = c.address.StreetAddress,
							City = c.address.City,
							State = c.address.State,
							Country = c.address.Country,
							Zip = c.address.Zip,
							UserId = c.address.UserId
						},
						ImageDto = c.userProfile?.Image == null ? null : new ImageDto
						{
							Id = c.userProfile.Image.Id,
							OriginalUrl = c.userProfile.Image.OriginalUrl,
							ThumbnailUrl = c.userProfile.Image.ThumbnailUrl,
							CreatedAt = c.userProfile.Image.CreatedAt,
							UpdatedAt = c.userProfile.Image.UpdatedAt
						}
					}).FirstOrDefault(),
				})
				.Select(x => new OrdersDto
				{
					Id = x.Id,
					CustomerId = x.CustomerId,
					CreatedAt = x.CreatedAt,
					CouponsId = x.CouponsId,
					StatusId = x.StatusId,
					PaymentStatus = x.PaymentStatus,
					CustomerName = x.CustomerName,
					CustomerContact = x.CustomerContact,
					OrderStatus = x.OrderStatus,
					Amount = x.Amount,
					Discount = x.Discount,
					Tax = x.Tax,
					TotalAmount = x.TotalAmount,
					OrderLines = x.OrderLines,
					ShippingsDto = x.ShippingsDto,
					BillingsDto = x.BillingsDto,
					CustomerDto = x.CustomerDto,
				}).ToList();

				return (result, totalCount);
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred while retrieving orders. Details: " + ex.Message, ex);
			}
		}

		public async Task<OrdersDto> GetOrder(int id)
		{
			try
			{
				var order = await _ordersRepository.GetOrderById(id);
				if (order == null)
				{
					return null!;
				}

				var shipping = await _ordersRepository.GetShippingById(order.Id);
				var billing = await _ordersRepository.GetBillingById(order.Id);
				var user = await _ordersRepository.GetUserById(order.CustomerId ?? 0);
				var userAddress = await _ordersRepository.GetUserAddressById(order.CustomerId ?? 0);
				var shippingAddress = await _ordersRepository.GetShippingAddressById(order.Id);
				var billingAddress = await _ordersRepository.GetbillingAddressById(order.Id);
				var status = await _ordersRepository.GetStatusById(order.StatusId ?? 0);
				var orderLines = await _ordersRepository.GetOrderLines(order.Id);

				decimal amount = orderLines.Sum(ol => (ol.Amount ?? 0m) * (ol.Quantity ?? 0));
				decimal taxRate = await _shippingrepository.GetTaxRateForUserAddressAsync(shippingAddress!);
				decimal taxAmount = amount * taxRate;

				decimal discountAmount = 0m;
				if (order.CouponsId.HasValue)
				{
					var coupon = await _couponRepository.GetCouponByIdAsync(order.CouponsId.Value);
					if (coupon != null && coupon.IsActive && amount >= coupon.MinimumCartAmount.GetValueOrDefault())
					{
						discountAmount = coupon.Amount.GetValueOrDefault();
					}
				}

				decimal shippingCharges = 0m;
				var products = _ordersRepository.GetAllProductWithImages();
				foreach (var orderLine in orderLines)
				{
					var product = products.FirstOrDefault(p => p.Id == orderLine.ProductId);
					if (product != null)
					{
						var category = await _ordersRepository.GetCategoryByIdAsync(product.CategoryId ?? 0);
						if (category != null && product.ShopId.HasValue)
						{
							var svcRelation = await _shippingrepository.GetSVCRelationByShopIdAndSizeAsync(product.ShopId.Value, category.Size);
							if (svcRelation != null)
							{
								shippingCharges += (svcRelation.Price ?? 0) * (orderLine.Quantity ?? 0);
							}
						}
					}
				}

				decimal totalAmount = amount - discountAmount + taxAmount + shippingCharges;

				var orderDto = new OrdersDto
				{
					Id = order.Id,
					CustomerId = order.CustomerId,
					CustomerName = user?.UserName,
					CustomerContact = user?.PhoneNumber,
					CreatedAt = order.CreatedAt,
					CouponsId = order.CouponsId,
					StatusId = order.StatusId,
					OrderStatus = status?.Name,
					PaymentStatus = order.PaymentStatus,
					Amount = amount + shippingCharges,
					Discount = discountAmount,
					TotalAmount = totalAmount,
					Tax = taxAmount,
					PaymentGateway = "stripe",
					OrderNumber = order.OrderNumber,
					OrderLines = orderLines.Select(orderLine => new OrdersProductResponse
					{
						Id = orderLine.ProductId ?? 0,
						Name = orderLine.Product?.Name!,
						Slug = orderLine.Product?.Slug!,
						Description = orderLine.Product?.Description!,
						Price = orderLine.Product?.Price,
						SalePrice = orderLine.Product?.SalePrice,
						OrderQuantity = orderLine.Quantity ?? 0,
						Amount = orderLine.Amount ?? 0m + shippingCharges,
						Discount = orderLine.Discount ?? 0m,
						DeliveryTime = orderLine.DeliveryTime,
						DeliveryFee = orderLine.DeliveryFee ?? 0,
						ImageDto = orderLine.Product?.Image != null ? new ImageDto
						{
							Id = orderLine.Product.Image.Id,
							OriginalUrl = orderLine.Product.Image.OriginalUrl,
							ThumbnailUrl = orderLine.Product.Image.ThumbnailUrl,
							CreatedAt = orderLine.Product.Image.CreatedAt,
							UpdatedAt = orderLine.Product.Image.UpdatedAt
						} : null ?? new ImageDto()
					}).ToList()
				};

				if (shipping != null)
				{
					orderDto.ShippingsDto = new ShippingsDto
					{
						Id = shipping.Id,
						TrackingNo = shipping.TrackingNo,
						OrderId = shipping.OrderId,
						CreatedAt = shipping.CreatedAt ?? DateTime.UtcNow,
						Amount = shipping.Amount,
						ShippingAddressDto = shippingAddress != null ? new ShippingAddressDto
						{
							Id = shippingAddress.Id,
							StreetAddress = shippingAddress.StreetAddress,
							City = shippingAddress.City,
							State = shippingAddress.State,
							Country = shippingAddress.Country,
							Zip = shippingAddress.Zip
						} : null
					};
				}

				if (billing != null)
				{
					orderDto.BillingsDto = new BillingsDto
					{
						Id = billing.Id,
						TrackingNo = billing.TrackingNo,
						OrderId = billing.OrderId,
						CreatedAt = billing.CreatedAt ?? DateTime.UtcNow,
						Amount = billing.Amount,
						BillingsAddressDto = billingAddress != null ? new BillingsAddressDto
						{
							Id = billingAddress.Id,
							StreetAddress = billingAddress.StreetAddress,
							City = billingAddress.City,
							State = billingAddress.State,
							Country = billingAddress.Country,
							Zip = billingAddress.Zip
						} : null
					};
				}


				if (user != null)
				{
					orderDto.CustomerDto = new CustomerDto
					{
						Id = user.Id,
						UserName = user.UserName,
						Email = user.Email,
						PhoneNumber = user.PhoneNumber,
						IsActive = user.IsActive,
						AddressDto = userAddress != null ? new AddressDto
						{
							Id = userAddress.Id,
							StreetAddress = userAddress.StreetAddress,
							City = userAddress.City,
							State = userAddress.State,
							Country = userAddress.Country,
							Zip = userAddress.Zip,
							UserId = userAddress.UserId
						} : null,
						ImageDto = user.Profiles?.FirstOrDefault()?.Image != null ? new ImageDto
						{
							Id = user.Profiles.FirstOrDefault()!.Image.Id,
							OriginalUrl = user.Profiles.FirstOrDefault()!.Image.OriginalUrl,
							ThumbnailUrl = user.Profiles.FirstOrDefault()!.Image.ThumbnailUrl,
							CreatedAt = user.Profiles.FirstOrDefault()!.Image.CreatedAt,
							UpdatedAt = user.Profiles.FirstOrDefault()!.Image.UpdatedAt
						} : null
					};
				}

				return orderDto;
			}
			catch (Exception exp)
			{
				throw new Exception("An error occurred while retrieving the order. Details: " + exp.Message, exp);
			}
		}

		public async Task<List<OrdersDto>> GetPendingOrders()
		{
			try
			{
				var orders = await _ordersRepository.GetPendingOrders();
				var res = new List<OrdersDto>();
				if (orders == null)
				{
					return null!;
				}

				foreach (var order in orders)
				{

					var shipping = await _ordersRepository.GetShippingById(order.Id);
					var user = await _ordersRepository.GetUserById(order.Id);
					var userAddress = await _ordersRepository.GetUserAddressById(order.Id);
					var shippingAddress = await _ordersRepository.GetShippingAddressById(order.Id);
					var statusId = order.StatusId ?? 0;
					var status = await _ordersRepository.GetStatusById(statusId);

					var orderDto = new OrdersDto
					{
						Id = order.Id,
						CustomerId = order.CustomerId,
						CustomerName = user!.UserName,
						CustomerContact = user.PhoneNumber,
						CreatedAt = order.CreatedAt,
						CouponsId = order.CouponsId,
						StatusId = order.StatusId,
						OrderStatus = status?.Name,
						PaymentStatus = order.PaymentStatus
					};

					var productDtos = new List<OrdersProductResponse>();

					var orderLines = await _ordersRepository.GetOrderLines(order.Id);

					foreach (var orderLine in orderLines)
					{
						var product = await _ordersRepository.GetProductById(orderLine.ProductId);

						if (product != null)
						{
							orderDto.OrdersProductResponse = new OrdersProductResponse
							{
								Id = product.Id,
								Name = product.Name,
								Slug = product.Slug,
								Description = product.Description,
								Price = product.Price,
								SalePrice = product.SalePrice,
								Language = product.Language,
								Sku = product.Sku,
								MinPrice = product.MinPrice,
								MaxPrice = product.MaxPrice,
								InStock = product.InStock,
								IsTaxable = product.IsTaxable,
								ShippingClassId = product.ShippingClassId,
								Status = product.Status,
								ProductType = product.ProductType,
								Unit = product.Unit,
								Height = product.Height,
								Width = product.Width,
								Length = product.Length,
								CreatedAt = product.CreatedAt,
								UpdatedAt = product.UpdatedAt,
								DeletedAt = product.DeletedAt,
								IsDigital = product.IsDigital,
								IsExternal = product.IsExternal,
								ExternalProductUrl = product.ExternalProductUrl,
								ExternalProductButtonText = product.ExternalProductButtonText,
								Ratings = product.Ratings,
								TotalReviews = product.TotalReviews,
								MyReview = product.MyReview,
								InWishlist = product.InWishlist,
								ShopId = product.ShopId,
								ManufacturerId = product.ManufacturerId,
								AuthorId = product.AuthorId,
								CategoryId = product.CategoryId,


								OrderQuantity = orderLine.Quantity ?? 0,
								Total = orderLine.Total ?? 0m,
								Amount = orderLine.Amount ?? 0m,
								Discount = orderLine.Discount ?? 0m,
								DeliveryTime = orderLine.DeliveryTime,
								DeliveryFee = orderLine.DeliveryFee ?? 0,
							};


							var productImage = product.Image;
							if (productImage != null)
							{
								orderDto.OrdersProductResponse.ImageDto = new ImageDto
								{
									Id = productImage.Id,
									OriginalUrl = productImage.OriginalUrl,
									ThumbnailUrl = productImage.ThumbnailUrl,
									CreatedAt = productImage.CreatedAt,
									UpdatedAt = productImage.UpdatedAt
								};
							}
						}


						productDtos.Add(orderDto.OrdersProductResponse);
					}


					if (shipping != null)
					{
						orderDto.ShippingsDto = new ShippingsDto
						{
							Id = shipping.Id,
							TrackingNo = "123456789",
							OrderId = shipping.OrderId,
							CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
						};
					}

					if (shippingAddress != null)
					{
						if (orderDto.ShippingsDto == null)
						{
							orderDto.ShippingsDto = new ShippingsDto();
						}

						orderDto.ShippingsDto.ShippingAddressDto = new ShippingAddressDto
						{
							Id = shippingAddress.Id,
							StreetAddress = shippingAddress.StreetAddress,
							City = shippingAddress.City,
							State = shippingAddress.State,
							Country = shippingAddress.Country,
							OrderId = shippingAddress.OrderId,
							Zip = shippingAddress.Zip
						};
					}


					if (user != null)
					{
						var firstProfile = user.Profiles?.FirstOrDefault();
						orderDto.CustomerDto = new CustomerDto
						{
							Id = user.Id,
							UserName = user.UserName,
							Email = user.Email,
							PhoneNumber = user.PhoneNumber,
							IsActive = user.IsActive,
						};
						var profileImage = firstProfile?.Image;
						if (profileImage != null)
						{
							orderDto.CustomerDto.ImageDto = new ImageDto
							{
								Id = profileImage.Id,
								OriginalUrl = profileImage.OriginalUrl,
								ThumbnailUrl = profileImage.ThumbnailUrl,
								CreatedAt = profileImage.CreatedAt,
								UpdatedAt = profileImage.UpdatedAt
							};
						}
						//if (user != null)
						//{
						//	orderDto.OrderUserDto = new OrderUserDto
						//	{
						//		Id = user.Id,
						//		UserName = user.UserName,
						//		Email = user.Email,
						//		EmailConfirmed = user.EmailConfirmed,
						//		PhoneNumber = user.PhoneNumber,
						//		IsActive = user.IsActive,
						//		ShopId = user.Id,
						//		CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
						//	};

						if (userAddress != null)
						{
							orderDto.CustomerDto.AddressDto = new AddressDto
							{
								Id = userAddress.Id,
								StreetAddress = userAddress.StreetAddress,
								City = userAddress.City,
								State = userAddress.State,
								Country = userAddress.Country,
								Zip = userAddress.Zip,
								UserId = userAddress.UserId
								// Add other properties as needed
							};
						}
					}


					res.Add(orderDto);
				}
				var adaptedOrderDto = _typeAdapter.Adapt<List<OrdersDto>>(res);

				return adaptedOrderDto;
			}
			catch (Exception)
			{

				throw;
			}

		}





		public async Task<List<OrdersDto>> GetPendingOrdersByUserId(int customerId)
		{
			var orders = await _ordersRepository.GetPendingOrdersByUserId(customerId);

			var orderDtos = new List<OrdersDto>();

			foreach (var order in orders)
			{
				var orderDto = new OrdersDto
				{
					Id = order.Id,
					CustomerId = order.CustomerId,
					CreatedAt = order.CreatedAt,
					CouponsId = order.CouponsId,
					StatusId = order.StatusId,
					OrderLines = order.OrderLines.Select(ol => new OrdersProductResponse
					{
						Id = ol.Product.Id,
						Name = ol.Product.Name,
						Slug = ol.Product.Slug,
						Description = ol.Product.Description,
						Price = ol.Product.Price,
						SalePrice = ol.Product.SalePrice,
						Language = ol.Product.Language,
						Sku = ol.Product.Sku,
						MinPrice = ol.Product.MinPrice,
						MaxPrice = ol.Product.MaxPrice,
						InStock = ol.Product.InStock,
						IsTaxable = ol.Product.IsTaxable,
						ShippingClassId = ol.Product.ShippingClassId,
						Status = ol.Product.Status,
						ProductType = ol.Product.ProductType,
						Unit = ol.Product.Unit,
						Height = ol.Product.Height,
						Width = ol.Product.Width,
						Length = ol.Product.Length,
						CreatedAt = ol.Product.CreatedAt,
						UpdatedAt = ol.Product.UpdatedAt,
						DeletedAt = ol.Product.DeletedAt,
						IsDigital = ol.Product.IsDigital,
						IsExternal = ol.Product.IsExternal,
						ExternalProductUrl = ol.Product.ExternalProductUrl,
						ExternalProductButtonText = ol.Product.ExternalProductButtonText,
						Ratings = ol.Product.Ratings,
						TotalReviews = ol.Product.TotalReviews,
						MyReview = ol.Product.MyReview,
						InWishlist = ol.Product.InWishlist,
						ShopId = ol.Product.ShopId,
						ManufacturerId = ol.Product.ManufacturerId,
						AuthorId = ol.Product.AuthorId,
						CategoryId = ol.Product.CategoryId,



						ImageDto = ol.Product.Image != null ? new ImageDto
						{
							Id = ol.Product.Image.Id,
							OriginalUrl = ol.Product.Image.OriginalUrl,
							ThumbnailUrl = ol.Product.Image.ThumbnailUrl,
							CreatedAt = ol.Product.Image.CreatedAt,
							UpdatedAt = ol.Product.Image.UpdatedAt
						} : null ?? new ImageDto()
					}).ToList()
				};

				orderDtos.Add(orderDto);
			}

			return orderDtos;
		}



		public async Task<List<OrdersDto>> GetAllOrdersByUserId(int customerId)
		{
			var orders = await _ordersRepository.GetAllOrdersByUserId(customerId);

			if (orders == null || !orders.Any())
			{
				return new List<OrdersDto>();
			}

			var orderDtos = new List<OrdersDto>();

			foreach (var order in orders)
			{
				var orderDto = new OrdersDto
				{
					Id = order.Id,
					CustomerId = order.CustomerId,
					CreatedAt = order.CreatedAt,
					CouponsId = order.CouponsId,
					StatusId = order.StatusId,
					OrderNumber = order.OrderNumber,
					//PaymentGateway = orderDto.PaymentGateway,
					OrderLines = order.OrderLines.Select(ol => new OrdersProductResponse
					{
						Id = ol.Product.Id,
						Name = ol.Product.Name,
						Slug = ol.Product.Slug,
						Description = ol.Product.Description,
						Price = ol.Product.Price,
						SalePrice = ol.Product.SalePrice,
						Language = ol.Product.Language,
						Sku = ol.Product.Sku,
						MinPrice = ol.Product.MinPrice,
						MaxPrice = ol.Product.MaxPrice,
						SalesTax = ol.SalesTax,
						Discount = ol.Discount,
						Total = ol.Total,
						Amount = ol.Amount, // Map Amount field properly
						InStock = ol.Product.InStock,
						IsTaxable = ol.Product.IsTaxable,
						ShippingClassId = ol.Product.ShippingClassId,
						Status = ol.Product.Status,
						ProductType = ol.Product.ProductType,
						Unit = ol.Product.Unit,
						Height = ol.Product.Height,
						Width = ol.Product.Width,
						Length = ol.Product.Length,
						CreatedAt = ol.Product.CreatedAt,
						Quantity = ol.Quantity,
						UpdatedAt = ol.Product.UpdatedAt,
						DeletedAt = ol.Product.DeletedAt,
						IsDigital = ol.Product.IsDigital,
						IsExternal = ol.Product.IsExternal,
						ExternalProductUrl = ol.Product.ExternalProductUrl,
						ExternalProductButtonText = ol.Product.ExternalProductButtonText,
						Ratings = ol.Product.Ratings,
						TotalReviews = ol.Product.TotalReviews,
						MyReview = ol.Product.MyReview,
						InWishlist = ol.Product.InWishlist,
						ShopId = ol.Product.ShopId,
						ManufacturerId = ol.Product.ManufacturerId,
						AuthorId = ol.Product.AuthorId,
						CategoryId = ol.Product.CategoryId,
						DeliveryFee = ol.DeliveryFee, // Map DeliveryFee if needed
						TrackingNo = "12345", // Map TrackingNo properly
						DeliveryTime = ol.DeliveryTime, // Map DeliveryTime if needed

						ImageDto = ol.Product.Image != null ? new ImageDto
						{
							Id = ol.Product.Image.Id,
							OriginalUrl = ol.Product.Image.OriginalUrl,
							ThumbnailUrl = ol.Product.Image.ThumbnailUrl,
							CreatedAt = ol.Product.Image.CreatedAt,
							UpdatedAt = ol.Product.Image.UpdatedAt
						} : null ?? new ImageDto()
					}).ToList()
				};

				orderDtos.Add(orderDto);
			}

			return orderDtos;
		}






		public async Task DeleteOrder(int id)
		{
			var order = await _ordersRepository.GetOrderById(id);
			if (order == null)
			{
				throw new DomainException("Order  not exists");
			}
			var shippingAddress = await _ordersRepository.GetShippingAddressById(id);
			if (shippingAddress != null)
			{
				_ordersRepository.DeleteOrderShippings(shippingAddress);
			}

			_ordersRepository.DeleteOrder(order);
			await _ordersRepository.UnitOfWork.SaveChangesAsync();
		}


		public async Task<AddOrderResponse> AddOrder(AddOrderDto addOrderDto)
		{
			try
			{
				int orderNumber;
				do
				{
					orderNumber = GenerateUniqueOrderNumber();
				}
				while (await _ordersRepository.DoesTrackingNumberExistAsync(orderNumber));

				var order = new Order
				{
					CouponsId = addOrderDto.CouponsId.GetValueOrDefault(),
					OrderStatus = addOrderDto.OrderStatus.GetValueOrDefault(),
					StatusId = addOrderDto.StatusId.GetValueOrDefault(),
					CustomerId = addOrderDto.CustomerId,
					CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
					OrderNumber = orderNumber,
				};

				_ordersRepository.AddOrders(order);
				await _ordersRepository.UnitOfWork.SaveChangesAsync();

				decimal orderTotal = 0;
				decimal discountAmount = 0;

				foreach (var product in addOrderDto.OrderLineForOrdersAdditionDto)
				{
					var products = await _ordersRepository.GetProductById(product.ProductId);
					if (products != null)
					{
						decimal salePrice = products.SalePrice.GetValueOrDefault(0m);
						int quantity = product.Quantity.GetValueOrDefault(0);
						orderTotal += salePrice * quantity;
					}
				}

				if (addOrderDto.CouponsId.HasValue)
				{
					var coupon = await _couponRepository.GetCouponByIdAsync(addOrderDto.CouponsId.Value);
					if (coupon != null && coupon.IsActive)
					{
						if (orderTotal >= coupon.MinimumCartAmount.GetValueOrDefault())
						{
							if (coupon.Amount.HasValue)
							{
								discountAmount = coupon.Amount.GetValueOrDefault();
							}
						}
					}
				}

				foreach (var product in addOrderDto.OrderLineForOrdersAdditionDto)
				{
					var products = await _ordersRepository.GetProductById(product.ProductId);
					if (products != null)
					{
						decimal Price = products.Price.GetValueOrDefault(0m);
						int quantity = product.Quantity.GetValueOrDefault(0);

						decimal lineDiscount = discountAmount / addOrderDto.OrderLineForOrdersAdditionDto.Count;
						decimal totalAmountAfterDiscount = (Price * quantity) - lineDiscount;

						var orderLine = new OrderLine
						{
							ProductId = product.ProductId,
							OrderId = order.Id,
							Quantity = product.Quantity,
							Amount = Price,
							Discount = lineDiscount,
							Total = totalAmountAfterDiscount,
							SalesTax = 6,
							DeliveryTime = product.DeliveryTime,
							Language = "Eng"
						};

						_ordersRepository.AddOrderInOrderLine(orderLine);
					}
				}

				await _ordersRepository.UnitOfWork.SaveChangesAsync();

				var orderLines = await _ordersRepository.GetOrderLines(order.Id);
				var totalAmount = orderLines.Sum(ol => ol.Amount.GetValueOrDefault() * ol.Quantity);
				var totalDiscount = orderLines.Sum(ol => ol.Discount.GetValueOrDefault());

				var finalAmount = totalAmount - totalDiscount;

				var shippingAddress = new ShippingAddress
				{
					OrderId = order.Id,
					Zip = addOrderDto.ShippingAddressDto.Zip,
					City = addOrderDto.ShippingAddressDto.City,
					Title = addOrderDto.ShippingAddressDto.Title,
					Type = addOrderDto.ShippingAddressDto.Type,
					IsDefault = addOrderDto.ShippingAddressDto.IsDefault,
					State = addOrderDto.ShippingAddressDto.State,
					Country = addOrderDto.ShippingAddressDto.Country,
					StreetAddress = addOrderDto.ShippingAddressDto.StreetAddress,
				};

				_ordersRepository.AddShippingAddress(shippingAddress);
				await _ordersRepository.UnitOfWork.SaveChangesAsync();

				var billingAddress = new BillingAddress
				{
					OrderId = order.Id,
					Zip = addOrderDto.BillingsAddressDto.Zip,
					City = addOrderDto.BillingsAddressDto.City,
					Title = addOrderDto.BillingsAddressDto.Title,
					IsDefault = addOrderDto.BillingsAddressDto.IsDefault,
					Type = addOrderDto.BillingsAddressDto.Type,
					State = addOrderDto.BillingsAddressDto.State,
					Country = addOrderDto.BillingsAddressDto.Country,
					StreetAddress = addOrderDto.BillingsAddressDto.StreetAddress,
				};

				_ordersRepository.AddBillingAddress(billingAddress);
				await _ordersRepository.UnitOfWork.SaveChangesAsync();

				var vendorOrderRelation = new VendorOrderRelation
				{
					VendorId = order.CustomerId,
					OrderId = order.Id
				};
				_ordersRepository.AddVendorOrderRelation(vendorOrderRelation);
				await _ordersRepository.UnitOfWork.SaveChangesAsync();

				var customer = await _ordersRepository.GetUserById(order.CustomerId.GetValueOrDefault());
				var shop = await _ordersRepository.GetShopById(order.ShopId.GetValueOrDefault());

				var data = new List<ProductDto>();


				var addOrderResponse = new AddOrderResponse()
				{
					Id = order.Id,
					OrderNumber = orderNumber,
					CustomerId = order.CustomerId,
					CustomerContact = customer!.PhoneNumber,
					CustomerName = customer.UserName,
					CouponId = order.CouponsId,
					Amount = order.OrderLines.Sum(ol => ol.Amount),
					SalesTax = 6,
					PaidTotal = 0,
					Total = finalAmount,
					Note = addOrderDto.Note,
					Language = addOrderDto.Language,
					Discount = discountAmount,
					PaymentGateway = "stripe",
					ShippingAddress = new ShippingAddressResponse
					{
						Zip = shippingAddress.Zip,
						City = shippingAddress.City,
						State = shippingAddress.State,
						Country = shippingAddress.Country,
						StreetAddress = shippingAddress.StreetAddress
					},
					BillingAddress = new ShippingAddressResponse
					{
						Zip = shippingAddress.Zip,
						City = shippingAddress.City,
						State = shippingAddress.State,
						Country = shippingAddress.Country,
						StreetAddress = shippingAddress.StreetAddress
					},
					Products = data,
					PaymentIntent = null
				};

				return addOrderResponse;
			}
			catch (Exception)
			{
				throw;
			}
		}

		private int GenerateUniqueOrderNumber()
		{
			Random random = new Random();
			return random.Next(100000, 999999);
		}

		public async Task<string> UploadImage(IFormFile image)
		{
			var uploadsFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
			if (!Directory.Exists(uploadsFolderPath))
			{
				Directory.CreateDirectory(uploadsFolderPath);
			}

			var uniqueFileName = $"{Guid.NewGuid()}_{image.FileName}";
			var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);
			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await image.CopyToAsync(stream);
			}
			return $"uploads/{uniqueFileName}";
		}
		public async Task<List<LookupDto>> OrderStatuesLookup()
		{
			var orderstatues = await _ordersRepository.GetOrderStatues();
			var lookupDto = orderstatues.Select(item => new LookupDto
			{
				Id = item.Id,
				Name = item.Name
			}).ToList();

			return lookupDto;
		}

		public async Task UpdateOrder(UpdateOrderDto updateOrderDto, int id)
		{
			var order = await _ordersRepository.GetUpdateOrderById(id);
			if (order == null)
			{
				throw new ArgumentException("Order not found");
			}

			if (string.IsNullOrEmpty(updateOrderDto.OrderStatus))
			{
				throw new ArgumentException("Order status is required");
			}

			// Fetch the status ID based on the order status string
			var status = await _ordersRepository.GetStatusByName(updateOrderDto.OrderStatus);
			if (status == null)
			{
				throw new ArgumentException("Invalid order status");
			}

			order.StatusId = status.Id;

			_ordersRepository.UpdateOrders(order);
			await _ordersRepository.UnitOfWork.SaveChangesAsync();
		}
	}


}
