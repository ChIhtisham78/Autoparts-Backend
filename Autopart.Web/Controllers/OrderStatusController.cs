using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZikApp.API.Utilities;

namespace Autopart.Api.Controllers
{
    [Route("OrderStatus")]
    [ApiController]
   
    public class OrderStatusController : ControllerBase
    {
        private readonly IOrderStatusService _orderStatusService;

        public OrderStatusController(IOrderStatusService orderStatusService)
        {
            _orderStatusService= orderStatusService;
        }

        [HttpGet("ordersStatus")]
        public async Task<ActionResult> GetOrdersStatus()
        {

            return Ok(await _orderStatusService.GetOrders());

        }


        [HttpGet("orderStatus")]
        public async Task<ActionResult> GetOrderStatus(int id)
        {
            return Ok(await _orderStatusService.GetOrderById(id));
        }

        [Authorize]
        [HttpDelete("orderStatus")]
        public async Task<ActionResult> DeleteOrderStatus(int id)
        {
            await _orderStatusService.RemoveOrder(id);
            return Ok($"Order-Status with Id = {id} deleted successfully");
        }

        [Authorize]
        [HttpPut("OrderStatus")]
        public async Task<ActionResult> PutOrderStatus(int id, [FromBody] OrderStatusDto orderStatusDto)
        {
            orderStatusDto.Id = id;
            if (id != orderStatusDto.Id)
            {
                return BadRequest("Order-Status ID in the route and body do not match.");
            }
            try
            {
                await _orderStatusService.UpdateOrder(orderStatusDto);
                return Ok(orderStatusDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating user: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("orderStatus")]
        public async Task<ActionResult> PostOrderStatus([FromBody] OrderStatusResponse orderStatusResponse)
        {
            var res = await _orderStatusService.AddOrder(orderStatusResponse);
            return Ok($"{res.Name} added sucessfully!");
        }
    }
}
