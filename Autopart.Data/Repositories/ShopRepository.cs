using Autopart.Domain.CommonDTO;
using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Autopart.Data.Repositories
{
    public class ShopRepository : IShopRepository
    {
        private readonly autopartContext _context;


        public IUnitOfWork UnitOfWork => _context;


        public ShopRepository(autopartContext context)
        {
            _context = context;
        }

        public async Task<Shop> AddShop(Shop shop)
        {
            await _context.Shops.AddAsync(shop);
            return shop;
        }

        public async Task<List<Shop>> GetShops()
        {
            return await _context.Shops.ToListAsync();
        }

        public async Task<List<ShopWithDetails>> GetShopsWithDetailsAsync()
        {
            // Step 1: Fetch the main data with necessary joins and project into ShopWithDetails
            var shopsWithDetailsQuery = from shop in _context.Shops
                                        join logo in _context.Images on shop.LogoImageId equals logo.Id into shopLogos
                                        from shopLogo in shopLogos.DefaultIfEmpty()
                                        join cover in _context.Images on shop.CoverImageId equals cover.Id into shopCovers
                                        from shopCover in shopCovers.DefaultIfEmpty()
                                        join address in _context.Addresses on shop.Id equals address.ShopId into shopAddresses
                                        from shopAddress in shopAddresses.DefaultIfEmpty()
                                        join setting in _context.Settings on shop.Id equals setting.ShopId into shopSettings
                                        from shopSetting in shopSettings.DefaultIfEmpty()
                                        join social in _context.Socials on shop.Id equals social.ShopId into shopSocials
                                        from shopSocial in shopSocials.DefaultIfEmpty()
                                        join balance in _context.Balances on shop.Id equals balance.ShopId into shopBalances
                                        from shopBalance in shopBalances.DefaultIfEmpty()
                                        join owner in _context.AspNetUsers on shop.OwnerId equals owner.Id into shopOwners
                                        from shopOwner in shopOwners.DefaultIfEmpty()
                                        select new ShopWithDetails
                                        {
                                            Shop = shop,
                                            LogoUrl = shopLogo != null ? shopLogo.ThumbnailUrl : string.Empty,
                                            CoverUrl = shopCover != null ? shopCover.OriginalUrl : string.Empty,
                                            ShopAddress = shopAddress,
                                            Setting = shopSetting,
                                            Social = shopSocial,
                                            Balance = shopBalance,
                                            Owner = shopOwner,
                                            OrdersCount = 0,
                                            ProductCount = 0,
                                            Roles = new List<string>()
                                        };


            var shopsWithDetails = await shopsWithDetailsQuery.ToListAsync();


            var shopIds = shopsWithDetails.Select(s => s.Shop.Id).ToList();

            var productCounts = await _context.Products
                                              .Where(p => shopIds.Contains((int)p.ShopId!))
                                              .GroupBy(p => p.ShopId)
                                              .Select(g => new { ShopId = g.Key, ProductCount = g.Count() })
                                              .ToListAsync();

            var orderCounts = await (from o in _context.Orders
                                     join ol in _context.OrderLines on o.Id equals ol.OrderId
                                     join p in _context.Products on ol.ProductId equals p.Id
                                     where shopIds.Contains((int)p.ShopId!)
                                     group o by p.ShopId into g
                                     select new
                                     {
                                         ShopId = g.Key,
                                         OrdersCount = g.Count()
                                     })
                                    .ToListAsync();

            var userRoles = await (from ur in _context.AspNetUserRoles
                                   join r in _context.AspNetRoles on ur.RoleId equals r.Id
                                   where shopIds.Contains((int)ur.UserId!)
                                   group new { ur, r } by ur.UserId into g
                                   select new
                                   {
                                       UserId = g.Key,
                                       Roles = g.Select(x => x.r.Name).ToList()
                                   }).ToListAsync();




            foreach (var shopDetail in shopsWithDetails)
            {
                shopDetail.OrdersCount = orderCounts.FirstOrDefault(o => o.ShopId == shopDetail.Shop.Id)?.OrdersCount ?? 0;
                shopDetail.ProductCount = productCounts.FirstOrDefault(p => p.ShopId == shopDetail.Shop.Id)?.ProductCount ?? 0;
                if (shopDetail.Owner != null)
                {
                    shopDetail.Roles = userRoles.FirstOrDefault(r => r.UserId == shopDetail.Owner.Id)?.Roles ?? new List<string>();
                }
            }

            return shopsWithDetails;
        }



        public async Task<int> GetOrdersCount(int shopId)
        {
            return await _context.Orders.CountAsync(x => x.ShopId == shopId);
        }

        public async Task<int> GetProductCount(int shopId)
        {
            return await _context.Products.CountAsync(x => x.ShopId == shopId);
        }

        public async Task<string> GetOwnerName(int? ownerId)
        {
            var user = await _context.AspNetUsers.FirstOrDefaultAsync(x => x.Id == ownerId);
            return user?.UserName!;
        }
        public async Task<string> GetImageById(int? imageId)
        {
            var image = await _context.Images.FirstOrDefaultAsync(x => x.Id == imageId);
            return image.OriginalUrl;
        }

        public async Task<Image?> GetImagesById(int? imageId)
        {
            return await _context.Images.FirstOrDefaultAsync(x => x.Id == imageId);
        }

        //public async Task<Shop> GetShopById(int Id)
        //{
        //    return await _context.Shops.Where(s => s.Id == Id).FirstOrDefaultAsync();
        //}

        public async Task<Shop> GetShopById(int id)
        {
            return await _context.Shops.Where(s => s.Id == id).FirstOrDefaultAsync() ?? new Shop();
        }
        public async Task<Shop> GetShopBySlug(string slug)
        {
            return await _context.Shops.Where(s => s.Slug == slug).FirstOrDefaultAsync() ?? new Shop();
        }

        //public async Task<IEnumerable<Shop>> GetShopBySlug(string slug)
        //{
        //    return await _context.Shops.Where(x => x.Slug.Contains(slug)).ToListAsync();
        //}

        public async Task<Address> GetShopAddressById(int shopId)
        {
            return await _context.Addresses.FirstOrDefaultAsync(a => a.ShopId == shopId) ?? new Address();
        }
        public async Task<Setting> GetSettingByShopId(int shopId)
        {
            return await _context.Settings.Where(s => s.ShopId == shopId).FirstOrDefaultAsync() ?? new Setting();
        }

        public async Task<AspNetUser?> GetShopByUserId(int ownerId)
        {
            return await _context.AspNetUsers.Where(x => x.Id == ownerId).FirstOrDefaultAsync();
        }

        public async Task<AspNetUser?> GetShopByUserRoleId(int ownerId)
        {
            return await _context.AspNetUsers.Include(u => u.AspNetUserRoles).ThenInclude(ur => ur.Role).Where(u => u.Id == ownerId)
                .FirstOrDefaultAsync();
        }

        public async Task<Social> GetSocialByShopId(int shopId)
        {
            var result = await _context.Socials.FirstOrDefaultAsync(s => s.ShopId == shopId) ?? new Social();
            return result;
        }

        public async Task<Balance> GetBalanceByShopId(int shopId)
        {
            var result = await _context.Balances.FirstOrDefaultAsync(b => b.ShopId == shopId) ?? new Balance();
            return result;
        }


        public async Task<List<Shop>> GetShopsByOwnerIdAsync(int ownerId)
        {
            var shops = await (from shop in _context.Shops
                               join logo in _context.Images on shop.LogoImageId equals logo.Id into logoGroup
                               from logo in logoGroup.DefaultIfEmpty()
                               join cover in _context.Images on shop.CoverImageId equals cover.Id into coverGroup
                               from cover in coverGroup.DefaultIfEmpty()
                               join address in _context.Addresses on shop.Id equals address.ShopId into addressGroup
                               from address in addressGroup.DefaultIfEmpty()
                               join setting in _context.Settings on shop.Id equals setting.ShopId into settingGroup
                               from setting in settingGroup.DefaultIfEmpty()
                               join social in _context.Socials on shop.Id equals social.ShopId into socialGroup
                               from social in socialGroup.DefaultIfEmpty()
                               join balance in _context.Balances on shop.Id equals balance.ShopId into balanceGroup
                               from balance in balanceGroup.DefaultIfEmpty()
                               where shop.OwnerId == ownerId
                               select new
                               {
                                   Shop = shop,
                                   LogoImage = logo,
                                   CoverImage = cover,
                                   Address = address,
                                   Setting = setting,
                                   Social = social,
                                   Balance = balance
                               }).ToListAsync();

            var result = shops.GroupBy(s => s.Shop.Id).Select(g => new Shop
            {
                Id = g.Key,
                Name = g.FirstOrDefault()!.Shop.Name,
                OwnerId = g.FirstOrDefault()!.Shop.OwnerId,
                Slug = g.FirstOrDefault()!.Shop.Slug,
                IsActive = g.FirstOrDefault()!.Shop.IsActive,
                Description = g.FirstOrDefault()!.Shop.Description,
                CreatedAt = g.FirstOrDefault()!.Shop.CreatedAt,
                LogoImage = g.FirstOrDefault()!.LogoImage,
                CoverImage = g.FirstOrDefault()!.CoverImage,
                Addresses = g.Select(a => a.Address).Where(a => a != null).ToList(),
                Setting = g.FirstOrDefault()!.Setting,
                Socials = g.Select(s => s.Social).Where(s => s != null).ToList(),
                Balance = g.FirstOrDefault()!.Balance
            }).ToList();

            return result;
        }


        public async Task<Shop> UpdateShop(Shop shop)
        {
            var existingShop = await _context.Shops.Include(s => s.CoverImage).FirstOrDefaultAsync(s => s.Id == shop.Id);
            if (existingShop == null)
            {
                return null!;
            }
            existingShop.Name = shop.Name;
            existingShop.Description = shop.Description;
            _context.Shops.Update(existingShop);
            return existingShop;
        }


        public async Task<Address> UpdateAddress(int shopId, Address address)
        {
            var existingAddress = await _context.Addresses.FirstOrDefaultAsync(a => a.ShopId == shopId);
            if (existingAddress == null)
            {
                return null!;
            }
            existingAddress.Zip = address.Zip;
            existingAddress.City = address.City;
            existingAddress.Country = address.Country;
            existingAddress.State = address.State;
            existingAddress.StreetAddress = address.StreetAddress;
            _context.Addresses.Update(existingAddress);
            return existingAddress;
        }


        public async Task<Balance> UpdateBalance(int id, Balance balance)
        {
            var existingBalance = await _context.Balances.FindAsync(id);
            if (existingBalance == null)
            {
                return null!;
            }
            existingBalance.AdminCommissionRate = balance.AdminCommissionRate;
            existingBalance.Shop = balance.Shop;
            existingBalance.TotalEarnings = balance.TotalEarnings;
            existingBalance.WithDrawnAmount = balance.WithDrawnAmount;
            existingBalance.CurrentBalance = balance.CurrentBalance;
            existingBalance.AccountNumber = balance.AccountNumber;
            existingBalance.AccountHolderEmail = balance.AccountHolderEmail;
            existingBalance.AccountHolderName = balance.AccountHolderName;
            existingBalance.BankName = balance.BankName;

            _context.Balances.Update(existingBalance);
            return existingBalance;
        }



        public async Task<Image> UpdateImage(Image image)
        {

            _context.Images.Update(image);
            return image;
        }



        public async Task<Setting> UpdateSetting(Setting setting)
        {
            _context.Settings.Update(setting);
            return setting;
        }

        public async Task<Social> UpdateSocial(Social social)
        {
            _context.Socials.Update(social);
            return social;
        }


        public async Task<bool> DeleteShop(int id)
        {
            var shop = await _context.Shops.FindAsync(id);
            if (shop == null)
            {
                return false;
            }
            _context.Shops.Remove(shop);
            return true;
        }

        public async Task<Shop> ApproveShop(int id)
        {
            var shop = await _context.Shops.FindAsync(id);
            if (shop == null)
            {
                return null!;
            }
            shop.IsActive = true;
            _context.Shops.Update(shop);
            return shop;
        }

        public async Task<Shop> DisapproveShop(int id)
        {
            var shop = await _context.Shops.FindAsync(id);
            if (shop == null)
            {
                return null!;
            }
            shop.IsActive = false;
            _context.Shops.Update(shop);
            return shop;
        }


        public async Task AddAddress(Address address)
        {
            await _context.Addresses.AddAsync(address);
        }


        public async Task AddSetting(Setting setting)
        {
            await _context.Settings.AddAsync(setting);
        }


        public async Task AddImage(Image image)
        {
            await _context.Images.AddAsync(image);
        }

        public async Task AddSocial(Social social)
        {

            await _context.Socials.AddAsync(social);
        }


        //public async Task AddSocialRange(IEnumerable<Social> socials)
        //{
        //    await _context.Socials.AddRangeAsync(socials);

        //}

        public async Task AddBalance(Balance balance)
        {
            await _context.Balances.AddAsync(balance);
        }


        // Inside ShopRepository class
        public async Task DeleteAddresses(int shopId)
        {
            var addresses = await _context.Addresses.Where(a => a.ShopId == shopId).ToListAsync();
            _context.Addresses.RemoveRange(addresses);
        }

        public async Task DeleteBalances(int shopId)
        {
            var balances = await _context.Balances.Where(b => b.ShopId == shopId).ToListAsync();
            _context.Balances.RemoveRange(balances);
        }

        public async Task DeleteImages(int shopId)
        {
            var shop = await _context.Shops.Where(i => i.Id == shopId).FirstOrDefaultAsync();
            var imagesToDelete = new List<Image>();
            var coverImage = await _context.Images.Where(x => x.Id == shop.CoverImageId).FirstOrDefaultAsync();
            var logoImage = await _context.Images.Where(x => x.Id == shop.LogoImageId).FirstOrDefaultAsync();
            if (coverImage != null)
            {
                imagesToDelete.Add(coverImage);
            }

            if (logoImage != null)
            {
                imagesToDelete.Add(logoImage);
            }
            _context.Images.RemoveRange(imagesToDelete);
        }

        public async Task DeleteSocials(int shopId)
        {
            var socials = await _context.Socials.Where(s => s.ShopId == shopId).ToListAsync();
            _context.Socials.RemoveRange(socials);
        }

        public async Task DeleteSettings(int shopId)
        {
            var settings = await _context.Settings.Where(s => s.ShopId == shopId).ToListAsync();
            _context.Settings.RemoveRange(settings);
        }
        public async Task DeleteOrders(int shopId)
        {
            var order = await _context.Orders.Where(s => s.ShopId == shopId).ToListAsync();
            _context.Orders.RemoveRange(order);
        }
        public async Task DeleteCoupons(int shopId)
        {
            var coupons = await _context.Coupons.Where(s => s.ShopId == shopId).ToListAsync();
            _context.Coupons.RemoveRange(coupons);
        }
        public async Task DeleteAttribute(int shopId)
        {
            try
            {

                var shop = await _context.Shops.FindAsync(shopId);
                if (shop != null)
                {

                    var attributes = await _context.Attributes.Where(a => a.ShopId == shopId).ToListAsync();


                    foreach (var attribute in attributes)
                    {
                        attribute.ShopId = null;
                    }


                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while deleting the shop.", ex);
            }
        }
        public async Task<bool> SlugExistsAsync(string slug)
        {
            return await _context.Shops.AnyAsync(p => p.Slug == slug);
        }

        public async Task<int> GetShopsCount(int? vendorId = null)
        {
            if (vendorId.HasValue)
               return await _context.Shops.CountAsync(shop => shop.OwnerId == vendorId.Value);
            
            else
             return await _context.Shops.CountAsync();
            
        }

    }
}
