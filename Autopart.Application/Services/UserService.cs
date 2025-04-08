using Autopart.Application.Enum;
using Autopart.Application.Helpers;
using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Application.Options;
using Autopart.Domain.Exceptions;
using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using Microsoft.Extensions.Options;
using System.Data;
using static Autopart.Domain.Enum.EnumAndConsonent;

namespace Autopart.Application.Services
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly IPasswordHelper _passwordHelperRepo;
		private readonly AuthenticationOptions _authenticationOptions;
		private readonly ITypeAdapter _typeAdapter;

		public UserService(IUserRepository userRepository, IOptions<AuthenticationOptions> options, IPasswordHelper passwordHelperRepo, ITypeAdapter typeAdapter)
		{
			_userRepository = userRepository;
			_authenticationOptions = options.Value;
			_passwordHelperRepo = passwordHelperRepo;
			_typeAdapter = typeAdapter;
		}
		public async Task<List<UserDto>> GetUsers(OrderBy? orderBy = OrderBy.Ascending, SortedByUsername? sortedBy = SortedByUsername.UserName, string search = null)
		{
			var users = await _userRepository.GetUsers();

			if (!string.IsNullOrEmpty(search))
			{
				users = users.Where(u => u.UserName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
			}

			switch (sortedBy)
			{
				case SortedByUsername.UserName:
					users = orderBy == OrderBy.Ascending
						? users.OrderBy(u => u.UserName).ToList()
						: users.OrderByDescending(u => u.UserName).ToList();
					break;
				default:
					break;
			}

			var userDtos = users.Select(user => new UserDto
			{
				Id = user.Id,
				UserName = user.UserName,
				Email = user.Email,
				ProfileImage = user.ProfileImage,
				PhoneNumber = user.PhoneNumber,
				IsActive = user.IsActive,
				Role = user.AspNetUserRoles.FirstOrDefault()?.Role?.Name,
				Permissions = GetPermissions(user),
				Address = user.Addresses.Select(a => new AddressFormatDto
				{
					Id = a.Id,
					Title = a.Title,
					Type = a.Type,
					IsDefault = a.IsDefault,
					Address = new AddressDetailsDto
					{
						StreetAddress = a.StreetAddress,
						Country = a.Country,
						City = a.City,
						State = a.State,
						Zip = a.Zip
					}
				}).ToList(),
				Profile = user.Profiles.FirstOrDefault() != null ? new ProfileDto
				{
					Id = user.Profiles.First().Id,
					Avatar = user.Profiles.First().Avatar,
					Bio = user.Profiles.First().Bio,
					Contact = user.Profiles.First().Contact,
					CustomerId = user.Profiles.First().CustomerId,
					ImageId = user.Profiles.First().ImageId,
					SocialId = user.Profiles.First().SocialId,
					Socials = user.Profiles.First().Socials,
				} : null ?? new ProfileDto(),
			}).ToList();

			return userDtos;
		}




		private static string[] GetPermissions(AspNetUser user)
		{
			var role = user.AspNetUserRoles.FirstOrDefault()?.Role?.Name;
			if (role == null) return Array.Empty<string>();

			return role switch
			{
				Roles.Super_admin => new[] { "super_admin", "store_owner", "customer" },
				Roles.store_owner => new[] { "store_owner", "customer" },
				Roles.Customer => new[] { "customer" },
				_ => Array.Empty<string>()
			};
		}



		public async Task UpdateUserPhoneNumber(UpdatePhoneNumberRequest request)
		{
			try
			{
				var user = await _userRepository.GetUserById(request.Id);
				if (user == null)
				{
					throw new DomainException("User does not exist");
				}
				user.PhoneNumber = request.PhoneNumber;

				_userRepository.UpdateUser(user);

				await _userRepository.UnitOfWork.SaveChangesAsync();
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<UserDto?> GetUserById(int id)
		{
			var user = await _userRepository.GetUserById(id);

			if (user == null)
			{
				return null;
			}
			bool isAdmin = user.AspNetUserRoles.Any(role => role.Role.Name == "super_admin");
			bool isShopOwner = user.AspNetUserRoles.Any(role => role.Role.Name == "store_owner");

			var userDto = new UserDto
			{
				Id = user.Id,
				Email = user.Email,
				PhoneNumber = user.PhoneNumber,
				UserName = user.UserName,
				ProfileImage = user.ProfileImage,
				IsActive = user.IsActive,
				Role = user.AspNetUserRoles.FirstOrDefault()?.Role?.Name,
				Permissions = GetPermissions(user),

				StripeVendorId = user.StripeVendorId,
				StripeDashboardAccess = user.StripeDashboardAccess,
				StripeVendorStatus = user.StripeVendorStatus,
				Address = user.Addresses.Select(a => new AddressFormatDto
				{
					Id = a.Id,
					Title = a.Title,
					Type = a.Type,
					IsDefault = a.IsDefault,
					Address = new AddressDetailsDto
					{
						StreetAddress = a.StreetAddress,
						Country = a.Country,
						City = a.City,
						State = a.State,
						Zip = a.Zip
					}
				}).ToList(),

			};

			if (isAdmin || isShopOwner)
			{
				var shops = await _userRepository.GetShopsByOwnerId(user.Id);
				userDto.Shops = shops.Select(shop => new AdminShopsDto
				{
					Id = shop.Id,
					Name = shop.Name,
				}).ToList();
			}

			return userDto;
		}


		public async Task RemoveUser(int id)
		{
			var aspNetUser = await _userRepository.GetUserById(id);
			if (aspNetUser == null)
			{
				throw new DomainException("User does not exist");
			}
			var userAddresses = await _userRepository.GetUserAddressesById(id);
			foreach (var address in userAddresses)
			{
				_userRepository.DeleteAddress(address);
			}

			_userRepository.DeleteUser(aspNetUser);
			await _userRepository.UnitOfWork.SaveChangesAsync();
		}


		public async Task UpdateUser(UpdateUser updateUser)
		{
			try
			{
				var user = await _userRepository.GetUserById(updateUser.Id);
				if (user == null)
				{
					throw new DomainException("User does not exist");
				}

				//user.Email = addUserDto.Email;
				user.UserName = updateUser.UserName;
				user.Bio = updateUser.Bio;
				//user.PasswordHash = addUserDto.Password;
				user.ProfileImage = updateUser.ProfileImage;
				user.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

				_userRepository.UpdateUser(user);

				//var existingAddresses = await _userRepository.GetUserAddressesById(addUserDto.Id);

				//var addressesToRemove = existingAddresses
				//	.Where(a => !addUserDto.Address.Any(aDto => aDto.Id == a.Id))
				//	.ToList();

				//foreach (var addressDto in addUserDto.Address)
				//{
				//	var existingAddress = existingAddresses.FirstOrDefault(a => a.Id == addressDto.Id);
				//	if (existingAddress != null)
				//	{
				//		existingAddress.Title = addressDto.Title;
				//		existingAddress.Type = addressDto.Type;
				//		existingAddress.IsDefault = addressDto.IsDefault;

				//		existingAddress.StreetAddress = addressDto.Address.StreetAddress;
				//		existingAddress.Country = addressDto.Address.Country;
				//		existingAddress.City = addressDto.Address.City;
				//		existingAddress.State = addressDto.Address.State;
				//		existingAddress.Zip = addressDto.Address.Zip;

				//		_userRepository.UpdateAddress(existingAddress);
				//	}
				//}

				await _userRepository.UnitOfWork.SaveChangesAsync();
			}
			catch (Exception)
			{
				throw;
			}
		}


		public async Task UpdateUserPassword(UpdateUserPassword updateUserPassword)
		{
			try
			{
				var user = await _userRepository.GetUserById(updateUserPassword.Id);
				if (user == null)
				{
					throw new DomainException("User does not exist");
				}
				if (!IsValidPassword(updateUserPassword.OldPassword, user))
				{
					throw new DomainException("The old password is incorrect");
				}

				user.PasswordHash = _passwordHelperRepo.CreatePasswordHash(user.SecurityStamp, updateUserPassword.Password);
				user.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

				_userRepository.UpdateUser(user);

				await _userRepository.UnitOfWork.SaveChangesAsync();
			}
			catch (Exception)
			{
				throw;
			}
		}


		public async Task UpdateUserAddress(UpdateUserAddress updateUserAddress)
		{
			try
			{
				// Check if the address exists
				var existingAddress = await _userRepository.GetAddressByUserIdAndAddressId(updateUserAddress.Id, updateUserAddress.Address.Id);

				if (existingAddress == null)
				{
					// If the address does not exist, create a new one
					var newAddress = new Address
					{
						UserId = updateUserAddress.Id,
						Title = updateUserAddress.Address.Title,
						Type = updateUserAddress.Address.Type,
						IsDefault = updateUserAddress.Address.IsDefault,
						StreetAddress = updateUserAddress.Address.Address.StreetAddress,
						Country = updateUserAddress.Address.Address.Country,
						City = updateUserAddress.Address.Address.City,
						State = updateUserAddress.Address.Address.State,
						Zip = updateUserAddress.Address.Address.Zip
					};

					_userRepository.AddAddress(newAddress);
				}
				else
				{
					// If the address exists, update it
					existingAddress.Title = updateUserAddress.Address.Title;
					existingAddress.Type = updateUserAddress.Address.Type;
					existingAddress.IsDefault = updateUserAddress.Address.IsDefault;
					existingAddress.StreetAddress = updateUserAddress.Address.Address.StreetAddress;
					existingAddress.Country = updateUserAddress.Address.Address.Country;
					existingAddress.City = updateUserAddress.Address.Address.City;
					existingAddress.State = updateUserAddress.Address.Address.State;
					existingAddress.Zip = updateUserAddress.Address.Address.Zip;

					_userRepository.UpdateAddress(existingAddress);
				}

				await _userRepository.UnitOfWork.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				// Handle exceptions as needed
				throw;
			}
		}



		public async Task BanUser(int id)
		{
			var user = await _userRepository.GetUserById(id);
			if (user != null)
			{
				user.IsActive = !user.IsActive;
				await _userRepository.UnitOfWork.SaveChangesAsync();
			}
		}

		public async Task ActiveUser(int id)
		{
			var user = await _userRepository.GetUserById(id);
			if (user != null)
			{
				user.IsActive = true;
				_userRepository.UpdateUser(user);
				await _userRepository.UnitOfWork.SaveChangesAsync();
			}
		}

		public async Task MakeAdmin(int id)
		{
			var user = await _userRepository.GetUserById(id);
			if (user != null)
			{
				_userRepository.RemoveRoles(user);

				var adminRole = await _userRepository.GetRoleByName(Roles.Super_admin);

				user.AspNetUserRoles.Add(new AspNetUserRole { RoleId = adminRole.Id, UserId = user.Id });
				await _userRepository.UnitOfWork.SaveChangesAsync();
			}
		}

		public async Task<List<string>> GetAllRoles()
		{
			var roles = await _userRepository.GetAllRoles();
			return roles.Select(r => r.Name).ToList();
		}

		public async Task<List<StoreOwnerDto>> GetVendors(bool? isActive = null)
		{
			var vendors = await _userRepository.GetUsersWithStoreOwnerRole(isActive);
			var vendorDtos = vendors.Select(v => new StoreOwnerDto
			{
				Id = v.Id,
				Email = v.Email,
				PhoneNumber = v.PhoneNumber,
				UserName = v.UserName,
				IsActive = v.IsActive,
				Role = v.AspNetUserRoles.FirstOrDefault()?.Role?.Name,
				Permissions = GetPermissions(v)



			}).ToList();

			return vendorDtos;
		}

		public async Task<AspNetRole?> GetRoleByName(string roleName)
		{
			return await _userRepository.GetRoleByName(roleName);
		}

		public async Task<List<UserDto>> GetUsersWithVendorRoleInactive()
		{
			var users = await _userRepository.GetUsersWithVendorRoleInactive();
			return users.Select(user => new UserDto
			{
				Id = user.Id,
				UserName = user.UserName,
				Email = user.Email,
			}).ToList();
		}

		public async Task<AspNetRole?> GetRoleById(int roleId)
		{
			return await _userRepository.GetRoleById(roleId);
		}

		public async Task<List<UserDto>> GetAdmins()
		{
			var users = await _userRepository.GetUsersByRoleName(Roles.Super_admin);
			var userDtos = new List<UserDto>();

			foreach (var user in users)
			{
				try
				{
					var userDto = _typeAdapter.Adapt<UserDto>(user);
					userDto.Email = user.Email;
					userDto.Id = user.Id;
					userDto.PhoneNumber = user.PhoneNumber;
					userDto.Role = user.AspNetUserRoles.FirstOrDefault()?.Role?.Name;
					userDto.Permissions = GetPermissions(user);
					userDtos.Add(userDto);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error processing user {user.Id}: {ex.Message}");
				}
			}

			return userDtos;
		}


		private bool IsValidPassword(string password, AspNetUser user)
		{
			var passwordHash = _passwordHelperRepo.CreatePasswordHash(user.SecurityStamp, password);
			return passwordHash == user.PasswordHash;
		}
	}
}

