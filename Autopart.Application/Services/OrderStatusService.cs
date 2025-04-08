using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Domain.Exceptions;
using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Application.Services
{
    public class OrderStatusService : IOrderStatusService
    {
        private readonly IOrderStatusRepository _orderstatusRepository;
        private readonly ITypeAdapter _typeAdapter;
        public OrderStatusService(IOrderStatusRepository orderStatusRepository, ITypeAdapter typeAdapter)
        {
            _orderstatusRepository = orderStatusRepository;
            _typeAdapter = typeAdapter;
        }


        public async Task<List<OrderStatusDto>> GetOrders()
        {
            var orders = await _orderstatusRepository.GetOrders();
            var OrderstatusDto = _typeAdapter.Adapt<List<OrderStatusDto>>(orders);
            return OrderstatusDto;
        }

        public async Task<OrderStatusDto> GetOrderById(int id)
        {

            var order = _typeAdapter.Adapt<OrderStatusDto>(await _orderstatusRepository.GetOrderById(id));
            return order;
        }

        public async Task RemoveOrder(int id)
        {
            var order = await _orderstatusRepository.GetOrderById(id);
            if (order == null)
            {
                throw new DomainException("Order  not exists");
            }
            _orderstatusRepository.DeleteOrder(order);
            await _orderstatusRepository.UnitOfWork.SaveChangesAsync();

        }
        public async Task<OrderStatusResponse> AddOrder(OrderStatusResponse orderStatusResponse)
        {
            var order = new OrderStatus()
            {
                Name = orderStatusResponse.Name,
                //Color= orderStatusResponse.Color,
                //Serial= orderStatusResponse.Serial,
                //Language= orderStatusResponse.Language

            };


            _orderstatusRepository.AddOrders(order);
            await _orderstatusRepository.UnitOfWork.SaveChangesAsync();

            var orderstatusDto = _typeAdapter.Adapt<OrderStatusResponse>(order);

            return orderstatusDto;
        }

        public async Task<OrderStatusDto> UpdateOrder(OrderStatusDto orderStatusDto)
        {
            var order = await _orderstatusRepository.GetOrderById(orderStatusDto.Id);
            if (order == null)
            {
                throw new DomainException("Order not exists");
            }

            order.Name = orderStatusDto.Name;
            //order.Color = orderStatusDto.Color;
            //order.Serial = orderStatusDto.Serial;
            //order.Language = orderStatusDto.Language;

            _orderstatusRepository.UpdateOrders(order);
            await _orderstatusRepository.UnitOfWork.SaveChangesAsync();
            var orderStatusResponse = _typeAdapter.Adapt<OrderStatusResponse>(order);
            return orderStatusDto;
        }

    }
}
