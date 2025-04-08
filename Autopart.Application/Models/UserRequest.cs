using Autopart.Application.Models;

public class UserRequest
{
    public AddNewUserDto addNewUserDto { get; set; }
    //public AuthenticatedUserResponse authenticatedUserResponse { get; set; }    
    public int Permission { get; set; }
}
