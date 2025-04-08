using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using static Autopart.Domain.Enum.EnumAndConsonent;

namespace Autopart.Domain.Interfaces
{
	public interface IUserRepository : IRepository<AspNetUser>
	{
		Task<List<AspNetUser>> GetUsers(OrderBy? orderBy = OrderBy.Ascending, SortedByUsername? sortedBy = SortedByUsername.UserName, string search = null);
		Task<List<AspNetUser>> GetUsersByRole(int roleId);
		Task<AspNetUser?> GetUserById(int id);
		Task<IEnumerable<Address>> GetUserAddressesById(int userId);
		void DeleteAddress(Address address);
		Task<AspNetUser?> GetUserByEmail(string email);
		void AddUser(AspNetUser aspNetUser);
		void AddAddress(Address address);
		void AddImage(Image image);
		void AddSocial(Social social);
		void AddProfile(Profile profile);
		void UpdateUser(AspNetUser aspNetUser);
		void UpdateAddress(Address address);
		void DeleteUser(AspNetUser aspNetUser);
		void BanUser(int id);
		void ActiveUser(int id);
		Task<AspNetRole?> GetRoleByName(string roleName);
		Task<AspNetRole?> GetRoleById(int roleId);
		Task<List<AspNetRole>> GetAllRoles();
		void AddRole(AspNetRole role);
		void RemoveRoles(AspNetUser user);
		Task<List<AspNetUser>> GetUsersWithVendorRoleInactive();
		Task<List<AspNetUser>> GetUsersByRoleName(string roleName);
		//Task<AspNetUser> UserStatusChange(int userId);
		Task<int> GetTotalVendorsCount(int? vendorId = null);
		Task<List<AspNetUser>> GetNewCustomers();
		Task<List<AspNetUser>> GetNewCustomer();
		Task<int> GetNewCustomerCountLastWeek(int? vendorId = null);
		Task<int> GetNewCustomerCountByDateRange(DateTime startDate, DateTime endDate);
		Task<bool> VendorExists(int vendorId);
		Task<List<Shop>> GetShopsByOwnerId(int ownerId);
		Task<AspNetUser> GetUserByStripeVendorId(string stripeVendorId);
		Task<List<AspNetUser>> GetUsersWithStoreOwnerRole(bool? isActive = null);
		Task<Address> GetDefualtUserAddressesByUserId(int userId);
		Task<Address> GetAddressByUserIdAndAddressId(int userId, int addressId);
	}
}
