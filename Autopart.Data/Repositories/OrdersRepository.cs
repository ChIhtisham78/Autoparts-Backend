using Autopart.Domain.CommonDTO;
using Autopart.Domain.CommonModel;
using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Autopart.Data.Repositories
{
	public class OrdersRepository : IOrdersRepository
	{
		private readonly autopartContext _context;
		public IUnitOfWork UnitOfWork => _context;

		public OrdersRepository(autopartContext context)
		{
			_context = context;
		}
		public async Task<Order> GetOrderByIdAsync(int id)
		{
			return await _context.Orders
								 .Include(o => o.OrderLines)
								 .ThenInclude(ol => ol.Product)
								 .Include(o => o.Customer)
								 .Include(o => o.Shippings)
									.ThenInclude(o => o.ShippingAddresses)
								 .Include(o => o.Billings)
									 .ThenInclude(b => b.BillingAddresses)
								 .FirstOrDefaultAsync(o => o.Id == id) ?? new Order();
		}

		public async Task<List<Category>> GetAllCategoriesAsync()
		{
			return await _context.Categories.ToListAsync();
		}

		public async Task<List<Svcrelation>> GetAllSVCRelationsAsync()
		{
			return await _context.Svcrelations.ToListAsync();
		}
		public IEnumerable<OrderLine> GetAllOrderLines()
		{
			return _context.OrderLines.AsQueryable();
		}
		public IEnumerable<Product> GetAllProductWithImages()
		{
			return _context.Products.Include(x => x.Image).AsQueryable();
		}

		public async Task<Address?> GetAddressByCustomerId(int customerId)
		{
			return await _context.Addresses
								 .FirstOrDefaultAsync(a => a.UserId == customerId);
		}

		public async Task SaveOrderAsync(OrderLine orderLine)
		{
			try
			{
				await _context.OrderLines.AddAsync(orderLine);
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				throw new Exception($"Error saving order line: {ex.Message}");
			}
		}



		public async Task<(IEnumerable<GetOrdersDTO> Orders, int TotalCount)> GetOrders(GetAllOrdersDto ordersDto)
		{
			try
			{
				var finalQuery = (from order in _context.Orders
								  join shipping in _context.Shippings
								  on order.Id equals shipping.OrderId into shippingGrouped
								  from shipping in shippingGrouped.DefaultIfEmpty()
								  join user in _context.AspNetUsers
								  on order.CustomerId equals user.Id into userGrouped
								  from user in userGrouped.DefaultIfEmpty()
								  join userProfile in _context.Profiles.Include(x => x.Image)
								  on user.Id equals userProfile.CustomerId into userProfileGrouped
								  from userProfile in userProfileGrouped.DefaultIfEmpty()
								  join address in _context.Addresses
								  on user.Id equals address.UserId into addressGrouped
								  from address in addressGrouped.DefaultIfEmpty()
								  join shippingAddress in _context.ShippingAddresses
								  on order.Id equals shippingAddress.OrderId into shippingAddressGrouped
								  from shippingAddress in shippingAddressGrouped.DefaultIfEmpty()
								  join status in _context.Statuses
								  on order.StatusId equals status.Id into statusGrouped
								  from status in statusGrouped.DefaultIfEmpty()
								  join tax in _context.Taxes
								  on shippingAddress.State equals tax.State into taxGrouped
								  from tax in taxGrouped.DefaultIfEmpty()
								  join coupon in _context.Coupons
								  on order.CouponsId equals coupon.Id into couponGrouped
								  from coupon in couponGrouped.DefaultIfEmpty()
								  join billing in _context.Billings on order.Id equals billing.OrderId into billinggrouped
								  from billing in billinggrouped.DefaultIfEmpty()
								  join billingAddress in _context.BillingAddresses on order.Id equals billingAddress.OrderId into shippingaddressgrouped
								  from billingAddress in shippingaddressgrouped.DefaultIfEmpty()
								  select new GetOrdersDTO
								  {
									  orders = order,
									  shipping = shipping,
									  billing = billing,
									  billingAddress = billingAddress,
									  user = user,
									  userProfile = userProfile,
									  address = address,
									  shippingAddress = shippingAddress,
									  status = status,
									  tax = tax,
									  coupon = coupon
								  });


				if (ordersDto.customerId.HasValue)
				{
					finalQuery = finalQuery.Where(o => o.orders.CustomerId == ordersDto.customerId);
				}

				// Filter by OrderNumber if provided
				if (ordersDto.orderNumber.HasValue)
				{
					finalQuery = finalQuery.Where(o => o.orders.Shippings.Any(s => s.TrackingNo.Contains(ordersDto.trackingNumber!)));
				}

				// Filter by search term if provided
				if (!string.IsNullOrEmpty(ordersDto.search))
				{
					finalQuery = finalQuery.Where(o => o.orders.OrderLines.Any(ol => ol.Product.Name.Contains(ordersDto.search) || ol.Product.Slug.Contains(ordersDto.search)));
				}


				var totalCount = await finalQuery.CountAsync();
				//var totalCount = 0;

				var orders = await finalQuery
					.Skip((ordersDto.pageNumber - 1) * ordersDto.pageSize)
					.Take(ordersDto.pageSize)
					.ToListAsync();

				return (orders, totalCount);
			}
			catch (Exception ex)
			{
				throw new Exception($"Error retrieving orders: {ex.Message}", ex);
			}
		}


		//public decimal GetTaxRateForUserAddress(Address userAddress)
		//{
		//	decimal taxRate = 0m;
		//	if (userAddress != null && !string.IsNullOrEmpty(userAddress.State))
		//	{
		//		var stateTax = _context.Taxes.FirstOrDefault(t => t.State == userAddress.State);
		//		if (stateTax != null && stateTax.Rate.HasValue)
		//		{
		//			taxRate = stateTax.Rate.Value;
		//		}
		//	}
		//	return taxRate;
		//}


		public async Task<List<Order>> GetPendingOrders()
		{
			return await _context.Orders.Include(o => o.Status).Where(o => o.Status.Name == "Pending").ToListAsync();
		}

		public async Task<List<Order>> GetOrderLineProducts()
		{
			var orders = await _context.Orders.Include(o => o.OrderLines).ThenInclude(ol => ol.Product).ToListAsync();

			return orders;
		}
		public async Task<Category> GetCategoryByIdAsync(int id)
		{
			return await _context.Categories.FindAsync(id) ?? new Category();
		}

		public async Task<Order?> GetOrderById(int id)
		{
			return await _context.Orders
		 .Include(o => o.OrderLines)
			 .ThenInclude(ol => ol.Product)
				 .ThenInclude(p => p.Image)
		 .FirstOrDefaultAsync(o => o.Id == id);
		}
		//public async Task<Product?> GetProductById(int? productId)
		//{
		//    return await _context.Products.Where(x=> x.Id == productId).FirstOrDefaultAsync();
		//} 
		public async Task<Product?> GetProductById(int? productId)
		{
			return await _context.Products
						 .Include(p => p.Image)
						 .FirstOrDefaultAsync(x => x.Id == productId);
		}

		public async Task<List<Order>> GetPendingOrdersByUserId(int customerId)
		{
			return await _context.Orders.Include(o => o.Status).Include(o => o.OrderLines).ThenInclude(ol => ol.Product)
				.ThenInclude(p => p.Image).Where(o => o.CustomerId == customerId && o.Status.Name == "Pending").ToListAsync();
		}

		public async Task<List<Order>> GetAllOrdersByUserId(int customerId)
		{
			return await _context.Orders.Include(o => o.Status).Include(o => o.OrderLines).ThenInclude(ol => ol.Product).ThenInclude(p => p.Image)
				.Where(o => o.CustomerId == customerId).ToListAsync();
		}


		public async Task<Shipping?> GetShippingById(int orderId)
		{
			return await _context.Shippings.Include(s => s.Order).FirstOrDefaultAsync(s => s.OrderId == orderId);
		}

		public async Task<Billing?> GetBillingById(int orderId)
		{
			return await _context.Billings.Include(s => s.Order).FirstOrDefaultAsync(s => s.OrderId == orderId);
		}
		public async Task<AspNetUser?> GetUserById(int orderId)
		{
			return await _context.AspNetUsers
								 .Include(u => u.Orders)
								 .Include(u => u.Profiles)
								 .FirstOrDefaultAsync(u => u.Orders.Any(o => o.Id == orderId));
		}


		public async Task<Shop> GetShopById(int Id)
		{
			return await _context.Shops.FirstOrDefaultAsync(s => s.Id == Id) ?? new Shop();
		}

		public async Task<Coupon?> GetCouponByOrderId(int Id)
		{
			return await _context.Coupons.Where(u => u.Id == Id).FirstOrDefaultAsync();
		}

		public async Task<Address?> GetUserAddressById(int orderId)
		{
			//var temp = _context.Orders.Include(x => x.Customer).Include(x => x.Customer.Addresses).Where(x=>x.Id == orderId).ToList();
			return await _context.Addresses.Include(a => a.User).FirstOrDefaultAsync(a => a.User.Orders.Any(o => o.Id == orderId));

		}
		public async Task<ShippingAddress?> GetShippingAddressById(int orderId)
		{
			var shippingAddress = await _context.ShippingAddresses.Where(x => x.OrderId == orderId).FirstOrDefaultAsync();

			return shippingAddress;
		}

		public async Task<BillingAddress?> GetbillingAddressById(int orderId)
		{
			var billingAddress = await _context.BillingAddresses.Where(x => x.OrderId == orderId).FirstOrDefaultAsync();

			return billingAddress;
		}

		public async Task<string> GetImageById(int? imageId)
		{
			var image = await _context.Images.FirstOrDefaultAsync(x => x.Id == imageId);
			return image!.OriginalUrl;
		}

		public void AddVendorOrderRelation(VendorOrderRelation vendorOrderRelation)
		{
			_context.VendorOrderRelations.Add(vendorOrderRelation);
		}

		public void AddOrders(Order order)
		{
			_context.Orders.Add(order);
		}

		public async Task<bool> DoesTrackingNumberExistAsync(int orderNumber)
		{
			return await _context.Orders.AnyAsync(o => o.OrderNumber == orderNumber);
		}


		public void AddImage(Image image)
		{
			_context.Images.Add(image);
		}
		public void AddProduct(Domain.Models.Product product)
		{
			_context.Products.Add(product);
		}
		public void AddOrderInOrderLine(OrderLine orderLine)
		{
			_context.OrderLines.Add(orderLine);
		}
		public void AddShippingAddress(ShippingAddress address)
		{
			_context.ShippingAddresses.Add(address);
		}
		public void AddBillingAddress(BillingAddress billingAddress)
		{
			_context.BillingAddresses.Add(billingAddress);
		}
		public void UpdateOrders(Order order)
		{
			_context.Orders.Update(order);
		}

		public async Task<Status> GetStatusByName(string orderStatus)
		{
			return await _context.Statuses
				.FirstOrDefaultAsync(s => s.Name.ToLower() == orderStatus.ToLower()) ?? new Status();
		}

		public void DeleteOrder(Order order)
		{
			_context.Orders.Remove(order);
		}
		public void DeleteOrderShippings(ShippingAddress shippingAddress)
		{
			_context.ShippingAddresses.Remove(shippingAddress);
		}
		public async Task<List<Status>> GetOrderStatues()
		{
			//var result = _context.Statuses.Select( x=> new LookupDto{ x.)
			return await _context.Statuses.ToListAsync();
		}
		public async Task<List<OrderLine>> GetOrderLines(int? orderId)
		{
			return await _context.OrderLines.Where(x => x.OrderId == orderId).ToListAsync();
		}


		public async Task<int> GetTotalOrdersCount(int? vendorId = null)
		{
			if (vendorId.HasValue)
			{
				return await (from order in _context.Orders
							  join shop in _context.Shops on order.ShopId equals shop.Id
							  where shop.OwnerId == vendorId.Value
							  select order).CountAsync();
			}
			else
			{
				return await _context.Orders.CountAsync();
			}
		}
		public async Task<int> GetTotalRefundOrdersCount(int? vendorId = null)
		{
			if (vendorId.HasValue)
			{
				return await (from order in _context.Orders
							  join shop in _context.Shops on order.ShopId equals shop.Id
							  where shop.OwnerId == vendorId.Value && order.Status.Name == "Refunded"
							  select order).CountAsync();
			}
			else
			{
				return await _context.Orders
					.Include(o => o.Status)
					.CountAsync(o => o.Status.Name == "Refunded");
			}
			//return await _context.Orders
			//	.Include(o => o.Status).CountAsync(o => o.Status.Name == "Refunded");
		}


		public async Task<IQueryable<Order>> GetOrdersQueryAsync(int? vendorId)
		{
			//return Task.FromResult(_context.Orders.AsQueryable());
			IQueryable<Order> query = _context.Orders.AsQueryable();
			if (vendorId.HasValue)
			{
				query = from order in _context.Orders
						join shop in _context.Shops on order.ShopId equals shop.Id
						where shop.OwnerId == vendorId.Value
						select order;
			}

			return await Task.FromResult(query);
		}

		public async Task<List<Order>> GetOrders(int? vendorId = null)
		{
			//return await _context.Orders.Include(o => o.Status).ToListAsync();
			IQueryable<Order> query = _context.Orders.Include(o => o.Status);

			//if (vendorId.HasValue)
			//{
			//	//query = from order in _context.Orders
			//	//        join shop in _context.Shops on order.ShopId equals shop.Id
			//	//        where shop.OwnerId == vendorId.Value
			//	//        select order;
			//	query = query.Where(order => order.Shop.OwnerId == vendorId.Value);
			//}

			return await query.ToListAsync();
		}

		public async Task<List<OrderLine>> GetOrderLine(int? vendorId = null)
		{
			//return await _context.OrderLines.Include(o => o.Total).ToListAsync();
			IQueryable<OrderLine> query = _context.OrderLines.Include(ol => ol.Order);

			if (vendorId.HasValue)
			{
				query = from orderLine in _context.OrderLines
						join order in _context.Orders on orderLine.OrderId equals order.Id
						join shop in _context.Shops on order.ShopId equals shop.Id
						where shop.OwnerId == vendorId.Value
						select orderLine;
			}

			return await query.ToListAsync();
		}

		public async Task<decimal?> GetTotalRevenue(int? vendorId = null)
		{
			if (vendorId.HasValue)
			{
				return await (from order in _context.Orders
							  join shop in _context.Shops on order.ShopId equals shop.Id
							  where shop.OwnerId == vendorId.Value
							  from orderLine in order.OrderLines
							  select (decimal?)orderLine.Total).SumAsync();
			}
			else
			{
				return await _context.Orders
					.Include(o => o.OrderLines)
					.SelectMany(o => o.OrderLines)
					.SumAsync(ol => (decimal?)ol.Total) ?? 0;
			} 
			//return await _context.Orders
			//	.Include(o => o.OrderLines)
			//	.SelectMany(o => o.OrderLines)
			//	.SumAsync(ol => (decimal?)ol.Total) ?? 0;
		}
		public async Task<decimal> GetTodayRevenue(int? vendorId = null)
		{
			var today = DateTime.UtcNow.Date;
			var startDate = DateTime.SpecifyKind(today, DateTimeKind.Unspecified);
			var endDate = DateTime.SpecifyKind(today.AddDays(1), DateTimeKind.Unspecified);

			if (vendorId.HasValue)
			{
				return await (from order in _context.Orders
							  join shop in _context.Shops on order.ShopId equals shop.Id
							  where shop.OwnerId == vendorId.Value && order.CreatedAt >= startDate && order.CreatedAt < endDate
							  from orderLine in order.OrderLines
							  select (decimal?)orderLine.Total).SumAsync() ?? 0;
			}
			else
			{
				return await _context.Orders
					.Include(o => o.OrderLines)
					.Where(o => o.CreatedAt >= startDate && o.CreatedAt < endDate)
					.SelectMany(o => o.OrderLines)
					.SumAsync(ol => (decimal?)ol.Total) ?? 0;
			}
			//	return (await _context.Orders
			//.Include(o => o.OrderLines)
			//.Where(o => o.CreatedAt >= startDate && o.CreatedAt < endDate)
			//.SelectMany(o => o.OrderLines)
			//.SumAsync(ol => (decimal?)ol.Total)) ?? 0;
		}

		public async Task<Status> GetStatusById(int statusId)
		{
			return await _context.Statuses.FirstOrDefaultAsync(s => s.Id == statusId) ?? new Status();
		}

		public async Task UpdateOrder(Order order)
		{
			_context.Orders.Update(order);

		}

		public async Task<Order> GetUpdateOrderById(int id)
		{
			return await _context.Orders.FindAsync(id) ?? new Order();
		}


		public async Task<AspNetUser?> GetOrderByCustomerId(int customerId)
		{
			return await _context.AspNetUsers
				.Include(u => u.Orders)
				.Include(u => u.Profiles)
				.FirstOrDefaultAsync(u => u.Id == customerId);
		}

	}
}
