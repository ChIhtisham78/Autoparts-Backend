using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Data.Repositories
{
    public class SeedRepository : ISeedRepository
    {
        private readonly autopartContext _context;
        public IUnitOfWork UnitOfWork => _context;

        public SeedRepository(autopartContext context)
        {
            _context = context;
        }

        public void AddUser(string email, string username, string password, string salt, string role, bool isActive)
        {
            var admin = _context.AspNetUsers.FirstOrDefault(x => x.Email == email);
            if (admin == null)
            {
                var user = new AspNetUser
                {
                    Email = email,
                    UserName = username,
                    PasswordHash = password,
                    EmailConfirmed = true,
                    NormalizedEmail = email.ToUpper(),
                    SecurityStamp = salt,
                    NormalizedUserName = username.ToUpper(),
                    IsActive = isActive,
                };
                _context.AspNetUsers.Add(user);
                _context.SaveChanges();

                var roleInDb = AddRole(role);

                AssignRoleToUser(user.Id, roleInDb.Id);
            }
        }
        private AspNetRole AddRole(string role)
        {
            var roleInDb = _context.AspNetRoles.FirstOrDefault(r => r.Name == role);
            if (roleInDb == null)
            {
                var guid = Guid.NewGuid();
                roleInDb = new AspNetRole { Name = role, ConcurrencyStamp = guid.ToString() };
                _context.AspNetRoles.Add(roleInDb);
                _context.SaveChanges();
            }
            return roleInDb;
        }
        private void AssignRoleToUser(int userId, int roleId)
        {

            var userRole = new AspNetUserRole()
            {
                RoleId = roleId,
                UserId = userId
            };
            _context.AspNetUserRoles.Add(userRole);
            _context.SaveChanges();
        }

        public void AddOrderStatus(string status)
        {
            if (!_context.Statuses.Any(os => os.Name == status))  // Updated to match the property name
            {
                _context.Statuses.Add(new Status { Name = status });  // Updated to match the property name
                _context.SaveChanges();
            }
        }
    }

}
