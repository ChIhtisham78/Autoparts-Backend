using Autopart.Application.Models;

namespace Autopart.Application.Interfaces
{
    public interface IOrderStatusService
    {
        Task<List<OrderStatusDto>> GetOrders();
        Task<OrderStatusDto> GetOrderById(int id);
        Task RemoveOrder(int id);
        Task<OrderStatusResponse> AddOrder(OrderStatusResponse orderStatusResponse);
        Task<OrderStatusDto> UpdateOrder(OrderStatusDto orderStatusDto);
    }
}
