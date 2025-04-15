using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;
using static Autopart.Domain.Enum.EnumAndConsonent;

namespace Autopart.Data.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly autopartContext _context;
		public IUnitOfWork UnitOfWork => _context;

		public UserRepository(autopartContext context)
		{
			_context = context;
		}

		public async Task<List<AspNetUser>> GetUsers(OrderBy? orderBy = OrderBy.Ascending, SortedByUsername? sortedBy = SortedByUsername.UserName, string search = null)
		{
			return await _context.AspNetUsers
				.Include(x => x.AspNetUserRoles)
				.ThenInclude(x => x.Role)
				.Include(x => x.Addresses)
				.ToListAsync();
		}

		public void UpdateAddress(Address address)
		{
			_context.Addresses.Update(address);
		}

		public void AddUser(AspNetUser aspNetUser)
		{
			_context.AspNetUsers.Add(aspNetUser);
		}
		public void DeleteAddress(Address address)
		{
			_context.Addresses.Remove(address);
		}


		public void AddImage(Image image)
		{
			_context.Images.Add(image);
		}
		public void AddSocial(Social social)
		{
			_context.Socials.Add(social);
		}
		public void AddAddress(Address address)
		{
			_context.Addresses.Add(address);
		}
		public void AddProfile(Profile profile)
		{
			_context.Profiles.Add(profile);
		}

		public async Task<IEnumerable<Address>> GetUserAddressesById(int userId)
		{
			return await _context.Addresses.Where(a => a.UserId == userId).ToListAsync();
		}
		public async Task<Address> GetDefualtUserAddressesByUserId(int userId)
		{
			return await _context.Addresses.FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault == 1) ?? new Address();
		}
		public async Task<Address> GetAddressByUserIdAndAddressId(int userId, int addressId)
		{
			return await _context.Addresses
				.FirstOrDefaultAsync(a => a.UserId == userId && a.Id == addressId)
				?? new Address();
		}


		public void UpdateUser(AspNetUser aspNetUser)
		{
			_context.AspNetUsers.Update(aspNetUser);
		}

		public void DeleteUser(AspNetUser aspNetUser)
		{
			_context.AspNetUsers.Remove(aspNetUser);
		}

		//public async Task<List<AspNetUser>> GetUsers()
		//{
		//    return await _context.AspNetUsers.Include(u => u.AspNetUserRoles).ThenInclude(ur => ur.Role).ToListAsync();
		//}


		public async Task<List<AspNetUser>> GetUsersByRole(int roleId)
		{
			return await _context.AspNetUsers.Include(x => x.AspNetUserRoles).ThenInclude(x => x.Role).Where(x => x.AspNetUserRoles.FirstOrDefault()!.RoleId == roleId).ToListAsync();
		}


		public async Task<AspNetUser?> GetUserById(int id)
		{
			return await _context.AspNetUsers
				.Include(x => x.AspNetUserRoles)
				.ThenInclude(x => x.Role)
				.Include(x => x.Addresses)
				.FirstOrDefaultAsync(x => x.Id == id);
		}

		public async Task<AspNetUser?> GetUserByEmail(string email)
		{
			return await _context.AspNetUsers.Include(x => x.AspNetUserRoles).ThenInclude(x => x.Role).FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower().Trim());
		}

		public void BanUser(int id)
		{
			_context.AspNetUsers.FirstOrDefault(u => u.Id == id);
		}

		public void ActiveUser(int id)
		{
			_context.AspNetUsers.FirstOrDefault(u => u.Id == id);
		}
		public async Task<AspNetRole?> GetRoleByName(string roleName)
		{
			return await _context.AspNetRoles.FirstOrDefaultAsync(r => r.Name == roleName);
		}

		public void RemoveRoles(AspNetUser user)
		{
			user.AspNetUserRoles.Clear();
		}

		public void AddRole(AspNetRole role)
		{
			_context.AspNetRoles.Add(role);
		}
		public async Task<List<AspNetRole>> GetAllRoles()
		{
			return await _context.AspNetRoles.ToListAsync();
		}

		public async Task<List<AspNetUser>> GetUsersWithVendorRoleInactive()
		{
			return await _context.AspNetUsers
				.Include(u => u.AspNetUserRoles)
				.ThenInclude(ur => ur.Role)
				.Where(u => u.AspNetUserRoles.Any(ur => ur.Role.Name == "Vendor" && u.IsActive == false))
				.ToListAsync();
		}

		public async Task<AspNetRole?> GetRoleById(int roleId)
		{
			return await _context.AspNetRoles.FirstOrDefaultAsync(r => r.Id == roleId);
		}
		//public void GetRoles(string roleName)
		//{
		//     _context.AspNetRoles.FirstOrDefaultAsync(r => r.Name == roleName);
		//}

		//public async Task<AspNetUser> UserStatusChange(int id)
		//{
		//    return await _context.AspNetUsers.FirstOrDefault(u => u.i == id);
		//}
		public async Task<List<AspNetUser>> GetUsersByRoleName(string roleName)
		{
			return await _context.AspNetUsers
				.Include(u => u.AspNetUserRoles)
				.ThenInclude(ur => ur.Role)
				.Where(u => u.AspNetUserRoles.Any(ur => ur.Role.Name == roleName))
				.ToListAsync();
		}


		public async Task<int> GetTotalVendorsCount(int? vendorId = null)
		{
			var query = _context.AspNetUsers
		.Include(u => u.AspNetUserRoles)
		.ThenInclude(ur => ur.Role)
		.Where(u => u.AspNetUserRoles.Any(ur => ur.Role.Name == "store_owner"));

			if (vendorId.HasValue)
			{
				query = query.Where(u => u.Id == vendorId.Value);
			}

			return await query.CountAsync();
			//return await _context.AspNetUsers.Include(u => u.AspNetUserRoles).ThenInclude(ur => ur.Role)
			//    .Where(u => u.AspNetUserRoles.Any(ur => ur.Role.Name == "store_owner")).CountAsync();
		}

		public async Task<List<AspNetUser>> GetNewCustomers()
		{
			return await _context.AspNetUsers.Include(u => u.AspNetUserRoles).ThenInclude(ur => ur.Role)
				.Where(u => u.AspNetUserRoles.Any(ur => ur.Role.Name == "customer"))
				.OrderByDescending(u => u.CreatedAt)
				.Take(10)
				.ToListAsync();
		}

		public async Task<List<AspNetUser>> GetNewCustomer()
		{
			return await _context.AspNetUsers
				.OrderByDescending(u => u.CreatedAt) // Order by creation date descending
				.Take(10) // Get top 10
				.ToListAsync(); // Execute query and return results
		}

		public async Task<int> GetNewCustomerCountLastWeek(int? vendorId = null)
		{
			var startDate = DateTime.UtcNow.AddDays(-7).Date;
			var endDate = DateTime.UtcNow.Date;

			IQueryable<AspNetUser> query = _context.AspNetUsers
				.Where(u => u.CreatedAt >= DateTime.SpecifyKind(startDate, DateTimeKind.Unspecified)
						 && u.CreatedAt < DateTime.SpecifyKind(endDate, DateTimeKind.Unspecified));
			//.CountAsync();
			if (vendorId.HasValue)
			{
				query = query.Where(u => u.Id == vendorId.Value);
			}
			return await query.CountAsync();
		}


		public async Task<int> GetNewCustomerCountByDateRange(DateTime startDate, DateTime endDate)
		{
			return await _context.AspNetUsers
				.Where(u => u.CreatedAt >= startDate && u.CreatedAt <= endDate)
				.CountAsync();
		}

		public async Task<bool> VendorExists(int vendorId)
		{
			return await _context.AspNetUsers.AnyAsync(u => u.Id == vendorId);
		}


		public async Task<List<Shop>> GetShopsByOwnerId(int ownerId)
		{
			return await _context.Shops.Where(shop => shop.OwnerId == ownerId).ToListAsync();
		}

		public async Task<AspNetUser> GetUserByStripeVendorId(string stripeVendorId)
		{
			return await _context.AspNetUsers.FirstOrDefaultAsync(u => u.StripeVendorId == stripeVendorId);
		}


		public async Task<List<AspNetUser>> GetUsersWithStoreOwnerRole(bool? isActive = null)
		{
			//return await _context.AspNetUsers.Include(u => u.AspNetUserRoles).ThenInclude(ur => ur.Role).Where(u => u.AspNetUserRoles.Any(ur => ur.Role.Name == "store_owner"))
			//    .ToListAsync();
			var query = _context.AspNetUsers
	   .Include(u => u.AspNetUserRoles)
	   .ThenInclude(ur => ur.Role)
	   .Where(u => u.AspNetUserRoles.Any(ur => ur.Role.Name == "store_owner"));

			if (isActive.HasValue)
			{
				query = query.Where(u => u.IsActive == isActive.Value);
			}

			return await query.ToListAsync();
		}

	}
}
