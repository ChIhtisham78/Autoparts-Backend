using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Data.Repositories
{
    public class OrderStatusRepository: IOrderStatusRepository
    {
        private readonly autopartContext _context;
        public IUnitOfWork UnitOfWork => _context;

        public OrderStatusRepository(autopartContext context)
        {
            _context = context;
        }

        public async Task<List<OrderStatus>> GetOrders()
        {
            return await _context.OrderStatuses.ToListAsync();
        }

        public async Task<OrderStatus?> GetOrderById(int id)
        {
            return await _context.OrderStatuses.FindAsync(id);
        }

        public void AddOrders(OrderStatus orderStatus)
        {
            _context.OrderStatuses.Add(orderStatus);
        }

        public void UpdateOrders(OrderStatus orderStatus)
        {
            _context.OrderStatuses.Update(orderStatus);
        }

        public void DeleteOrder(OrderStatus orderStatus)
        {
            _context.OrderStatuses.Remove(orderStatus);
        }
    }
}
