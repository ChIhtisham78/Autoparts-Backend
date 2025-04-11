using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Autopart.Domain.Enum.EnumAndConsonent;

namespace Autopart.Api.Controllers
{
	[ApiController]
	[Authorize]
	public class UserController : ControllerBase
	{

		private readonly IUserService _userService;
		private readonly ILogger<UserController> _logger;

		public UserController(IUserService userService, ILogger<UserController> logger)
		{
			_userService = userService;
			_logger = logger;
		}

		[HttpGet("api/User/users")]
		public async Task<IActionResult> GetUsers(OrderBy? orderBy = OrderBy.Ascending, SortedByUsername? sortedBy = SortedByUsername.UserName, string search = null)
		{
			try
			{
				var users = await _userService.GetUsers(orderBy, sortedBy, search);

				if (users == null || !users.Any())
				{
					return NotFound();
				}

				return Ok(users);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Success = false, Message = "An error occurred while fetching users." });
			}
		}

		[HttpGet("api/User/vendors")]
		public async Task<IActionResult> GetVendors(bool? isActive = null)
		{
			try
			{
				var vendors = await _userService.GetVendors(isActive);

				if (vendors == null || !vendors.Any())
				{
					return NotFound();
				}

				return Ok(vendors);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Success = false, Message = "An error occurred while fetching users." });
			}
		}



		[HttpGet("api/User/user/{id}")]
		public async Task<IActionResult> GetUser(int id)
		{
			try
			{
				var user = await _userService.GetUserById(id);
				if (user == null)
				{
					return NotFound(new { Success = false, Message = $"User with ID {id} not found." });
				}

				return Ok(user);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating user: {ex.Message}");

			}
		}


		[HttpPut("api/User/phonenumber/{id}")]
		public async Task<IActionResult> UpdateUserPhoneNumber(int id, [FromBody] UpdatePhoneNumberRequest request)
		{
			if (id != request.Id)
			{
				return BadRequest("User ID in the route and body do not match.");
			}

			try
			{
				var userDto = await _userService.GetUserById(id);
				if (userDto == null)
				{
					return Ok(new List<object>());
				}

				await _userService.UpdateUserPhoneNumber(request);
				return Ok("Phone number updated successfully.");
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating phone number: {ex.Message}");
			}
		}

		[HttpDelete("api/User/user/{id}")]
		public async Task<ActionResult> DeleteUser(int id)
		{
			await _userService.RemoveUser(id);
			return Ok($"User with Id = {id} deleted successfully");
		}

		[HttpPut("api/User/user/{id}")]
		public async Task<ActionResult> PutUser(int id, [FromBody] UpdateUser updateUser)
		{
			if (id != updateUser.Id)
			{
				return BadRequest("User ID in the route and body do not match.");
			}

			try
			{
				var userDto = await _userService.GetUserById(id);
				if (userDto == null)
				{
					return NotFound("User not found.");
				}

				await _userService.UpdateUser(updateUser);
				return Ok(updateUser);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating user: {ex.Message}");
			}
		}


		[HttpPut("api/User/userpassword/{id}")]
		public async Task<ActionResult> PutUserPassword(int id, UpdateUserPassword updateUserPassword)
		{
			if (id != updateUserPassword.Id)
			{
				return BadRequest("User ID in the route and body do not match.");
			}

			try
			{
				var userDto = await _userService.GetUserById(id);
				if (userDto == null)
				{
					return NotFound("User not found.");
				}

				await _userService.UpdateUserPassword(updateUserPassword);
				return Ok("Password updated successfully.");
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating user: {ex.Message}");
			}
		}



		[HttpPut("api/User/useraddress/{id}")]
		public async Task<ActionResult> PutUserAddresses(int id, [FromBody] UpdateUserAddress updateUserAddress)
		{
			if (updateUserAddress.Id != id)
			{
				return BadRequest("User ID in the route and body do not match.");
			}

			try
			{
				var userDto = await _userService.GetUserById(id);
				if (userDto == null)
				{
					return NotFound("User not found.");
				}

				await _userService.UpdateUserAddress(updateUserAddress);
				return Ok("Addresses updated successfully.");
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating user addresses: {ex.Message}");
			}
		}



		[HttpPost("api/User/user/ban/{id}")]
		public async Task<ActionResult> BanUser(int id)
		{
			await _userService.BanUser(id);
			return Ok($"User with Id = {id} banned successfully");
		}


		[HttpPost("api/User/user/active/{id}")]
		public async Task<ActionResult> ActiveUser(int id)
		{
			await _userService.ActiveUser(id);
			return Ok($"User with Id = {id} activated successfully");
		}


		[HttpPost("api/User/user/makeadmin/{id}")]
		public async Task<IActionResult> MakeAdmin(int id)
		{
			await _userService.MakeAdmin(id);
			return Ok();
		}


		[HttpGet("api/User/user/getrolebyname")]
		public async Task<IActionResult> GetRoleByName(string roleName)
		{
			var role = await _userService.GetRoleByName(roleName);
			if (role == null)
			{
				return NotFound();
			}
			return Ok(role);
		}

		[HttpGet("api/User/user/getuserswithvendorroleinactive")]
		public async Task<IActionResult> GetUsersWithVendorRoleInactive()
		{
			var users = await _userService.GetUsersWithVendorRoleInactive();
			return Ok(users);
		}

		[HttpGet("api/User/admins")]
		public async Task<ActionResult> GetAdmins()
		{
			var admins = await _userService.GetAdmins();
			return Ok(admins);
		}
	}
}
