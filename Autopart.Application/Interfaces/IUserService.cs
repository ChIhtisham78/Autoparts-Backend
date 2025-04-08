using Autopart.Application.Models;
using Autopart.Domain.Models;
using static Autopart.Domain.Enum.EnumAndConsonent;

namespace Autopart.Application.Interfaces
{
	public interface IUserService
	{
		Task<List<UserDto>> GetUsers(OrderBy? orderBy = OrderBy.Ascending, SortedByUsername? sortedBy = SortedByUsername.UserName, string search = null);
		Task UpdateUserPhoneNumber(UpdatePhoneNumberRequest request);



		//Task<List<AspNetUser>> GetUsersByRole(int roleId);
		Task<UserDto?> GetUserById(int id);
		//Task<AspNetUser?> GetUserByEmail(string email);
		//Task UpdateUser(AddUserDto addUserDto);
		Task UpdateUser(UpdateUser updateUser);
		Task UpdateUserPassword(UpdateUserPassword updateUserPassword);
		//Task UpdateUserAddresses(UpdateUserAddress updateUserAddresses);
		Task UpdateUserAddress(UpdateUserAddress updateUserAddress);

        Task RemoveUser(int id);
		Task<List<UserDto>> GetAdmins();
		Task BanUser(int id);
		Task ActiveUser(int id);
		Task MakeAdmin(int id);
		Task<AspNetRole?> GetRoleByName(string roleName);
		Task<List<UserDto>> GetUsersWithVendorRoleInactive();
		Task<AspNetRole?> GetRoleById(int roleId);
		Task<List<StoreOwnerDto>> GetVendors(bool? isActive = null);

	}
}
