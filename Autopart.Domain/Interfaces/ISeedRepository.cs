using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Domain.Interfaces
{
    public interface ISeedRepository : IRepository<AspNetUser>
    {
        void AddUser(string email, string username, string password, string salt, string role,bool isActive);
        void AddOrderStatus(string status);
        void AddPaymentStatus(string status);
    }
}
