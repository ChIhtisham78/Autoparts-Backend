using Autopart.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Autopart.Api.Controllers
{
    [Route("api/Auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpGet("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var uniqueId = Guid.NewGuid();
            var authResponse = await _authService.Login(email, password, uniqueId);

            if (authResponse == null || authResponse.IsEmpty())
            {
                return Ok("User Does not exist");
            }
            return Ok(authResponse);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var result = await _authService.ChangePassword(request.Email, request.Password, request.ConfirmPassword);
                if (result)
                {
                    return Ok(new { message = "Password changed successfully." });
                }
                return BadRequest(new { message = "Failed to change password." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] UserRequest request)
        {
            try
            {
                var authResponse = await _authService.AddUser(request.addNewUserDto, request.Permission);

                return Ok(new
                {
                    authResponse.AccessToken,
                    authResponse.RefreshToken,
                    user = new
                    {
                        authResponse.User.Id,
                        authResponse.User.Email,
                        authResponse.User.UserName,
                        authResponse.User.PhoneNumber,
                        authResponse.User.Role,
                        authResponse.User.Permissions
                    }

                });
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] EmailRequest emailRequest)
        {
            if (string.IsNullOrEmpty(emailRequest?.Email))
            {
                return BadRequest("Email is required.");
            }

            var result = await _authService.ForgetPassword(emailRequest.Email);
            if (!result)
            {
                return BadRequest("Failed to send reset password email. Check if the email is correct.");
            }

            return Ok(new { message = "Reset password email sent successfully.", success = true });
        }


        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(jwtToken))
                {
                    return Ok(false);
                    //return BadRequest(new { message = "JWT token not found." });
                }

                var result = await _authService.Logout(jwtToken);
                if (!result)
                {
                    return Ok(result);
                    //return BadRequest(new { message = "Logout failed. Unable to invalidate JWT token." });
                }

                return Ok(true);
            }
            catch (Exception ex)
            {
                return Ok(false);
                //return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Logout failed", error = ex.Message });
            }
        }
    }
}
