using Autopart.Application.Models;

public interface IAuthService
{
    Task<AuthResponse> Login(string email, string password, Guid sessionId);
    Task<bool> Logout(string jwtToken);
    Task<AuthResponse> AddUser(AddNewUserDto addUserDto, int permission);
    Task<bool> ForgetPassword(string email);
    Task<bool> ChangePassword(ChangePasswordRequest request);

}

