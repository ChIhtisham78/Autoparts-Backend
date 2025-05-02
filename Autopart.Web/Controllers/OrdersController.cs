using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Domain.CommonDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Autopart.Api.Controllers
{
	[Route("Order")]
	[ApiController]

	public class OrdersController : ControllerBase
	{
		private readonly IOrdersService _ordersService;

		public OrdersController(IOrdersService ordersService)
		{
			_ordersService = ordersService;
		}

		[HttpPost("order/verify")]
		public async Task<IActionResult> VerifyOrder([FromBody] VerifyOrderDto verifyOrderDto)
		{
			var verifyOrder = await _ordersService.VerifyOrder(verifyOrderDto);

			if (verifyOrder == null)
				return NotFound();

            return Ok(verifyOrder);
        }

		[HttpGet("orders")]
		public async Task<ActionResult> GetOrders(GetAllOrdersDto ordersDto)
		{

			var (orders, totalCount) = await _ordersService.GetOrders(ordersDto);

			var response = new
			{
				result = orders,
				total = totalCount,
				currentPage = ordersDto.pageNumber,
				count = orders.Count(),
				lastPage = (int)Math.Ceiling((double)totalCount / ordersDto.pageSize),
				firstItem = (ordersDto.pageNumber - 1) * ordersDto.pageSize + 1,
				lastItem = (ordersDto.pageNumber - 1) * ordersDto.pageSize + orders.Count(),
				perPage = ordersDto.pageSize,
			};

			return new JsonResult(response)
			{
				StatusCode = (int)HttpStatusCode.OK
			};

		}


		[HttpGet("pending-orders")]
		public async Task<ActionResult> GetPendingOrders()
		{
			var result = await _ordersService.GetPendingOrders();
			return Ok(result);
		}

		[HttpGet("{Id}")]
		public async Task<ActionResult<OrdersDto>> GetOrder(int Id)
		{
			var order = await _ordersService.GetOrder(Id);
			if (order == null)
			{
				return NotFound();
			}
			return Ok(order);
		}

		[HttpGet("pending-orders/{customerId}")]
		public async Task<ActionResult<OrdersDto>> GetPendingOrderByUserId(int customerId)
		{
			var order = await _ordersService.GetPendingOrdersByUserId(customerId);
			if (order == null)
			{
				return NotFound();
			}
			return Ok(order);
		}

		[HttpGet("all-orders/{customerId}")]
		public async Task<ActionResult<List<OrdersDto>>> GetAllOrdersByUserId(int customerId)
		{
			var orders = await _ordersService.GetAllOrdersByUserId(customerId);

			if (orders == null || !orders.Any())
			{
				return Ok(new List<OrdersDto>());
			}

			return Ok(orders);
		}


		[Authorize]
		[HttpDelete("Order")]
		public async Task<ActionResult> DeleteOrder(int id)
		{

			await _ordersService.DeleteOrder(id);

			return Ok($"Order with Id = {id} deleted successfully");
		}

		//[Authorize]
		[HttpPost("Order")]
		public async Task<ActionResult> PostOrders([FromBody] AddOrderDto addOrderDto)
		{
			var result = await _ordersService.AddOrder(addOrderDto);
			return Ok(result);
		}





		//[Authorize]
		[HttpPut("Order/{id}")]
		public async Task<ActionResult> PutOrder(UpdateOrderDto updateOrderDto, int id)
		{
			if (string.IsNullOrEmpty(updateOrderDto.OrderStatus))
			{
				return BadRequest("Order status is required.");
			}
			try
			{
				await _ordersService.UpdateOrder(updateOrderDto, id);
				return Ok(new { updated = true });
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating order: {ex.Message}");
			}
		}







	}
}
