using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Domain.Interfaces;

namespace Autopart.Application.Services
{
	public class AnalyticsService : IAnalyticsService
	{
		private readonly IShopRepository _shopRepository;
		private readonly IUserRepository _userRepository;
		private readonly IOrdersRepository _ordersRepository;

		public AnalyticsService(IShopRepository shopRepository, IUserRepository userRepository, IOrdersRepository ordersRepository)
		{
			_shopRepository = shopRepository;
			_userRepository = userRepository;
			_ordersRepository = ordersRepository;
		}

		public async Task<int> GetTotalShopsCount(int? vendorId = null)
		{
			return await _shopRepository.GetShopsCount(vendorId);
		}

		public async Task<int> GetTotalVendorsCount(int? vendorId = null)
		{
			return await _userRepository.GetTotalVendorsCount(vendorId);
		}

		public async Task<int> GetTotalOrdersCount(int? vendorId = null)
		{
			return await _ordersRepository.GetTotalOrdersCount(vendorId);
		}
		public async Task<int> GetTotalRefundedOrdersCount(int? vendorId = null)
		{
			return await _ordersRepository.GetTotalRefundOrdersCount(vendorId);
		}
		public async Task<decimal?> GetTotalRevenue(int? vendorId = null)
		{
			return await _ordersRepository.GetTotalRevenue(vendorId);
		}
		public async Task<Decimal> GetTodayTotalRevenue(int? vendorId = null)
		{
			return await _ordersRepository.GetTodayRevenue(vendorId);
		}

		//public async Task<List<NewlyCreatedUsers>> GetNewCustomers()
		//{

		//    var users = await _userRepository.GetNewCustomer();

		//    var userDtos = users.Select(u => new NewlyCreatedUsers
		//    {
		//        UserName = u.UserName,
		//        Email = u.Email
		//        // Map other properties as needed
		//    }).ToList();

		//    return userDtos;
		//}
		public async Task<int> GetNewCustomerCountLast5Days(int? vendorId = null)
		{
			return await _userRepository.GetNewCustomerCountLastWeek(vendorId);
		}


		public async Task<TodayTotalOrdersResponse> GetTodayTotalOrdersByStatus(int? vendorId = null)
		{
			var today = DateTime.UtcNow.Date;

			var orders = await _ordersRepository.GetOrders(vendorId);

			var totalOrdersByStatus = orders?
				.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value >= today)
				.Where(o => o.Status != null && o.Status.Name != null)
				.GroupBy(o => o.Status.Name)
				.Select(g => new
				{
					Status = g.Key,
					Count = g.Count()
				})
				.ToDictionary(x => x.Status, x => x.Count) ?? new Dictionary<string, int>();

			return new TodayTotalOrdersResponse
			{
				Pending = totalOrdersByStatus.GetValueOrDefault("order-pending", 0),
				Processing = totalOrdersByStatus.GetValueOrDefault("order-processing", 0),
				Complete = totalOrdersByStatus.GetValueOrDefault("order-completed", 0),
				Cancelled = totalOrdersByStatus.GetValueOrDefault("order-cancelled", 0),
				Refunded = totalOrdersByStatus.GetValueOrDefault("order-refunded", 0),
				Failed = totalOrdersByStatus.GetValueOrDefault("order-failed", 0),
				LocalFacility = totalOrdersByStatus.GetValueOrDefault("order-at-local-facility", 0),
				OutForDelivery = totalOrdersByStatus.GetValueOrDefault("order-out-for-delivery", 0)
			};
		}


		public async Task<WeeklyTotalOrdersByStatus> GetWeeklyTotalOrdersByStatus(int? vendorId = null)
		{
			var today = DateTime.UtcNow.Date;
			var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
			var endOfWeek = startOfWeek.AddDays(7).AddTicks(-1);

			var orders = await _ordersRepository.GetOrders(vendorId);

			var totalOrdersByStatus = orders
				.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value >= startOfWeek && o.CreatedAt.Value <= endOfWeek)
				.Where(o => o.Status != null && o.Status.Name != null)
				.GroupBy(o => o.Status.Name)
				.Select(g => new
				{
					Status = g.Key,
					Count = g.Count()
				})
				.ToDictionary(x => x.Status, x => x.Count) ?? new Dictionary<string, int>();

			return new WeeklyTotalOrdersByStatus
			{
				Pending = totalOrdersByStatus.GetValueOrDefault("order-pending", 0),
				Processing = totalOrdersByStatus.GetValueOrDefault("order-processing", 0),
				Complete = totalOrdersByStatus.GetValueOrDefault("order-completed", 0),
				Cancelled = totalOrdersByStatus.GetValueOrDefault("order-cancelled", 0),
				Refunded = totalOrdersByStatus.GetValueOrDefault("order-refunded", 0),
				Failed = totalOrdersByStatus.GetValueOrDefault("order-failed", 0),
				LocalFacility = totalOrdersByStatus.GetValueOrDefault("order-at-local-facility", 0),
				OutForDelivery = totalOrdersByStatus.GetValueOrDefault("order-out-for-delivery", 0)
			};
		}

		public async Task<MonthlyTotalOrdersResponse> GetMonthlyTotalOrdersByStatus(int? vendorId = null)
		{
			var today = DateTime.UtcNow.Date;
			var startOfMonth = new DateTime(today.Year, today.Month, 1);
			var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);


			var orders = await _ordersRepository.GetOrders(vendorId);

			var totalOrdersByStatus = orders?
				.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value >= startOfMonth && o.CreatedAt.Value <= endOfMonth)
				.Where(o => o.Status != null && o.Status.Name != null)
				.GroupBy(o => o.Status.Name)
				.Select(g => new
				{
					Status = g.Key,
					Count = g.Count()
				})
				.ToDictionary(x => x.Status, x => x.Count) ?? new Dictionary<string, int>();

			return new MonthlyTotalOrdersResponse
			{
				Pending = totalOrdersByStatus.GetValueOrDefault("order-pending", 0),
				Processing = totalOrdersByStatus.GetValueOrDefault("order-processing", 0),
				Complete = totalOrdersByStatus.GetValueOrDefault("order-completed", 0),
				Cancelled = totalOrdersByStatus.GetValueOrDefault("order-cancelled", 0),
				Refunded = totalOrdersByStatus.GetValueOrDefault("order-refunded", 0),
				Failed = totalOrdersByStatus.GetValueOrDefault("order-failed", 0),
				LocalFacility = totalOrdersByStatus.GetValueOrDefault("order-at-local-facility", 0),
				OutForDelivery = totalOrdersByStatus.GetValueOrDefault("order-out-for-delivery", 0)
			};
		}

		public async Task<YearlyTotalOrdersByStatus> GetYearlyTotalOrdersByStatus(int? vendorId = null)
		{
			var today = DateTime.UtcNow.Date;
			var startOfYear = new DateTime(today.Year, 1, 1);
			var endOfYear = new DateTime(today.Year, 12, 31, 23, 59, 59);

			var orders = await _ordersRepository.GetOrders(vendorId);

			var totalOrdersByStatus = orders?
				.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value >= startOfYear && o.CreatedAt.Value <= endOfYear)
				.Where(o => o.Status != null && o.Status.Name != null)
				.GroupBy(o => o.Status.Name)
				.Select(g => new
				{
					Status = g.Key,
					Count = g.Count()
				})
				.ToDictionary(x => x.Status, x => x.Count) ?? new Dictionary<string, int>();

			return new YearlyTotalOrdersByStatus
			{
				Pending = totalOrdersByStatus.GetValueOrDefault("order-pending", 0),
				Processing = totalOrdersByStatus.GetValueOrDefault("order-processing", 0),
				Complete = totalOrdersByStatus.GetValueOrDefault("order-completed", 0),
				Cancelled = totalOrdersByStatus.GetValueOrDefault("order-cancelled", 0),
				Refunded = totalOrdersByStatus.GetValueOrDefault("order-refunded", 0),
				Failed = totalOrdersByStatus.GetValueOrDefault("order-failed", 0),
				LocalFacility = totalOrdersByStatus.GetValueOrDefault("order-at-local-facility", 0),
				OutForDelivery = totalOrdersByStatus.GetValueOrDefault("order-out-for-delivery", 0)
			};
		}


		public async Task<List<TotalYearSaleByMonth>> GetTotalYearSaleByMonth(int? vendorId = null)
		{
			var today = DateTime.UtcNow.Date;
			var startOfYear = new DateTime(today.Year, 1, 1);
			var endOfYear = new DateTime(today.Year, 12, 31, 23, 59, 59);

			var orders = await _ordersRepository.GetOrders(vendorId);
			var monthlySales = orders
		.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value >= startOfYear && o.CreatedAt.Value <= endOfYear)
		.SelectMany(o => o.OrderLines) // Flatten the OrderLines
		.GroupBy(ol => ol.Order.CreatedAt.HasValue ? ol.Order.CreatedAt.Value.Month : 0) // Group by the Order's CreatedAt.Month
		.Select(g => new TotalYearSaleByMonth
		{
			Total = g.Sum(ol => ol.Amount ?? 0),
			Month = g.Key > 0 ? new DateTime(today.Year, g.Key, 1).ToString("MMMM") : "Unknown"
		})
		.ToList();

			for (int i = 1; i <= 12; i++)
			{
				if (!monthlySales.Any(ms => ms.Month == new DateTime(today.Year, i, 1).ToString("MMMM")))
				{
					monthlySales.Add(new TotalYearSaleByMonth
					{
						Total = 0,
						Month = new DateTime(today.Year, i, 1).ToString("MMMM")
					});
				}
			}

			// Sort the list by month
			monthlySales = monthlySales.OrderBy(ms => DateTime.ParseExact(ms.Month, "MMMM", null)).ToList();

			return monthlySales;
		}


		public async Task<AnalyticsSummaryResponse> GetAnalyticsSummary(int? vendorId = null)
		{
			if (vendorId.HasValue)
			{
				var vendorExists = await _userRepository.VendorExists(vendorId.Value);
				if (!vendorExists)
				{
					return null; // return empty response
				}
			}
			try
			{
				var totalRevenue = await GetTotalRevenue(vendorId);
				var todayRevenue = await GetTodayTotalRevenue(vendorId);
				var totalShops = await GetTotalShopsCount(vendorId);
				var totalRefunds = await GetTotalRefundedOrdersCount(vendorId);
				var totalVendors = await GetTotalVendorsCount(vendorId);
				var totalOrders = await GetTotalOrdersCount(vendorId);
				//var newlyCreatedUser = await GetNewCustomers();
				var newCustomers = await GetNewCustomerCountLast5Days(vendorId);
				var todayTotalOrdersByStatus = await GetTodayTotalOrdersByStatus(vendorId);
				var weeklyTotalOrdersByStatus = await GetWeeklyTotalOrdersByStatus(vendorId);
				var monthlyTotalOrdersByStatus = await GetMonthlyTotalOrdersByStatus(vendorId);
				var yearlyTotalOrdersByStatus = await GetYearlyTotalOrdersByStatus(vendorId);
				var totalYearlySaleByMonth = await GetTotalYearSaleByMonth(vendorId);



				return new AnalyticsSummaryResponse
				{
					TotalRevenue = totalRevenue ?? 0,
					TodayRevenue = todayRevenue,
					TotalShops = totalShops,
					TotalVendors = totalVendors,
					TotalOrders = totalOrders,
					TotalRefunds = totalRefunds,
					//NewlyCreatedUsers = newlyCreatedUser,
					NewCustomers = newCustomers,
					TodayTotalOrderByStatus = todayTotalOrdersByStatus,
					WeeklyTotalOrderByStatus = weeklyTotalOrdersByStatus,
					MonthlyTotalOrderByStatus = monthlyTotalOrdersByStatus,
					YearlyTotalOrderByStatus = yearlyTotalOrdersByStatus,
					TotalYearSaleByMonth = totalYearlySaleByMonth
				};
			}
			catch (Exception ex) { throw ex; }

		}


		//public async Task<int> GetNewCustomersLastWeek()
		//{
		//    var today = DateTime.UtcNow.Date;
		//    var startOfWeek = today.AddDays(-(int)today.DayOfWeek); // Start of this week
		//    var startDateLastWeek = startOfWeek.AddDays(-7); // Start of last week
		//    var endDateLastWeek = startOfWeek.AddTicks(-1); // End of last week (just before this week starts)

		//    return await _userRepository.GetNewCustomerCountByDateRange(startDateLastWeek, endDateLastWeek);
		//}


		//private int GetWeekOfYear(DateTime date)
		//{
		//    var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
		//    return cal.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday);
		//}
	}


}
