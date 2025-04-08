using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Application.Models.Dto;
using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Autopart.Application.Services
{
    public class ShopService : IShopService
    {
        private readonly IShopRepository _shopRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITypeAdapter _typeAdapter;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly RootFolder _rootFolder;


        public ShopService(IShopRepository shopRepository, ITypeAdapter typeAdapter, IOptions<RootFolder> rootFolder, IWebHostEnvironment webHostEnvironment, IUserRepository userRepository)
        {
            _shopRepository = shopRepository;
            _userRepository = userRepository;
            _typeAdapter = typeAdapter;
            _rootFolder = rootFolder.Value;
            _webHostEnvironment = webHostEnvironment;

        }

        public async Task<ShopDto> AddShop(ShopDtoRequest shopDtoRequest, int userId)
        {
            try
            {
                var user = await _userRepository.GetUserById(userId);

                int coverImageId = 0;
                int logoImageId = 0;
                string certificateUrl = null;

                if (shopDtoRequest.imageDto?.CoverImage != null || shopDtoRequest.imageDto?.Logo != null)
                {
                    var image = new Image
                    {
                        OriginalUrl = shopDtoRequest.imageDto.CoverImage,
                        ThumbnailUrl = shopDtoRequest.imageDto.Logo,
                        CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                        UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                    };

                    await _shopRepository.AddImage(image);
                    await _shopRepository.UnitOfWork.SaveChangesAsync();
                    coverImageId = image.Id;
                    logoImageId = image.Id;
                }

                if (!string.IsNullOrEmpty(shopDtoRequest.CertificateUrl))
                {
                    certificateUrl = shopDtoRequest.CertificateUrl;
                }

                var slug = await EnsureUniqueSlugAsync(shopDtoRequest.Name, shopDtoRequest.Slug);

                var shop = new Shop
                {
                    Name = shopDtoRequest.Name,
                    Description = shopDtoRequest.Description,
                    CoverImageId = coverImageId,
                    LogoImageId = logoImageId,
                    Slug = slug,
                    OwnerId = userId,
                    IsActive = user.IsActive,
                    CertificateUrl = certificateUrl
                };

                var createdShop = await _shopRepository.AddShop(shop);
                await _shopRepository.UnitOfWork.SaveChangesAsync();

                var address = new Address
                {
                    ShopId = createdShop.Id,
                    Zip = shopDtoRequest.shopAddressDto.Zip,
                    City = shopDtoRequest.shopAddressDto.City,
                    State = shopDtoRequest.shopAddressDto.State,
                    Country = shopDtoRequest.shopAddressDto.Country,
                    StreetAddress = shopDtoRequest.shopAddressDto.StreetAddress,
                };

                await _shopRepository.AddAddress(address);
                await _shopRepository.UnitOfWork.SaveChangesAsync();

                var balance = new Balance
                {
                    ShopId = createdShop.Id,
                    AdminCommissionRate = shopDtoRequest.balanceDto.AdminCommissionRate,
                    Shop = createdShop.Name,
                    TotalEarnings = shopDtoRequest.balanceDto.TotalEarnings,
                    WithDrawnAmount = shopDtoRequest.balanceDto.WithDrawnAmount,
                    CurrentBalance = shopDtoRequest.balanceDto.CurrentBalance,
                    AccountNumber = shopDtoRequest.balanceDto.AccountNumber,
                    AccountHolderEmail = shopDtoRequest.balanceDto.AccountHolderEmail,
                    AccountHolderName = shopDtoRequest.balanceDto.AccountHolderName,
                    BankName = shopDtoRequest.balanceDto.BankName,
                };

                await _shopRepository.AddBalance(balance);
                await _shopRepository.UnitOfWork.SaveChangesAsync();

                if (shopDtoRequest.socialDtos != null)
                {
                    var social = new Social
                    {
                        ShopId = createdShop.Id,
                        Url = shopDtoRequest.socialDtos.Url,
                        Icon = shopDtoRequest.socialDtos.Icon,
                    };
                    await _shopRepository.AddSocial(social);
                    await _shopRepository.UnitOfWork.SaveChangesAsync();
                }

                var setting = new Setting
                {
                    ShopId = createdShop.Id,
                    Contact = shopDtoRequest.settingDto.Contact,
                    Website = shopDtoRequest.settingDto.Website,
                    LocationLat = shopDtoRequest.settingDto.LocationLat,
                    LocationLng = shopDtoRequest.settingDto.LocationLng,
                    LocationCity = shopDtoRequest.settingDto.LocationCity,
                    LocationState = shopDtoRequest.settingDto.LocationState,
                    LocationZip = shopDtoRequest.settingDto.LocationZip,
                    LocationCountry = shopDtoRequest.settingDto.LocationCountry,
                    LocationFormattedAddress = shopDtoRequest.settingDto.LocationFormattedAddress,
                };

                await _shopRepository.AddSetting(setting);
                await _shopRepository.UnitOfWork.SaveChangesAsync();

                return _typeAdapter.Adapt<ShopDto>(createdShop);
            }
            catch (Exception exp)
            {
                // Handle exceptions
                throw;
            }
        }

        public async Task<string> UploadImage(IFormFile image)
        {
            var uploadsFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{image.FileName}";
            var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }
            return $"uploads/{uniqueFileName}";
        }

        public async Task<List<ShopDto>> GetShops()
        {
            var shopDetails = await _shopRepository.GetShopsWithDetailsAsync();
            var result = new List<ShopDto>();

            foreach (var detail in shopDetails)
            {
                var shopDto = new ShopDto
                {
                    Id = detail.Shop.Id,
                    Name = detail.Shop.Name,
                    OwnerId = detail.Shop.OwnerId,
                    Slug = detail.Shop.Slug,
                    Description = detail.Shop.Description,
                    IsActive = detail.Shop.IsActive,
                    OrdersCount = detail.OrdersCount,
                    ProductCount = detail.ProductCount,
                    Status = detail.Shop.IsActive,
                    CertificateUrl = detail.Shop.CertificateUrl,
                    OwnerName = detail.Owner?.UserName ?? string.Empty,
                    CreatedAt = detail.Shop.CreatedAt,
                    Logo = detail.LogoUrl,
                    CoverImage = detail.CoverUrl,

                    shopAddressDto = detail.ShopAddress != null ? new ShopAddressDto
                    {
                        Id = detail.ShopAddress.Id,
                        StreetAddress = detail.ShopAddress.StreetAddress,
                        City = detail.ShopAddress.City,
                        State = detail.ShopAddress.State,
                        Zip = detail.ShopAddress.Zip,
                        Country = detail.ShopAddress.Country
                    } : null,

                    settingDto = detail.Setting != null ? new SettingDto
                    {
                        Contact = detail.Setting.Contact,
                        Website = detail.Setting.Website,
                        LocationLat = detail.Setting.LocationLat,
                        LocationLng = detail.Setting.LocationLng,
                        LocationZip = detail.Setting.LocationZip,
                        LocationCity = detail.Setting.LocationCity,
                        LocationState = detail.Setting.LocationState,
                        LocationCountry = detail.Setting.LocationCountry,
                        LocationFormattedAddress = detail.Setting.LocationFormattedAddress,
                    } : null,

                    socialDto = detail.Social != null ? new SocialDto
                    {
                        Id = detail.Social.Id,
                        Link = detail.Social.Link,
                        Icon = detail.Social.Icon,
                        Url = detail.Social.Url,
                        Type = detail.Social.Type,
                    } : null,

                    balanceDto = detail.Balance != null ? new BalanceDto
                    {
                        Id = detail.Balance.Id,
                        ShopId = detail.Balance.ShopId,
                        AdminCommissionRate = detail.Balance.AdminCommissionRate,
                        TotalEarnings = detail.Balance.TotalEarnings,
                        WithDrawnAmount = detail.Balance.WithDrawnAmount,
                        CurrentBalance = detail.Balance.CurrentBalance,
                        AccountNumber = detail.Balance.AccountNumber,
                        AccountHolderEmail = detail.Balance.AccountHolderEmail,
                        AccountHolderName = detail.Balance.AccountHolderName,
                        BankName = detail.Balance.BankName
                    } : null,

                    OrderUserDto = detail.Owner != null ? new OrderUserDto
                    {
                        Id = detail.Owner.Id,
                        UserName = detail.Owner.UserName,
                        Email = detail.Owner.Email,
                        EmailConfirmed = detail.Owner.EmailConfirmed,
                        PhoneNumber = detail.Owner.PhoneNumber,
                        IsActive = detail.Owner.IsActive,
                        ShopId = detail.Shop.Id,
                        CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                        Roles = detail.Roles != null ? string.Join(",", detail.Roles) : null
                    } : null
                };

                result.Add(shopDto);
            }

            return _typeAdapter.Adapt<List<ShopDto>>(result);
        }


        public async Task<ShopDto> GetShop(int Id)
        {
            var shop = await _shopRepository.GetShopById(Id);
            if (shop == null)
            {
                return null;
            }
            //if (Filter != null)
            //{
            //    if (filer.slug != null)
            //    {
            //        shop = 
            //    }
            //}
            string logoUrl = string.Empty;
            string coverUrl = string.Empty;

            if (shop.LogoImageId.HasValue)
            {
                var logo = await _shopRepository.GetImageById(shop.LogoImageId.Value);
                logoUrl = logo;
            }

            if (shop.CoverImageId.HasValue)
            {
                var cover = await _shopRepository.GetImageById(shop.CoverImageId.Value);
                coverUrl = cover;
            }

            var shopAddress = await _shopRepository.GetShopAddressById(shop.Id);
            var add = new List<ShopAddressDto>();
            var setting = await _shopRepository.GetSettingByShopId(shop.Id);
            var sett = new List<SettingDto>();
            var social = await _shopRepository.GetSocialByShopId(shop.Id);
            var soc = new List<SocialDto>();
            var balance = await _shopRepository.GetBalanceByShopId(shop.Id);
            var bal = new List<BalanceDto>();
            //var user = await _shopRepository.GetUserById(shop.Id);
            var us = new List<AspNetUser>();
            AspNetUser? user = null;
            List<string>? ownerRoles = null;

            if (shop.OwnerId.HasValue)
            {
                user = await _shopRepository.GetShopByUserId(shop.OwnerId.Value);
                if (user != null)
                {
                    ownerRoles = await GetRolesByUserId(user.Id);
                }
            }



            var res = new ShopDto
            {
                Id = shop.Id,
                Name = shop.Name,
                OwnerId = shop.OwnerId,
                Slug = shop.Slug,
                Description = shop.Description,
                OrdersCount = await _shopRepository.GetOrdersCount(shop.Id),
                ProductCount = await _shopRepository.GetProductCount(shop.Id),
                Status = shop.IsActive,
                OwnerName = await _shopRepository.GetOwnerName(shop.OwnerId),
                CreatedAt = shop.CreatedAt,
                Logo = logoUrl,
                CoverImage = coverUrl,


                shopAddressDto = shopAddress != null ? new ShopAddressDto
                {
                    Id = shopAddress.Id,
                    StreetAddress = shopAddress.StreetAddress,
                    City = shopAddress.City,
                    State = shopAddress.State,
                    Zip = shopAddress.Zip,
                    Country = shopAddress.Country
                } : null ?? new ShopAddressDto(),
                settingDto = setting != null ? new SettingDto
                {
                    Contact = setting.Contact,
                    Website = setting.Website,
                    LocationLat = setting.LocationLat,
                    LocationLng = setting.LocationLng,
                    LocationZip = setting.LocationZip,
                    LocationCity = setting.LocationCity,
                    LocationState = setting.LocationState,
                    LocationCountry = setting.LocationCountry,
                    LocationFormattedAddress = setting.LocationFormattedAddress,
                } : null ?? new SettingDto(),
                socialDto = social != null ? new SocialDto
                {
                    Id = social.Id,
                    Link = social.Link,
                    Icon = social.Icon,
                    Url = social.Url,
                    Type = social.Type,

                } : null ?? new SocialDto(),

                balanceDto = balance != null ? new BalanceDto
                {
                    Id = balance.Id,
                    AdminCommissionRate = balance.AdminCommissionRate,
                    TotalEarnings = balance.TotalEarnings,
                    WithDrawnAmount = balance.WithDrawnAmount,
                    CurrentBalance = balance.CurrentBalance,
                    AccountNumber = balance.AccountNumber,
                    AccountHolderEmail = balance.AccountHolderEmail,
                    AccountHolderName = balance.AccountHolderName,
                    BankName = balance.BankName
                } : null ?? new BalanceDto(),
                OrderUserDto = user != null ? new OrderUserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    PhoneNumber = user.PhoneNumber,
                    IsActive = user.IsActive,
                    ShopId = user.Id,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                    Roles = ownerRoles != null ? string.Join(",", ownerRoles) : null

                } : null ?? new OrderUserDto(),
            };



            return _typeAdapter.Adapt<ShopDto>(res);
        }

        public async Task<ShopDto> GetShopBySlug(string slug)
        {
            var shop = await _shopRepository.GetShopBySlug(slug);
            if (shop == null)
            {
                return null; // Or throw an exception if shop not found
            }
            string logoUrl = string.Empty;
            string coverUrl = string.Empty;

            if (shop.LogoImageId.HasValue)
            {
                var logo = await _shopRepository.GetImageById(shop.LogoImageId.Value);
                logoUrl = logo.StartsWith("https") ? logo : _rootFolder.ApplicationUrl + logo;
            }

            if (shop.CoverImageId.HasValue)
            {
                var cover = await _shopRepository.GetImageById(shop.CoverImageId.Value);
                coverUrl = cover.StartsWith("https") ? cover : _rootFolder.ApplicationUrl + cover;
            }

            var shopAddress = await _shopRepository.GetShopAddressById(shop.Id);
            var add = new List<ShopAddressDto>();
            var setting = await _shopRepository.GetSettingByShopId(shop.Id);
            var sett = new List<SettingDto>();
            var social = await _shopRepository.GetSocialByShopId(shop.Id);
            var soc = new List<SocialDto>();
            var balance = await _shopRepository.GetBalanceByShopId(shop.Id);
            var bal = new List<BalanceDto>();
            //var user = await _shopRepository.GetUserById(shop.Id);
            var us = new List<AspNetUser>();
            AspNetUser? user = null;
            List<string>? ownerRoles = null;

            if (shop.OwnerId.HasValue)
            {
                user = await _shopRepository.GetShopByUserId(shop.OwnerId.Value);
                if (user != null)
                {
                    ownerRoles = await GetRolesByUserId(user.Id);
                }
            }



            var res = new ShopDto
            {
                Id = shop.Id,
                Name = shop.Name,
                OwnerId = shop.OwnerId,
                IsActive = shop.IsActive,
                Slug = shop.Slug,
                Description = shop.Description,
                OrdersCount = await _shopRepository.GetOrdersCount(shop.Id),
                ProductCount = await _shopRepository.GetProductCount(shop.Id),
                Status = shop.IsActive,
                OwnerName = await _shopRepository.GetOwnerName(shop.OwnerId),
                CreatedAt = shop.CreatedAt,
                Logo = logoUrl,
                CoverImage = coverUrl,


                shopAddressDto = shopAddress != null ? new ShopAddressDto
                {
                    Id = shopAddress.Id,
                    StreetAddress = shopAddress.StreetAddress,
                    City = shopAddress.City,
                    State = shopAddress.State,
                    Zip = shopAddress.Zip,
                    Country = shopAddress.Country
                } : null ?? new ShopAddressDto(),
                settingDto = setting != null ? new SettingDto
                {
                    Contact = setting.Contact,
                    Website = setting.Website,
                    LocationLat = setting.LocationLat,
                    LocationLng = setting.LocationLng,
                    LocationZip = setting.LocationZip,
                    LocationCity = setting.LocationCity,
                    LocationState = setting.LocationState,
                    LocationCountry = setting.LocationCountry,
                    LocationFormattedAddress = setting.LocationFormattedAddress,
                } : null ?? new SettingDto(),
                socialDto = social != null ? new SocialDto
                {
                    Id = social.Id,
                    Link = social.Link,
                    Icon = social.Icon,
                    Url = social.Url,
                    Type = social.Type,

                } : null ?? new SocialDto(),

                balanceDto = balance != null ? new BalanceDto
                {
                    Id = balance.Id,
                    AdminCommissionRate = balance.AdminCommissionRate,
                    TotalEarnings = balance.TotalEarnings,
                    WithDrawnAmount = balance.WithDrawnAmount,
                    CurrentBalance = balance.CurrentBalance,
                    AccountNumber = balance.AccountNumber,
                    AccountHolderEmail = balance.AccountHolderEmail,
                    AccountHolderName = balance.AccountHolderName,
                    BankName = balance.BankName
                } : null ?? new BalanceDto(),
                OrderUserDto = user != null ? new OrderUserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    PhoneNumber = user.PhoneNumber,
                    IsActive = user.IsActive,
                    ShopId = user.Id,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                    Roles = ownerRoles != null ? string.Join(",", ownerRoles) : null

                } : null ?? new OrderUserDto(),
            };

            return _typeAdapter.Adapt<ShopDto>(res);
        }






        public async Task<ShopDto> UpdateShop(ShopDto shopDto)
        {
            try
            {
                var shop = await _shopRepository.GetShopById(shopDto.Id);
                if (shop == null)
                {
                    return null;
                }

                // Handle Cover Image Update
                if (shopDto.imageDto != null && !string.IsNullOrEmpty(shopDto.imageDto.CoverImage))
                {
                    var coverImage = new Image
                    {
                        OriginalUrl = shopDto.imageDto.CoverImage,
                        CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                        UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                    };

                    await _shopRepository.UpdateImage(coverImage);
                    await _shopRepository.UnitOfWork.SaveChangesAsync();
                    shop.CoverImageId = coverImage.Id;

                }

                // Handle Logo Image Update
                if (shopDto.imageDto != null && !string.IsNullOrEmpty(shopDto.imageDto.Logo))
                {
                    var logoImage = new Image
                    {
                        OriginalUrl = shopDto.imageDto.Logo,
                        CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                        UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                    };

                    await _shopRepository.UpdateImage(logoImage);
                    await _shopRepository.UnitOfWork.SaveChangesAsync();
                    shop.LogoImageId = logoImage.Id;
                }

                shop.Name = shopDto.Name;
                shop.Description = shopDto.Description;
                shop.Slug = shopDto.Slug;
                if (shopDto.OwnerId.HasValue)
                {
                    shop.OwnerId = shopDto.OwnerId.Value;
                }
                if (shopDto.IsActive.HasValue)
                {
                    shop.IsActive = shopDto.IsActive.Value;
                }

                var updatedShop = await _shopRepository.UpdateShop(shop);
                await _shopRepository.UnitOfWork.SaveChangesAsync();

                var address = new Address
                {
                    ShopId = updatedShop.Id,
                    Zip = shopDto.shopAddressDto!.Zip,
                    City = shopDto.shopAddressDto.City,
                    State = shopDto.shopAddressDto.State,
                    Country = shopDto.shopAddressDto.Country,
                    StreetAddress = shopDto.shopAddressDto.StreetAddress,
                };
                await _shopRepository.UpdateAddress(updatedShop.Id, address);
                await _shopRepository.UnitOfWork.SaveChangesAsync();

                var balance = new Balance
                {
                    ShopId = updatedShop.Id,
                    AdminCommissionRate = shopDto.balanceDto!.AdminCommissionRate,
                    Shop = shopDto.balanceDto.Shop,
                    TotalEarnings = shopDto.balanceDto.TotalEarnings,
                    WithDrawnAmount = shopDto.balanceDto.WithDrawnAmount,
                    CurrentBalance = shopDto.balanceDto.CurrentBalance,
                    AccountNumber = shopDto.balanceDto.AccountNumber,
                    AccountHolderEmail = shopDto.balanceDto.AccountHolderEmail,
                    AccountHolderName = shopDto.balanceDto.AccountHolderName,
                    BankName = shopDto.balanceDto.BankName,
                };
                await _shopRepository.UpdateBalance(balance);
                await _shopRepository.UnitOfWork.SaveChangesAsync();
                //var image = new Image
                //{
                //    //Id = updatedShop.Id,
                //    ThumbnailUrl = imageDto.ThumbnailUrl,
                //    OriginalUrl = imageDto.OriginalUrl,
                //    CreatedAt = imageDto.CreatedAt,
                //    UpdatedAt = imageDto.UpdatedAt,
                //};
                //await _shopRepository.UpdateImage(image);
                var social = await _shopRepository.GetSocialByShopId(updatedShop.Id);
                if (social == null)
                {
                    social = new Social
                    {
                        ShopId = updatedShop.Id,
                        Url = shopDto.socialDto.Url,
                        Icon = shopDto.socialDto.Icon,
                    };
                    await _shopRepository.AddSocial(social);
                }
                else
                {
                    social.Url = shopDto.socialDto.Url;
                    social.Icon = shopDto.socialDto.Icon;
                    await _shopRepository.UpdateSocial(social);
                }
                await _shopRepository.UnitOfWork.SaveChangesAsync();

                // Update shop setting
                var setting = await _shopRepository.GetSettingByShopId(updatedShop.Id);
                if (setting == null)
                {
                    setting = new Setting
                    {
                        ShopId = updatedShop.Id,
                        Contact = shopDto.settingDto.Contact,
                        Website = shopDto.settingDto.Website,
                        LocationLat = shopDto.settingDto.LocationLat,
                        LocationLng = shopDto.settingDto.LocationLng,
                        LocationCity = shopDto.settingDto.LocationCity,
                        LocationState = shopDto.settingDto.LocationState,
                        LocationZip = shopDto.settingDto.LocationZip,
                        LocationCountry = shopDto.settingDto.LocationCountry,
                        LocationFormattedAddress = shopDto.settingDto.LocationFormattedAddress,
                    };
                    await _shopRepository.AddSetting(setting);
                }
                else
                {
                    setting.Contact = shopDto.settingDto.Contact;
                    setting.Website = shopDto.settingDto.Website;
                    setting.LocationLat = shopDto.settingDto.LocationLat;
                    setting.LocationLng = shopDto.settingDto.LocationLng;
                    setting.LocationCity = shopDto.settingDto.LocationCity;
                    setting.LocationState = shopDto.settingDto.LocationState;
                    setting.LocationZip = shopDto.settingDto.LocationZip;
                    setting.LocationCountry = shopDto.settingDto.LocationCountry;
                    setting.LocationFormattedAddress = shopDto.settingDto.LocationFormattedAddress;
                    await _shopRepository.UpdateSetting(setting);
                }
                await _shopRepository.UnitOfWork.SaveChangesAsync();

                return _typeAdapter.Adapt<ShopDto>(updatedShop);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<string>> GetRolesByUserId(int userId)
        {
            var user = await _shopRepository.GetShopByUserRoleId(userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            var roles = user.AspNetUserRoles.Select(ur => ur.Role.Name).ToList();

            return roles;
        }



        public async Task<bool> DeleteShop(int id)
        {
            try
            {
                var result = await _shopRepository.DeleteShop(id);
                if (!result)
                {
                    return false;
                }
                await _shopRepository.DeleteAddresses(id);
                await _shopRepository.DeleteBalances(id);
                await _shopRepository.DeleteImages(id);
                await _shopRepository.DeleteSocials(id);
                await _shopRepository.DeleteSettings(id);
                await _shopRepository.DeleteOrders(id);
                await _shopRepository.DeleteCoupons(id);
                await _shopRepository.DeleteAttribute(id);

                await _shopRepository.UnitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<ShopDto> ApproveShop(int id)
        {
            var approvedShop = await _shopRepository.ApproveShop(id);
            await _shopRepository.UnitOfWork.SaveChangesAsync();
            return _typeAdapter.Adapt<ShopDto>(approvedShop);
        }

        public async Task<ShopDto> DisapproveShop(int id)
        {
            var disapprovedShop = await _shopRepository.DisapproveShop(id);
            await _shopRepository.UnitOfWork.SaveChangesAsync();
            return _typeAdapter.Adapt<ShopDto>(disapprovedShop);
        }



        public async Task<List<ShopDto>> GetShopsByOwnerIdAsync(int ownerId)
        {
            try
            {
                var shopDetails = await _shopRepository.GetShopsWithDetailsAsync();

                var result = shopDetails
                    .Where(detail => detail.Shop.OwnerId == ownerId)
                    .Select(detail => new ShopDto
                    {
                        Id = detail.Shop.Id,
                        Name = detail.Shop.Name,
                        OwnerId = detail.Shop.OwnerId,
                        Slug = detail.Shop.Slug,
                        Description = detail.Shop.Description,
                        IsActive = detail.Shop.IsActive,
                        OrdersCount = detail.OrdersCount,
                        ProductCount = detail.ProductCount,
                        Status = detail.Shop.IsActive,
                        OwnerName = detail.Owner?.UserName ?? string.Empty,
                        CreatedAt = detail.Shop.CreatedAt,
                        Logo = detail.LogoUrl,
                        CoverImage = detail.CoverUrl,

                        shopAddressDto = detail.ShopAddress != null ? new ShopAddressDto
                        {
                            Id = detail.ShopAddress.Id,
                            StreetAddress = detail.ShopAddress.StreetAddress,
                            City = detail.ShopAddress.City,
                            State = detail.ShopAddress.State,
                            Zip = detail.ShopAddress.Zip,
                            Country = detail.ShopAddress.Country
                        } : null,

                        settingDto = detail.Setting != null ? new SettingDto
                        {
                            Contact = detail.Setting.Contact,
                            Website = detail.Setting.Website,
                            LocationLat = detail.Setting.LocationLat,
                            LocationLng = detail.Setting.LocationLng,
                            LocationZip = detail.Setting.LocationZip,
                            LocationCity = detail.Setting.LocationCity,
                            LocationState = detail.Setting.LocationState,
                            LocationCountry = detail.Setting.LocationCountry,
                            LocationFormattedAddress = detail.Setting.LocationFormattedAddress,
                        } : null,

                        socialDto = detail.Social != null ? new SocialDto
                        {
                            Id = detail.Social.Id,
                            Link = detail.Social.Link,
                            Icon = detail.Social.Icon,
                            Url = detail.Social.Url,
                            Type = detail.Social.Type,
                        } : null,

                        balanceDto = detail.Balance != null ? new BalanceDto
                        {
                            Id = detail.Balance.Id,
                            ShopId = detail.Balance.ShopId,
                            AdminCommissionRate = detail.Balance.AdminCommissionRate,
                            TotalEarnings = detail.Balance.TotalEarnings,
                            WithDrawnAmount = detail.Balance.WithDrawnAmount,
                            CurrentBalance = detail.Balance.CurrentBalance,
                            AccountNumber = detail.Balance.AccountNumber,
                            AccountHolderEmail = detail.Balance.AccountHolderEmail,
                            AccountHolderName = detail.Balance.AccountHolderName,
                            BankName = detail.Balance.BankName
                        } : null,

                        OrderUserDto = detail.Owner != null ? new OrderUserDto
                        {
                            Id = detail.Owner.Id,
                            UserName = detail.Owner.UserName,
                            Email = detail.Owner.Email,
                            EmailConfirmed = detail.Owner.EmailConfirmed,
                            PhoneNumber = detail.Owner.PhoneNumber,
                            IsActive = detail.Owner.IsActive,
                            ShopId = detail.Shop.Id,
                            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                            Roles = detail.Roles != null ? string.Join(",", detail.Roles) : null
                        } : null
                    }).ToList();

                return _typeAdapter.Adapt<List<ShopDto>>(result);
            }
            catch (Exception ex)
            {
                // Consider logging the exception
                throw new Exception("An error occurred while fetching shops.", ex);
            }
        }




        private string GenerateSlug(string name)
        {
            return name.ToLower().Replace(" ", "-");
        }

        private async Task<string> EnsureUniqueSlugAsync(string name, string requestedSlug)
        {
            string slug = GenerateSlug(requestedSlug);

            if (!await _shopRepository.SlugExistsAsync(slug))
            {
                return slug;
            }
            int counter = 1;
            string originalSlug = slug;

            while (await _shopRepository.SlugExistsAsync(slug))
            {
                slug = $"{originalSlug}-{counter}";
                counter++;
            }
            return slug;
        }

    }
}
