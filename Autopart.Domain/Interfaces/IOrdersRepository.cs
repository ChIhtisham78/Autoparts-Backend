using Autopart.Domain.CommonModel;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Domain.Interfaces
{
	public interface IOrdersRepository : IRepository<Order>
	{
		Task<(IEnumerable<GetOrdersDTO> Orders, int TotalCount)> GetOrders(int? customerId = null, int? orderNumber = null, string trackingNumber = null, string search = null, int pageNumber = 1, int pageSize = 10);
		Task<List<Category>> GetAllCategoriesAsync();
		Task<Category> GetCategoryByIdAsync(int id);

		Task<List<Svcrelation>> GetAllSVCRelationsAsync();
		void AddVendorOrderRelation(VendorOrderRelation vendorOrderRelation);
		Task<Order> GetOrderByIdAsync(int id);
		Task<AspNetUser?> GetOrderByCustomerId(int customerId);
		Task<Address?> GetAddressByCustomerId(int customerId);

		Task<List<Order>> GetPendingOrders();
		Task<List<Order>> GetPendingOrdersByUserId(int customerId);
		Task<List<Order>> GetAllOrdersByUserId(int customerId);
		Task SaveOrderAsync(OrderLine orderLine);
		Task<Order> GetUpdateOrderById(int id);

		Task<List<Order>> GetOrderLineProducts();

		IEnumerable<OrderLine> GetAllOrderLines();
		IEnumerable<Product> GetAllProductWithImages();

		Task<Order?> GetOrderById(int id);
		Task<Product?> GetProductById(int? id);
		Task<Shipping?> GetShippingById(int orderId);
		Task<Billing?> GetBillingById(int orderId);
		Task<AspNetUser?> GetUserById(int orderId);
		Task<Shop?> GetShopById(int Id);
		Task<Address?> GetUserAddressById(int orderId);
		Task<ShippingAddress?> GetShippingAddressById(int orderId);
		Task<BillingAddress?> GetbillingAddressById(int orderId);
		Task<Status> GetStatusByName(string orderStatus);

		Task<string> GetImageById(int? imageId);
		void AddOrders(Order order);
		Task<bool> DoesTrackingNumberExistAsync(int trackingNumber);

		void AddImage(Image image);
		void AddProduct(Product product);
		void AddOrderInOrderLine(OrderLine orderLine);
		void UpdateOrders(Order order);
		void DeleteOrder(Order order);
		void DeleteOrderShippings(ShippingAddress shippingAddress);
		Task<Coupon?> GetCouponByOrderId(int Id);
		void AddShippingAddress(ShippingAddress address);
		void AddBillingAddress(BillingAddress billingAddress);
		Task<List<Status>> GetOrderStatues();
		Task<List<OrderLine>> GetOrderLines(int? orderId);

		Task<int> GetTotalOrdersCount(int? vendorId = null);
		Task<IQueryable<Order>> GetOrdersQueryAsync(int? vendorId = null);
		Task<List<Order>> GetOrders(int? vendorId = null);
		Task<List<OrderLine>> GetOrderLine(int? vendorId = null);
		Task<int> GetTotalRefundOrdersCount(int? vendorId = null);
		Task<decimal?> GetTotalRevenue(int? vendorId = null);
		Task<decimal> GetTodayRevenue(int? vendorId = null);
		Task<Status> GetStatusById(int statusId);
		Task UpdateOrder(Order order);


	}
}
