using Autopart.Domain.Interfaces;
using Autopart.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Data.Repositories
{
    public class AccountRepository: IAccountRepository
    {
        private readonly autopartContext _context;
        public IUnitOfWork UnitOfWork => _context;

        public AccountRepository(autopartContext context)
        {
            _context = context;
        }


    }
}
