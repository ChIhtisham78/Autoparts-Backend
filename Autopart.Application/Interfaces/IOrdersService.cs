using Autopart.Application.Models;
using Autopart.Application.Options;
using Autopart.Domain.CommonDTO;
using Autopart.Domain.CommonModel;
using Autopart.Domain.Models;

namespace Autopart.Application.Interfaces
{
	public interface IOrdersService
	{
        Task<(List<OrdersDto> Orders, int TotalCount)> GetOrders(GetAllOrdersDto ordersDto);
		Task<List<OrdersDto>> GetPendingOrders();
		Task<VerifyOrderResponse> VerifyOrder(VerifyOrderDto verifyOrderDto);

		Task UpdateOrder(UpdateOrderDto updateOrderDto, int id);
		Task<Order> GetOrderByIdAsync(int orderId);

		Task<List<OrdersDto>> GetPendingOrdersByUserId(int customerId);
		Task<List<OrdersDto>> GetAllOrdersByUserId(int customerId);
		Task<OrdersDto> GetOrder(int Id);
		Task DeleteOrder(int id);
		//Task<OrdersDto> UpdateOrders(OrdersDto ordersDto, OrdersProductResponse ordersProductResponse, ShippingsDto shippingsDto, OrderUserDto orderUserDto, AddressDto addressDto);
		Task<AddOrderResponse> AddOrder(AddOrderDto addOrderDto);
		Task<List<LookupDto>> OrderStatuesLookup();

	}
}
