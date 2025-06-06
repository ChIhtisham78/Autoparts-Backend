﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Autopart.Data.Repositories
{
    public class HomePageRepository : IHomePageRepository
    {
        private readonly autopartContext _context;
        public IUnitOfWork UnitOfWork => _context;

        public HomePageRepository(autopartContext context)
        {
            _context = context;
        }

        public async Task<List<HomePage>> GetHomePagesAsync()
        {
            var getAllPages = await _context.HomePages.ToListAsync();
            return getAllPages ?? new List<HomePage>();
        }

        public async Task<HomePage> GetByIdAsync(int id)
        {
            var getHomePage = await _context.HomePages.FindAsync(id);
            return getHomePage ?? new HomePage();
        }

        public void UpdateHomePage(HomePage homePage)
        {
            _context.HomePages.Update(homePage);
        }

        public void RemoveHomePage(HomePage homePage)
        {
            _context.HomePages.Remove(homePage);
        }
    }
}
