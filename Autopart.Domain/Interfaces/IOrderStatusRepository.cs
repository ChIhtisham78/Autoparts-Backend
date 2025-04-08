using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Domain.Interfaces
{
    public interface IOrderStatusRepository :IRepository<OrderStatus>
    {
        Task<List<OrderStatus>> GetOrders();
        Task<OrderStatus?> GetOrderById(int id);
        void AddOrders(OrderStatus orderStatus);
        void UpdateOrders(OrderStatus orderStatus);
        void DeleteOrder(OrderStatus orderStatus);
    }
}
