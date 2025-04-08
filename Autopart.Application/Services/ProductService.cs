using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Application.Models.Dto;
using Autopart.Application.Models.Products;
using Autopart.Application.Options;
using Autopart.Domain.CommonDTO;
using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using static Autopart.Domain.Enum.EnumAndConsonent;

namespace Autopart.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _context;
        private readonly IManufacturersRepository _manufacturerRepository;
        private readonly IEngineRepository _engineRepository;
        private readonly ISubCategoryListRepository _subCategoryListRepository;
        private readonly IManufactureModelRepository _manufactureModelRepository;
        private readonly IShopRepository _shopRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITypeAdapter _typeAdapter;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly RootFolder _rootFolder;

        public ProductService(IProductRepository context, IEngineRepository engineRepository, ISubCategoryListRepository subCategoryListRepository, IManufactureModelRepository manufactureModelRepository, IManufacturersRepository manufacturerRepository, ICategoryRepository categoryRepository, IShopRepository shopRepository, ITypeAdapter typeAdapter, IOptions<RootFolder> rootFolder, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _manufacturerRepository = manufacturerRepository;
            _engineRepository = engineRepository;
            _subCategoryListRepository = subCategoryListRepository;
            _manufactureModelRepository = manufactureModelRepository;
            _shopRepository = shopRepository;
            _categoryRepository = categoryRepository;
            _typeAdapter = typeAdapter;
            _rootFolder = rootFolder.Value;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IEnumerable<ProductDtoResponse>> GetWishlistProductsAsync(int userId)
        {
            var wishlistProducts = await _context.GetWishlistProductsByUserIdAsync(userId);

            var result = new List<ProductDtoResponse>();

            foreach (var product in wishlistProducts)
            {
                var category = product.CategoryId.HasValue
                    ? await _context.GetCategoryByIdAsync(product.CategoryId.Value)
                    : null;

                var shopAddress = product.ShopId.HasValue
                    ? await _context.GetShopAddressById(product.ShopId.Value)
                    : null;

                var shop = product.ShopId.HasValue
                    ? await _context.GetShopByProductId(product.ShopId.Value)
                    : null;

                var setting = await _context.GetSettingByProductId(product.Id);
                var rating = await _context.GetRatingByProductId(product.Id);
                var gallery = await _context.GetGalleryByProductId(product.Id);
                var promotional = await _context.GetPromotionalByProductId(product.Id);

                var image = product.Image;
                // Fetch the shipping charges based on the product's category size and Shop ID

                decimal shippingCharges = 0;
                if (category != null && product.ShopId.HasValue)
                {
                    var svcRelation = await _context.GetSVCRelationByShopIdAndSizeAsync(product.ShopId.Value, category.Size);
                    if (svcRelation != null)
                    {
                        shippingCharges = svcRelation.Price ?? 0;
                    }
                }

                // Calculate total price
                decimal totalPrice = (product.Price ?? 0) + shippingCharges;

                var shopAddressDto = shopAddress != null
                    ? new ShopAddressDto
                    {
                        Id = shopAddress.Id,
                        Country = shopAddress.Country,
                        City = shopAddress.City,
                        State = shopAddress.State,
                        Zip = shopAddress.Zip,
                        StreetAddress = shopAddress.StreetAddress,
                        ShopId = shopAddress.ShopId
                    }
                    : null;

                var ratingDto = rating != null
                    ? new RatingDto
                    {
                        Rating1 = rating.Rating1,
                        PositiveFeedbacksCount = rating.PositiveFeedbacksCount,
                        NegativeFeedbacksCount = rating.NegativeFeedbacksCount,
                        AbusiveReportsCount = rating.AbusiveReportsCount,
                        Total = rating.Total
                    }
                    : null;

                var categoryDto = category != null
                    ? new CategoryDto
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Slug = category.Slug,
                        Icon = category.Icon,
                        ParentId = category.ParentId,
                        Language = category.Language,
                        CreatedAt = category.CreatedAt,
                        UpdatedAt = category.UpdatedAt,
                        DeletedAt = category.DeletedAt,
                        TypeId = category.TypeId
                    }
                    : null;

                var settingDto = setting != null
                    ? new SettingDto
                    {
                        Contact = setting.Contact,
                        Website = setting.Website,
                        LocationLat = setting.LocationLat,
                        LocationLng = setting.LocationLng,
                        LocationCity = setting.LocationCity,
                        LocationState = setting.LocationState,
                        LocationCountry = setting.LocationCountry,
                        LocationFormattedAddress = setting.LocationFormattedAddress,
                        LocationZip = setting.LocationZip
                    }
                    : null;

                string logoUrl = null!;
                if (shop != null)
                {
                    if (shop.LogoImageId.HasValue)
                    {
                        var logo = await _shopRepository.GetImageById(shop.LogoImageId.Value);
                        if (logo != null && _rootFolder != null && !string.IsNullOrEmpty(_rootFolder.ApplicationUrl))
                        {
                            logoUrl = logo;
                        }
                    }
                }

                var shopDto = shop != null
                    ? new ShopDto
                    {
                        Id = shop.Id,
                        Name = shop.Name,
                        Slug = shop.Slug,
                        Description = shop.Description,
                        IsActive = shop.IsActive,
                        Logo = logoUrl,
                        OwnerId = shop.OwnerId
                    }
                    : null;

                var promotionalSliderDto = promotional != null
                    ? new PromotionalSliderDto
                    {
                        Id = promotional.Id,
                        OriginalUrl = promotional.OriginalUrl,
                        ThumbnailUrl = promotional.ThumbnailUrl
                    }
                    : null;

                var productGalleryImageDto = gallery != null
                    ? new ProductGalleryImageDto
                    {
                        Id = gallery.Id,
                        OriginalUrl = gallery.OriginalUrl,
                        ThumbnailUrl = gallery.ThumbnailUrl
                    }
                    : null;

                var imageDto = image != null
                        ? new ImageDto
                        {
                            Id = image.Id,
                            OriginalUrl = image.OriginalUrl,
                            ThumbnailUrl = image.ThumbnailUrl
                        }
                        : null;

                var res = new ProductDtoResponse
                {
                    Id = product.Id,
                    Name = product.Name,
                    Slug = product.Slug,
                    Description = product.Description,
                    SubPrice = product.Price,
                    TypeId = product.TypeId,
                    ShopId = product.ShopId,
                    SalePrice = product.SalePrice,
                    Language = product.Language,
                    MaxPrice = product.MaxPrice,
                    MinPrice = product.MinPrice,
                    Quantity = product.Quantity,
                    Sku = product.Sku,
                    InStock = product.InStock,
                    IsTaxable = product.IsTaxable,
                    ShippingClassId = product.ShippingClassId,
                    Status = product.Status,
                    ProductType = product.ProductType,
                    Unit = product.Unit,
                    Height = product.Height,
                    Width = product.Width,
                    Length = product.Length,
                    DeletedAt = product.DeletedAt,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt,
                    TotalReviews = product.TotalReviews,
                    ManufacturerId = product.ManufacturerId,
                    IsDigital = product.IsDigital,
                    ExternalProductButtonText = product.ExternalProductButtonText,
                    ExternalProductUrl = product.ExternalProductUrl,
                    IsExternal = product.IsExternal,
                    Ratings = product.Ratings,
                    MyReview = product.MyReview,
                    ImageId = product.ImageId,
                    AuthorId = product.AuthorId,
                    InWishlist = product.InWishlist,
                    Year = product.Year,
                    Model = product.Model,
                    Mileage = product.Mileage,
                    Grade = product.Grade,
                    Damage = product.Damage,
                    TrimLevel = product.TrimLevel,
                    EngineId = product.EngineId,
                    Transmission = product.Transmission,
                    Drivetrain = product.Drivetrain,
                    NewUsed = product.NewUsed,
                    OemPartNumber = product.OemPartNumber,
                    PartslinkNumber = product.PartslinkNumber,
                    HollanderIc = product.HollanderIc,
                    StockNumber = product.StockNumber,
                    TagNumber = product.TagNumber,
                    Location = product.Location,
                    Site = product.Site,
                    Vin = product.Vin,
                    Core = product.Core,
                    Color = product.Color,
                    ShippingCharges = shippingCharges,
                    Price = totalPrice,


                    ImageDto = imageDto,
                    SettingDto = settingDto!,
                    ShopAddressDto = shopAddressDto,
                    CategoryDto = categoryDto!,
                    PromotionalSliderDto = promotionalSliderDto,
                    ShopDto = shopDto,
                    ProductGalleryImageDto = productGalleryImageDto,
                    RatingDto = ratingDto
                };

                result.Add(res);
            }

            return result;
        }



        public async Task<bool> IsProductInWishlistAsync(int userId, int productId)
        {
            var wishlistProduct = await _context.GetWishlistProductAsync(userId, productId);
            return wishlistProduct != null;
        }

        public async Task<IEnumerable<ProductDtoResponse>> GetWishlistProductsByProductIdAsync(int productId)
        {
            var wishlistProducts = await _context.GetWishlistProductsByProductIdAsync(productId);

            var result = new List<ProductDtoResponse>();

            foreach (var product in wishlistProducts)
            {
                var category = product.CategoryId.HasValue
                    ? await _context.GetCategoryByIdAsync(product.CategoryId.Value)
                    : null;

                var shopAddress = product.ShopId.HasValue
                    ? await _context.GetShopAddressById(product.ShopId.Value)
                    : null;

                var shop = product.ShopId.HasValue
                    ? await _context.GetShopByProductId(product.ShopId.Value)
                    : null;

                var setting = await _context.GetSettingByProductId(product.Id);
                var rating = await _context.GetRatingByProductId(product.Id);
                var gallery = await _context.GetGalleryByProductId(product.Id);
                var promotional = await _context.GetPromotionalByProductId(product.Id);

                var image = product.Image;
                // Fetch the shipping charges based on the product's category size and shop ID
                decimal shippingCharges = 0;
                if (category != null && product.ShopId.HasValue)
                {
                    var svcRelation = await _context.GetSVCRelationByShopIdAndSizeAsync(product.ShopId.Value, category.Size);
                    if (svcRelation != null)
                    {
                        shippingCharges = svcRelation.Price ?? 0;
                    }
                }
                decimal totalPrice = (product.Price ?? 0) + shippingCharges;

                var shopAddressDto = shopAddress != null
                    ? new ShopAddressDto
                    {
                        Id = shopAddress.Id,
                        Country = shopAddress.Country,
                        City = shopAddress.City,
                        State = shopAddress.State,
                        Zip = shopAddress.Zip,
                        StreetAddress = shopAddress.StreetAddress,
                        ShopId = shopAddress.ShopId
                    }
                    : null;

                var ratingDto = rating != null
                    ? new RatingDto
                    {
                        Rating1 = rating.Rating1,
                        PositiveFeedbacksCount = rating.PositiveFeedbacksCount,
                        NegativeFeedbacksCount = rating.NegativeFeedbacksCount,
                        AbusiveReportsCount = rating.AbusiveReportsCount,
                        Total = rating.Total
                    }
                    : null;

                var categoryDto = category != null
                    ? new CategoryDto
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Slug = category.Slug,
                        Icon = category.Icon,
                        ParentId = category.ParentId,
                        Language = category.Language,
                        CreatedAt = category.CreatedAt,
                        UpdatedAt = category.UpdatedAt,
                        DeletedAt = category.DeletedAt,
                        TypeId = category.TypeId
                    }
                    : null;

                var settingDto = setting != null
                    ? new SettingDto
                    {
                        Contact = setting.Contact,
                        Website = setting.Website,
                        LocationLat = setting.LocationLat,
                        LocationLng = setting.LocationLng,
                        LocationCity = setting.LocationCity,
                        LocationState = setting.LocationState,
                        LocationCountry = setting.LocationCountry,
                        LocationFormattedAddress = setting.LocationFormattedAddress,
                        LocationZip = setting.LocationZip
                    }
                    : null;

                string logoUrl = null!;
                if (shop != null)
                {
                    if (shop.LogoImageId.HasValue)
                    {
                        var logo = await _shopRepository.GetImageById(shop.LogoImageId.Value);
                        if (logo != null && _rootFolder != null && !string.IsNullOrEmpty(_rootFolder.ApplicationUrl))
                        {
                            logoUrl = _rootFolder.ApplicationUrl + logo;
                        }
                    }
                }

                var shopDto = shop != null
                    ? new ShopDto
                    {
                        Id = shop.Id,
                        Name = shop.Name,
                        Slug = shop.Slug,
                        Description = shop.Description,
                        IsActive = shop.IsActive,
                        Logo = logoUrl,
                        OwnerId = shop.OwnerId
                    }
                    : null;

                var promotionalSliderDto = promotional != null
                    ? new PromotionalSliderDto
                    {
                        Id = promotional.Id,
                        OriginalUrl = promotional.OriginalUrl,
                        ThumbnailUrl = promotional.ThumbnailUrl
                    }
                    : null;

                var productGalleryImageDto = gallery != null
                    ? new ProductGalleryImageDto
                    {
                        Id = gallery.Id,
                        OriginalUrl = gallery.OriginalUrl,
                        ThumbnailUrl = gallery.ThumbnailUrl
                    }
                    : null;

                var imageDto = image != null
                            ? new ImageDto
                            {
                                Id = image.Id,
                                OriginalUrl = image.OriginalUrl,
                                ThumbnailUrl = image.ThumbnailUrl
                            }
                            : null;

                var res = new ProductDtoResponse
                {
                    Id = product.Id,
                    Name = product.Name,
                    Slug = product.Slug,
                    Description = product.Description,
                    SubPrice = product.Price,
                    TypeId = product.TypeId,
                    ShopId = product.ShopId,
                    SalePrice = product.SalePrice,
                    Language = product.Language,
                    MaxPrice = product.MaxPrice,
                    MinPrice = product.MinPrice,
                    Quantity = product.Quantity,
                    Sku = product.Sku,
                    InStock = product.InStock,
                    IsTaxable = product.IsTaxable,
                    ShippingClassId = product.ShippingClassId,
                    Status = product.Status,
                    ProductType = product.ProductType,
                    Unit = product.Unit,
                    Height = product.Height,
                    Width = product.Width,
                    Length = product.Length,
                    DeletedAt = product.DeletedAt,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt,
                    TotalReviews = product.TotalReviews,
                    ManufacturerId = product.ManufacturerId,
                    IsDigital = product.IsDigital,
                    ExternalProductButtonText = product.ExternalProductButtonText,
                    ExternalProductUrl = product.ExternalProductUrl,
                    IsExternal = product.IsExternal,
                    Ratings = product.Ratings,
                    MyReview = product.MyReview,
                    ImageId = product.ImageId,
                    AuthorId = product.AuthorId,
                    InWishlist = product.InWishlist,
                    Year = product.Year,
                    Model = product.Model,
                    Mileage = product.Mileage,
                    Grade = product.Grade,
                    Damage = product.Damage,
                    TrimLevel = product.TrimLevel,
                    EngineId = product.EngineId,
                    Transmission = product.Transmission,
                    Drivetrain = product.Drivetrain,
                    NewUsed = product.NewUsed,
                    OemPartNumber = product.OemPartNumber,
                    PartslinkNumber = product.PartslinkNumber,
                    HollanderIc = product.HollanderIc,
                    StockNumber = product.StockNumber,
                    TagNumber = product.TagNumber,
                    Location = product.Location,
                    Site = product.Site,
                    Vin = product.Vin,
                    Core = product.Core,
                    Color = product.Color,
                    ShippingCharges = shippingCharges,
                    Price = totalPrice,

                    ImageDto = imageDto,
                    SettingDto = settingDto!,
                    ShopAddressDto = shopAddressDto,
                    CategoryDto = categoryDto!,
                    PromotionalSliderDto = promotionalSliderDto,
                    ShopDto = shopDto,
                    ProductGalleryImageDto = productGalleryImageDto,
                    RatingDto = ratingDto
                };

                result.Add(res);
            }

            return result;
        }


        public async Task AddProductToWishlistAsync(int userId, int productId)
        {
            await _context.AddProductToWishlistAsync(userId, productId);
        }

        public async Task<bool> DeleteWishlistProductsByProductIdAsync(int productId)
        {
            return await _context.DeleteWishlistProductsByProductIdAsync(productId);
        }

        public async Task<(IEnumerable<ProductDtoResponse> Products, int TotalCount)> GetProductsAsync(GetProductsDto getProductsDto)
        {
            var (products, totalCount) = await _context.GetProductsAsync(getProductsDto);

            var result = products.Select(q => new ProductDtoResponse
            {
                Id = q.Product.Id,
                Name = q.Product.Name,
                Slug = q.Product.Slug,
                Description = q.Product.Description,
                SubPrice = q.Product.Price,
                ShippingCharges = q.Category != null && q.Product.ShopId.HasValue
                    ? (_context.GetSVCRelationByShopIdAndSizeAsync(q.Product.ShopId.Value, q.Category.Size).Result?.Price ?? 0)
                    : 0,
                Price = (q.Product.Price ?? 0) + (q.Category != null && q.Product.ShopId.HasValue
                    ? (_context.GetSVCRelationByShopIdAndSizeAsync(q.Product.ShopId.Value, q.Category.Size).Result?.Price ?? 0)
                    : 0),
                ShopId = q.Product.ShopId,
                Model = q.Product.Model,
                IsDigital = q.Product.IsDigital,
                CreatedAt = q.Product.CreatedAt,
                UpdatedAt = q.Product.UpdatedAt,
                DeletedAt = q.Product.DeletedAt,
                IsExternal = q.Product.IsExternal,
                ExternalProductUrl = q.Product.ExternalProductUrl,
                ExternalProductButtonText = q.Product.ExternalProductButtonText,
                Ratings = q.Rating != null ? q.Rating.Total : 0,
                TotalReviews = q.Product.TotalReviews ?? 0,
                MyReview = q.Product.MyReview,
                IsTaxable = q.Product.IsTaxable,
                ShippingClassId = q.Product.ShippingClassId,
                Status = q.Product.Status,
                ProductType = q.Product.ProductType,
                Unit = q.Product.Unit,
                Height = q.Product.Height,
                Width = q.Product.Width,
                Length = q.Product.Length,
                ImageId = q.Product.ImageId,
                InWishlist = q.Product.InWishlist,
                SalePrice = q.Product.SalePrice,
                MinPrice = q.Product.MinPrice ?? 0,
                MaxPrice = q.Product.MaxPrice ?? 0,
                Quantity = q.Product.Quantity ?? 0,
                Sku = q.Product.Sku,
                TypeId = q.Product.TypeId,
                AuthorId = q.Product.AuthorId,
                ManufacturerId = q.Product.ManufacturerId,
                Language = q.Product.Language,
                InStock = q.Product.InStock,
                Year = q.Product.Year,
                Mdoel = q.Product.Model,
                Mileage = q.Product.Mileage,
                Grade = q.Product.Grade,
                Damage = q.Product.Damage,
                TrimLevel = q.Product.TrimLevel,
                EngineId = q.Product.EngineId,
                ModelId = q.Product.ModelId,
                Transmission = q.Product.Transmission,
                Drivetrain = q.Product.Drivetrain,
                NewUsed = q.Product.NewUsed,
                OemPartNumber = q.Product.OemPartNumber,
                PartslinkNumber = q.Product.PartslinkNumber,
                HollanderIc = q.Product.HollanderIc,
                StockNumber = q.Product.StockNumber,
                TagNumber = q.Product.TagNumber,
                Location = q.Product.Location,
                Site = q.Product.Site,
                Vin = q.Product.Vin,
                Core = q.Product.Core,
                Color = q.Product.Color,

                CategoryDto = q.Category != null ? new CategoryDto
                {
                    Id = q.Category.Id,
                    Name = q.Category.Name,
                    Slug = q.Category.Slug,
                    Language = q.Category.Language,
                    Icon = q.Category.Icon,
                    CreatedAt = q.Category.CreatedAt,
                    UpdatedAt = q.Category.UpdatedAt
                } : null ?? new CategoryDto(),

                ImageDto = q.Product.Image != null ? new ImageDto
                {
                    Id = q.Product.Image.Id,
                    OriginalUrl = q.Product.Image.OriginalUrl,
                    ThumbnailUrl = q.Product.Image.ThumbnailUrl
                } : null ?? new ImageDto(),

                RatingDto = q.Rating != null ? new RatingDto
                {
                    Rating1 = q.Rating.Rating1,
                    PositiveFeedbacksCount = q.Rating.PositiveFeedbacksCount,
                    NegativeFeedbacksCount = q.Rating.NegativeFeedbacksCount,
                    AbusiveReportsCount = q.Rating.AbusiveReportsCount,
                    Total = q.Rating.Total
                } : null ?? new RatingDto(),

                ShopDto = q.Shop != null ? new ShopDto
                {
                    Id = q.Shop.Id,
                    Name = q.Shop.Name,
                    Slug = q.Shop.Slug,
                    Description = q.Shop.Description,
                    IsActive = q.Shop.IsActive,
                    OwnerId = q.Shop.OwnerId,
                    Logo = q.LogoImage != null ? q.LogoImage.OriginalUrl : null!,
                    CoverImage = q.CoverImage != null ? q.CoverImage.ThumbnailUrl : null!,
                } : null ?? new ShopDto(),


                PromotionalSliderDto = q.Promotional != null ? new PromotionalSliderDto
                {
                    Id = q.Promotional.Id,
                    OriginalUrl = q.Promotional.OriginalUrl,
                    ThumbnailUrl = q.Promotional.ThumbnailUrl
                } : null ?? new PromotionalSliderDto(),

                ProductGalleryImageDto = q.Gallery != null ? new ProductGalleryImageDto
                {
                    Id = q.Gallery.Id,
                    OriginalUrl = q.Gallery.OriginalUrl,
                    ThumbnailUrl = q.Gallery.ThumbnailUrl
                } : null ?? new ProductGalleryImageDto(),

                SettingDto = q.Shop?.Setting != null ? new SettingDto
                {
                    Contact = q.Shop.Setting.Contact,
                    Website = q.Shop.Setting.Website,
                    LocationLat = q.Shop.Setting.LocationLat,
                    LocationLng = q.Shop.Setting.LocationLng,
                    LocationCity = q.Shop.Setting.LocationCity,
                    LocationState = q.Shop.Setting.LocationState,
                    LocationCountry = q.Shop.Setting.LocationCountry,
                    LocationFormattedAddress = q.Shop.Setting.LocationFormattedAddress,
                    LocationZip = q.Shop.Setting.LocationZip
                } : null ?? new SettingDto(),

                ShopAddressDto = q.Shop?.Addresses != null && q.Shop.Addresses.Any() ? new ShopAddressDto
                {
                    Id = q.Shop.Addresses.FirstOrDefault()!.Id,
                    Country = q.Shop.Addresses.FirstOrDefault()!.Country,
                    City = q.Shop.Addresses.FirstOrDefault()!.City,
                    State = q.Shop.Addresses.FirstOrDefault()!.State,
                    Zip = q.Shop.Addresses.FirstOrDefault()!.Zip,
                    StreetAddress = q.Shop.Addresses.FirstOrDefault()!.StreetAddress,
                    ShopId = q.Shop.Id
                } : null ?? new ShopAddressDto()

            }).ToList();

            return (result, totalCount);
        }


            

        public async Task<List<CategoryProductCountDto>> GetShopsWithCategoryProductCountsAsync(GetProductsDto getProductsDto)
        {
            var shops = await _context.GetShopsWithCategoriesAndProductsAsync();
            var products = await _context.GetProductsAsync(getProductsDto);
            var categories = await _context.GetCategorieAsync();

            var result = (from shop in shops
                          join product in products.Products
                          on shop.Id equals product.Product.ShopId
                          join cat in categories
                          on product.Product.CategoryId equals cat.Id
                          group new { Category = cat, Shop = shop } by new
                          {
                              CategoryId = cat.Id,
                              CategoryName = cat.Name,
                              ShopId = shop.Id,
                              ShopName = shop.Name
                          } into g
                          select new CategoryProductCountDto
                          {
                              CategoryId = g.Key.CategoryId,
                              CategoryName = g.Key.CategoryName,
                              ShopName = g.Key.ShopName,
                              ProductCount = g.Count()
                          }).ToList();

            return result;
        }




        public async Task<IEnumerable<ProductDtoResponse>> GetLowStockProductsAsync(int lowStockThreshold)
        {
            var products = await _context.GetProductsBelowStockAsync(lowStockThreshold);
            var finalresult = new List<ProductDtoResponse>();

            var result = products.Select(q => new ProductDtoResponse
            {
                Id = q.Product.Id,
                Name = q.Product.Name,
                Slug = q.Product.Slug,
                Description = q.Product.Description,
                SubPrice = q.Product.Price,
                ShippingCharges = q.Category != null && q.Product.ShopId.HasValue
                    ? (_context.GetSVCRelationByShopIdAndSizeAsync(q.Product.ShopId.Value, q.Category.Size).Result?.Price ?? 0)
                    : 0,
                Price = (q.Product.Price ?? 0) + (q.Category != null && q.Product.ShopId.HasValue
                    ? (_context.GetSVCRelationByShopIdAndSizeAsync(q.Product.ShopId.Value, q.Category.Size).Result?.Price ?? 0)
                    : 0),
                ShopId = q.Product.ShopId,
                Model = q.Product.Model,
                IsDigital = q.Product.IsDigital,
                CreatedAt = q.Product.CreatedAt,
                UpdatedAt = q.Product.UpdatedAt,
                DeletedAt = q.Product.DeletedAt,
                IsExternal = q.Product.IsExternal,
                ExternalProductUrl = q.Product.ExternalProductUrl,
                ExternalProductButtonText = q.Product.ExternalProductButtonText,
                Ratings = q.Rating?.Total ?? 0,
                TotalReviews = q.Product.TotalReviews ?? 0,
                MyReview = q.Product.MyReview,
                IsTaxable = q.Product.IsTaxable,
                ShippingClassId = q.Product.ShippingClassId,
                Status = q.Product.Status,
                ProductType = q.Product.ProductType,
                Unit = q.Product.Unit,
                Height = q.Product.Height,
                Width = q.Product.Width,
                Length = q.Product.Length,
                ImageId = q.Product.ImageId,
                InWishlist = q.Product.InWishlist,
                SalePrice = q.Product.SalePrice,
                MinPrice = q.Product.MinPrice ?? 0,
                MaxPrice = q.Product.MaxPrice ?? 0,
                Quantity = q.Product.Quantity ?? 0,
                Sku = q.Product.Sku,
                TypeId = q.Product.TypeId,
                AuthorId = q.Product.AuthorId,
                ManufacturerId = q.Product.ManufacturerId,
                Language = q.Product.Language,
                InStock = q.Product.InStock,
                Year = q.Product.Year,
                Mdoel = q.Product.Model,
                Mileage = q.Product.Mileage,
                Grade = q.Product.Grade,
                Damage = q.Product.Damage,
                TrimLevel = q.Product.TrimLevel,
                EngineId = q.Product.EngineId,
                ModelId = q.Product.ModelId,
                Transmission = q.Product.Transmission,
                Drivetrain = q.Product.Drivetrain,
                NewUsed = q.Product.NewUsed,
                OemPartNumber = q.Product.OemPartNumber,
                PartslinkNumber = q.Product.PartslinkNumber,
                HollanderIc = q.Product.HollanderIc,
                StockNumber = q.Product.StockNumber,
                TagNumber = q.Product.TagNumber,
                Location = q.Product.Location,
                Site = q.Product.Site,
                Vin = q.Product.Vin,
                Core = q.Product.Core,
                Color = q.Product.Color,

                CategoryDto = q.Category != null ? new CategoryDto
                {
                    Id = q.Category.Id,
                    Name = q.Category.Name,
                    Slug = q.Category.Slug,
                    Language = q.Category.Language,
                    Icon = q.Category.Icon,
                    CreatedAt = q.Category.CreatedAt,
                    UpdatedAt = q.Category.UpdatedAt
                } : new CategoryDto(),

                ImageDto = q.Product.Image != null ? new ImageDto
                {
                    Id = q.Product.Image.Id,
                    OriginalUrl = q.Product.Image.OriginalUrl,
                    ThumbnailUrl = q.Product.Image.ThumbnailUrl
                } : new ImageDto(),

                RatingDto = q.Rating != null ? new RatingDto
                {
                    Rating1 = q.Rating.Rating1,
                    PositiveFeedbacksCount = q.Rating.PositiveFeedbacksCount,
                    NegativeFeedbacksCount = q.Rating.NegativeFeedbacksCount,
                    AbusiveReportsCount = q.Rating.AbusiveReportsCount,
                    Total = q.Rating.Total
                } : new RatingDto(),

                ShopDto = q.Shop != null ? new ShopDto
                {
                    Id = q.Shop.Id,
                    Name = q.Shop.Name,
                    Slug = q.Shop.Slug,
                    Description = q.Shop.Description,
                    IsActive = q.Shop.IsActive,
                    OwnerId = q.Shop.OwnerId,
                    Logo = q.LogoImage?.OriginalUrl!,
                    CoverImage = q.CoverImage?.ThumbnailUrl!
                } : new ShopDto(),

                PromotionalSliderDto = q.Promotional != null ? new PromotionalSliderDto
                {
                    Id = q.Promotional.Id,
                    OriginalUrl = q.Promotional.OriginalUrl,
                    ThumbnailUrl = q.Promotional.ThumbnailUrl
                } : new PromotionalSliderDto(),

                ProductGalleryImageDto = q.Gallery != null ? new ProductGalleryImageDto
                {
                    Id = q.Gallery.Id,
                    OriginalUrl = q.Gallery.OriginalUrl,
                    ThumbnailUrl = q.Gallery.ThumbnailUrl
                } : new ProductGalleryImageDto(),

                SettingDto = q.Shop?.Setting != null ? new SettingDto
                {
                    Contact = q.Shop.Setting.Contact,
                    Website = q.Shop.Setting.Website,
                    LocationLat = q.Shop.Setting.LocationLat,
                    LocationLng = q.Shop.Setting.LocationLng,
                    LocationCity = q.Shop.Setting.LocationCity,
                    LocationState = q.Shop.Setting.LocationState,
                    LocationCountry = q.Shop.Setting.LocationCountry,
                    LocationFormattedAddress = q.Shop.Setting.LocationFormattedAddress,
                    LocationZip = q.Shop.Setting.LocationZip
                } : new SettingDto(),

                ShopAddressDto = q.Shop?.Addresses?.FirstOrDefault() != null ? new ShopAddressDto
                {
                    Id = q.Shop.Addresses.FirstOrDefault()!.Id,
                    Country = q.Shop.Addresses.FirstOrDefault()!.Country,
                    City = q.Shop.Addresses.FirstOrDefault()!.City,
                    State = q.Shop.Addresses.FirstOrDefault()!.State,
                    Zip = q.Shop.Addresses.FirstOrDefault()!.Zip,
                    StreetAddress = q.Shop.Addresses.FirstOrDefault()!.StreetAddress,
                    ShopId = q.Shop.Id
                } : new ShopAddressDto()

            }).ToList();

            return finalresult;
        }


        public async Task<IEnumerable<ProductDtoResponse>> GetProductsByShopIdAsync(int shopId)
        {
            var products = await _context.GetProductsByShopId(shopId);
            var finalresult = new List<ProductDtoResponse>();
            var result = products.Select(q => new ProductDtoResponse
            {
                Id = q.Product.Id,
                Name = q.Product.Name,
                Slug = q.Product.Slug,
                Description = q.Product.Description,
                SubPrice = q.Product.Price,
                ShippingCharges = q.Category != null && q.Product.ShopId.HasValue
                    ? (_context.GetSVCRelationByShopIdAndSizeAsync(q.Product.ShopId.Value, q.Category.Size).Result?.Price ?? 0)
                    : 0,
                Price = (q.Product.Price ?? 0) + (q.Category != null && q.Product.ShopId.HasValue
                    ? (_context.GetSVCRelationByShopIdAndSizeAsync(q.Product.ShopId.Value, q.Category.Size).Result?.Price ?? 0)
                    : 0),
                ShopId = q.Product.ShopId,
                Model = q.Product.Model,
                IsDigital = q.Product.IsDigital,
                CreatedAt = q.Product.CreatedAt,
                UpdatedAt = q.Product.UpdatedAt,
                DeletedAt = q.Product.DeletedAt,
                IsExternal = q.Product.IsExternal,
                ExternalProductUrl = q.Product.ExternalProductUrl,
                ExternalProductButtonText = q.Product.ExternalProductButtonText,
                Ratings = q.Rating != null ? q.Rating.Total : 0,
                TotalReviews = q.Product.TotalReviews ?? 0,
                MyReview = q.Product.MyReview,
                IsTaxable = q.Product.IsTaxable,
                ShippingClassId = q.Product.ShippingClassId,
                Status = q.Product.Status,
                ProductType = q.Product.ProductType,
                Unit = q.Product.Unit,
                Height = q.Product.Height,
                Width = q.Product.Width,
                Length = q.Product.Length,
                ImageId = q.Product.ImageId,
                InWishlist = q.Product.InWishlist,
                SalePrice = q.Product.SalePrice,
                MinPrice = q.Product.MinPrice ?? 0,
                MaxPrice = q.Product.MaxPrice ?? 0,
                Quantity = q.Product.Quantity ?? 0,
                Sku = q.Product.Sku,
                TypeId = q.Product.TypeId,
                AuthorId = q.Product.AuthorId,
                ManufacturerId = q.Product.ManufacturerId,
                Language = q.Product.Language,
                InStock = q.Product.InStock,
                Year = q.Product.Year,
                Mdoel = q.Product.Model,
                Mileage = q.Product.Mileage,
                Grade = q.Product.Grade,
                Damage = q.Product.Damage,
                TrimLevel = q.Product.TrimLevel,
                EngineId = q.Product.EngineId,
                ModelId = q.Product.ModelId,
                Transmission = q.Product.Transmission,
                Drivetrain = q.Product.Drivetrain,
                NewUsed = q.Product.NewUsed,
                OemPartNumber = q.Product.OemPartNumber,
                PartslinkNumber = q.Product.PartslinkNumber,
                HollanderIc = q.Product.HollanderIc,
                StockNumber = q.Product.StockNumber,
                TagNumber = q.Product.TagNumber,
                Location = q.Product.Location,
                Site = q.Product.Site,
                Vin = q.Product.Vin,
                Core = q.Product.Core,
                Color = q.Product.Color,

                CategoryDto = q.Category != null ? new CategoryDto
                {
                    Id = q.Category.Id,
                    Name = q.Category.Name,
                    Slug = q.Category.Slug,
                    Language = q.Category.Language,
                    Icon = q.Category.Icon,
                    CreatedAt = q.Category.CreatedAt,
                    UpdatedAt = q.Category.UpdatedAt
                } : null ?? new CategoryDto(),

                ImageDto = q.Product.Image != null ? new ImageDto
                {
                    Id = q.Product.Image.Id,
                    OriginalUrl = q.Product.Image.OriginalUrl,
                    ThumbnailUrl = q.Product.Image.ThumbnailUrl
                } : null ?? new ImageDto(),

                RatingDto = q.Rating != null ? new RatingDto
                {
                    Rating1 = q.Rating.Rating1,
                    PositiveFeedbacksCount = q.Rating.PositiveFeedbacksCount,
                    NegativeFeedbacksCount = q.Rating.NegativeFeedbacksCount,
                    AbusiveReportsCount = q.Rating.AbusiveReportsCount,
                    Total = q.Rating.Total
                } : null ?? new RatingDto(),

                ShopDto = q.Shop != null ? new ShopDto
                {
                    Id = q.Shop.Id,
                    Name = q.Shop.Name,
                    Slug = q.Shop.Slug,
                    Description = q.Shop.Description,
                    IsActive = q.Shop.IsActive,
                    OwnerId = q.Shop.OwnerId,
                    Logo = q.LogoImage != null ? q.LogoImage.OriginalUrl : null!,
                    CoverImage = q.CoverImage != null ? q.CoverImage.ThumbnailUrl : null!,
                } : null ?? new ShopDto(),


                PromotionalSliderDto = q.Promotional != null ? new PromotionalSliderDto
                {
                    Id = q.Promotional.Id,
                    OriginalUrl = q.Promotional.OriginalUrl,
                    ThumbnailUrl = q.Promotional.ThumbnailUrl
                } : null ?? new PromotionalSliderDto(),

                ProductGalleryImageDto = q.Gallery != null ? new ProductGalleryImageDto
                {
                    Id = q.Gallery.Id,
                    OriginalUrl = q.Gallery.OriginalUrl,
                    ThumbnailUrl = q.Gallery.ThumbnailUrl
                } : null ?? new ProductGalleryImageDto(),

                SettingDto = q.Shop?.Setting != null ? new SettingDto
                {
                    Contact = q.Shop.Setting.Contact,
                    Website = q.Shop.Setting.Website,
                    LocationLat = q.Shop.Setting.LocationLat,
                    LocationLng = q.Shop.Setting.LocationLng,
                    LocationCity = q.Shop.Setting.LocationCity,
                    LocationState = q.Shop.Setting.LocationState,
                    LocationCountry = q.Shop.Setting.LocationCountry,
                    LocationFormattedAddress = q.Shop.Setting.LocationFormattedAddress,
                    LocationZip = q.Shop.Setting.LocationZip
                } : null ?? new SettingDto(),

                ShopAddressDto = q.Shop?.Addresses != null && q.Shop.Addresses.Any() ? new ShopAddressDto
                {
                    Id = q.Shop.Addresses.FirstOrDefault()!.Id,
                    Country = q.Shop.Addresses.FirstOrDefault()!.Country,
                    City = q.Shop.Addresses.FirstOrDefault()!.City,
                    State = q.Shop.Addresses.FirstOrDefault()!.State,
                    Zip = q.Shop.Addresses.FirstOrDefault()!.Zip,
                    StreetAddress = q.Shop.Addresses.FirstOrDefault()!.StreetAddress,
                    ShopId = q.Shop.Id
                } : null ?? new ShopAddressDto()

            }).ToList();

            return result;
        }

        public async Task<ProductDtoResponse> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _context.GetProductByIdAsync(id);
                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with id {id} not found.");
                }
                var category = product.CategoryId.HasValue
                    ? await _context.GetCategoryByIdAsync(product.CategoryId.Value)
                    : null;

                var shopAddress = product.ShopId.HasValue
                    ? await _context.GetShopAddressById(product.ShopId.Value)
                    : null;
                var shop = product.ShopId.HasValue
                    ? await _context.GetShopByProductId(product.ShopId.Value)
                    : null;

                var setting = await _context.GetSettingByProductId(product.Id);
                var rating = await _context.GetRatingByProductId(product.Id);
                var gallery = await _context.GetGalleryByProductId(product.Id);
                var promotional = await _context.GetPromotionalByProductId(product.Id);
                //var shop = await _context.GetShopByProductId(product.Id);
                var image = await _context.GetImageByProductId(product.Id);

                // Fetch the shipping charges based on the product's category size and Shop ID
                decimal shippingCharges = 0;
                if (category != null && product.ShopId.HasValue)
                {
                    var svcRelation = await _context.GetSVCRelationByShopIdAndSizeAsync(product.ShopId.Value, category.Size);
                    if (svcRelation != null)
                    {
                        shippingCharges = svcRelation.Price ?? 0;
                    }
                }
                decimal totalPrice = (product.Price ?? 0) + shippingCharges;

                var shopAddressDto = shopAddress != null
                    ? new ShopAddressDto
                    {
                        Id = shopAddress.Id,
                        Country = shopAddress.Country,
                        City = shopAddress.City,
                        State = shopAddress.State,
                        Zip = shopAddress.Zip,
                        StreetAddress = shopAddress.StreetAddress,
                        ShopId = shopAddress.ShopId
                    }
                    : null;

                var ratingDto = rating != null
                    ? new RatingDto
                    {
                        Rating1 = rating.Rating1,
                        PositiveFeedbacksCount = rating.PositiveFeedbacksCount,
                        NegativeFeedbacksCount = rating.NegativeFeedbacksCount,
                        AbusiveReportsCount = rating.AbusiveReportsCount,
                        Total = rating.Total
                    }
                    : null;

                var categoryDto = category != null
                    ? new CategoryDto
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Slug = category.Slug,
                        Icon = category.Icon,
                        ParentId = category.ParentId,
                        Language = category.Language,
                        CreatedAt = category.CreatedAt,
                        UpdatedAt = category.UpdatedAt,
                        DeletedAt = category.DeletedAt,
                        TypeId = category.TypeId
                    }
                    : null;

                var settingDto = setting != null
                    ? new SettingDto
                    {
                        Contact = setting.Contact,
                        Website = setting.Website,
                        LocationLat = setting.LocationLat,
                        LocationLng = setting.LocationLng,
                        LocationCity = setting.LocationCity,
                        LocationState = setting.LocationState,
                        LocationCountry = setting.LocationCountry,
                        LocationFormattedAddress = setting.LocationFormattedAddress,
                        LocationZip = setting.LocationZip
                    }
                    : null;

                string logoUrl = null!;
                if (shop != null)
                {
                    if (shop.LogoImageId.HasValue)
                    {
                        var logo = await _shopRepository.GetImageById(shop.LogoImageId.Value);
                        if (logo != null && _rootFolder != null && !string.IsNullOrEmpty(_rootFolder.ApplicationUrl))
                        {
                            logoUrl = logo;
                        }
                    }
                }

                var shopDto = shop != null
                    ? new ShopDto
                    {
                        Id = shop.Id,
                        Name = shop.Name,
                        Slug = shop.Slug,
                        Description = shop.Description,
                        IsActive = shop.IsActive,
                        Logo = logoUrl,
                        OwnerId = shop.OwnerId
                    }
                    : null;

                var promotionalSliderDto = promotional != null
                    ? new PromotionalSliderDto
                    {
                        Id = promotional.Id,
                        OriginalUrl = promotional.OriginalUrl,
                        ThumbnailUrl = promotional.ThumbnailUrl
                    }
                    : null;
                var productGalleryImageDto = product.Galleries?.FirstOrDefault() != null
                       ? new ProductGalleryImageDto
                       {
                           Id = product.Galleries.First().Id,
                           OriginalUrl = product.Galleries.First().OriginalUrl,
                           ThumbnailUrl = product.Galleries.First().ThumbnailUrl
                       }
                       : null;


                var imageDto = image != null
                    ? new ImageDto
                    {
                        Id = image.Id,
                        OriginalUrl = image.OriginalUrl,
                        ThumbnailUrl = image.ThumbnailUrl
                    }
                    : null;

                var productDtoResponse = new ProductDtoResponse
                {
                    Id = product.Id,
                    Name = product.Name,
                    Slug = product.Slug,
                    Description = product.Description,
                    SubPrice = product.Price,
                    TypeId = product.TypeId,
                    ShopId = product.ShopId,
                    SalePrice = product.SalePrice,
                    Language = product.Language,
                    MaxPrice = product.MaxPrice,
                    MinPrice = product.MinPrice,
                    Quantity = product.Quantity,
                    Sku = product.Sku,
                    InStock = product.InStock,
                    IsTaxable = product.IsTaxable,
                    ShippingClassId = product.ShippingClassId,
                    Status = product.Status,
                    ProductType = product.ProductType,
                    Unit = product.Unit,
                    Height = product.Height,
                    Width = product.Width,
                    Length = product.Length,
                    DeletedAt = product.DeletedAt,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt,
                    TotalReviews = product.TotalReviews,
                    ManufacturerId = product.ManufacturerId,
                    IsDigital = product.IsDigital,
                    ExternalProductButtonText = product.ExternalProductButtonText,
                    ExternalProductUrl = product.ExternalProductUrl,
                    IsExternal = product.IsExternal,
                    Ratings = product.Ratings,
                    MyReview = product.MyReview,
                    ImageId = product.ImageId,
                    AuthorId = product.AuthorId,
                    InWishlist = product.InWishlist,
                    Year = product.Year,
                    Mdoel = product.Model,
                    Mileage = product.Mileage,
                    Grade = product.Grade,
                    Damage = product.Damage,
                    TrimLevel = product.TrimLevel,
                    EngineId = product.EngineId,
                    ModelId = product.ModelId,
                    Transmission = product.Transmission,
                    Drivetrain = product.Drivetrain,
                    NewUsed = product.NewUsed,
                    OemPartNumber = product.OemPartNumber,
                    PartslinkNumber = product.PartslinkNumber,
                    HollanderIc = product.HollanderIc,
                    StockNumber = product.StockNumber,
                    TagNumber = product.TagNumber,
                    Location = product.Location,
                    Site = product.Site,
                    Vin = product.Vin,
                    Core = product.Core,
                    Color = product.Color,
                    ShippingCharges = shippingCharges, // Include the calculated shipping charges
                    Price = totalPrice,

                    ImageDto = imageDto,
                    SettingDto = settingDto!,
                    ShopAddressDto = shopAddressDto,
                    CategoryDto = categoryDto!,
                    PromotionalSliderDto = promotionalSliderDto,
                    ShopDto = shopDto,
                    ProductGalleryImageDto = productGalleryImageDto
                };

                return productDtoResponse;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<ProductDto> CreateProductAsync(ProductRequestDto requestDto)
        {
            try
            {

                var Image = new Image
                {

                    OriginalUrl = requestDto.imageDto.CoverImage,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                    UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                };

                await _context.AddImage(Image);
                await _context.UnitOfWork.SaveChangesAsync();

                //string slug = GenerateSlug(requestDto.productDto.Name);
                var slug = await EnsureUniqueSlugAsync(requestDto.productDto.Name, requestDto.productDto.Slug);
                var product = new Product
                {
                    Id = requestDto.productDto.Id,
                    Name = requestDto.productDto.Name,
                    Slug = slug,
                    Description = requestDto.productDto.Description,
                    Price = requestDto.productDto.Price,
                    SalePrice = requestDto.productDto.SalePrice,
                    Quantity = requestDto.productDto.Quantity,
                    InStock = requestDto.productDto.InStock,
                    Status = requestDto.productDto.Status,
                    ProductType = requestDto.productDto.ProductType,
                    Unit = requestDto.productDto.Unit,
                    Ratings = requestDto.productDto.Ratings,
                    Language = requestDto.productDto.Language,
                    MinPrice = requestDto.productDto.MinPrice,
                    MaxPrice = requestDto.productDto.MaxPrice,
                    Sku = requestDto.productDto.Sku,
                    IsTaxable = requestDto.productDto.IsTaxable,
                    Height = requestDto.productDto.Height,
                    Width = requestDto.productDto.Width,
                    Length = requestDto.productDto.Length,
                    IsDigital = requestDto.productDto.IsDigital,
                    IsExternal = requestDto.productDto.IsExternal,
                    ExternalProductUrl = requestDto.productDto.ExternalProductUrl,
                    ExternalProductButtonText = requestDto.productDto.ExternalProductButtonText,
                    TotalReviews = requestDto.productDto.TotalReviews,
                    MyReview = requestDto.productDto.MyReview,
                    InWishlist = requestDto.productDto.InWishlist,
                    Year = requestDto.productDto.Year,
                    Model = requestDto.productDto.Model,
                    Mileage = requestDto.productDto.Mileage,
                    Grade = requestDto.productDto.Grade,
                    Damage = requestDto.productDto.Damage,
                    TrimLevel = requestDto.productDto.TrimLevel,
                    EngineId = requestDto?.productDto?.EngineId,
                    ModelId = requestDto?.productDto?.ModelId,
                    Transmission = requestDto!.productDto.Transmission,
                    Drivetrain = requestDto.productDto.Drivetrain,
                    NewUsed = requestDto.productDto.NewUsed,
                    OemPartNumber = requestDto.productDto.OemPartNumber,
                    PartslinkNumber = requestDto.productDto.PartslinkNumber,
                    HollanderIc = requestDto.productDto.HollanderIc,
                    StockNumber = requestDto.productDto.StockNumber,
                    TagNumber = requestDto.productDto.TagNumber,
                    Location = requestDto.productDto.Location,
                    Site = requestDto.productDto.Site,
                    Vin = requestDto.productDto.Vin,
                    Core = requestDto.productDto.Core,
                    Color = requestDto.productDto.Color,

                    ImageId = Image.Id,
                    ShopId = requestDto.productDto.ShopId,
                    ManufacturerId = requestDto.productDto.ManufacturerId,
                    SubCategoryId = requestDto.productDto.SubCategoryId,
                    CategoryId = requestDto.productDto.CategoryId,
                };
                var createdProduct = await _context.CreateProductAsync(product);

                if (requestDto.GallaryImageUrls != null && requestDto.GallaryImageUrls.Any())
                {
                    foreach (var galleryImageUrl in requestDto.GallaryImageUrls)
                    {
                        var gallery = new Gallery
                        {
                            OriginalUrl = galleryImageUrl,
                            ProductId = createdProduct.Id,
                        };
                        await _context.CreateGalleryAsync(gallery);
                    }
                }

                var productTags = new List<ProductTag>();

                if (requestDto?.productDto?.TagIds != null)
                {
                    foreach (var tagId in requestDto?.productDto?.TagIds!)
                    {
                        productTags.Add(new ProductTag
                        {
                            TagId = tagId,
                            ProductId = createdProduct.Id,
                        });
                    }
                }

                await _context.CreateProductTagsAsync(productTags);
                await _context.UnitOfWork.SaveChangesAsync();


                var createdProductDto = _typeAdapter.Adapt<ProductDto>(createdProduct);
                return createdProductDto;
            }
            catch (Exception)
            {
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
            return $"{_rootFolder.ApplicationUrl}uploads/{uniqueFileName}";
        }

        public async Task UpdateProductAsync(ProductRequestDto requestDto)
        {
            var existingProduct = await _context.GetProductByIdAsync(requestDto.productDto.Id);

            if (existingProduct == null)
            {
                throw new KeyNotFoundException("Product not found.");
            }

            int? coverImageId = existingProduct.ImageId;

            if (requestDto.imageDto != null && requestDto.imageDto.CoverImage != null)
            {

                var coverImage = new Image
                {
                    OriginalUrl = requestDto.imageDto.CoverImage,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                    UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                };

                await _context.AddImage(coverImage);
                await _context.UnitOfWork.SaveChangesAsync();
                coverImageId = coverImage.Id;
            }

            existingProduct.Name = requestDto.productDto.Name;
            existingProduct.Slug = requestDto.productDto.Slug;
            existingProduct.Description = requestDto.productDto.Description;
            existingProduct.Price = requestDto.productDto.Price;
            existingProduct.SalePrice = requestDto.productDto.SalePrice;
            existingProduct.Language = requestDto.productDto.Language;
            existingProduct.MinPrice = requestDto.productDto.MinPrice;
            existingProduct.MaxPrice = requestDto.productDto.MaxPrice;
            existingProduct.Sku = requestDto.productDto.Sku;
            existingProduct.Quantity = requestDto.productDto.Quantity;
            existingProduct.InStock = requestDto.productDto.InStock;
            existingProduct.IsTaxable = requestDto.productDto.IsTaxable;
            existingProduct.Status = requestDto.productDto.Status;
            existingProduct.ProductType = requestDto.productDto.ProductType;
            existingProduct.Unit = requestDto.productDto.Unit;
            existingProduct.Height = requestDto.productDto.Height;
            existingProduct.Width = requestDto.productDto.Width;
            existingProduct.Length = requestDto.productDto.Length;
            existingProduct.IsDigital = requestDto.productDto.IsDigital;
            existingProduct.IsExternal = requestDto.productDto.IsExternal;
            existingProduct.ExternalProductUrl = requestDto.productDto.ExternalProductUrl;
            existingProduct.ExternalProductButtonText = requestDto.productDto.ExternalProductButtonText;
            existingProduct.Ratings = requestDto.productDto.Ratings;
            existingProduct.TotalReviews = requestDto.productDto.TotalReviews;
            existingProduct.MyReview = requestDto.productDto.MyReview;
            existingProduct.InWishlist = requestDto.productDto.InWishlist;
            existingProduct.Year = requestDto.productDto.Year;
            existingProduct.Model = requestDto.productDto.Model;
            existingProduct.Mileage = requestDto.productDto.Mileage;
            existingProduct.Grade = requestDto.productDto.Grade;
            existingProduct.Damage = requestDto.productDto.Damage;
            existingProduct.TrimLevel = requestDto.productDto.TrimLevel;
            //existingProduct.EngineId = requestDto.productDto.EngineId;
            //existingProduct.ModelId = requestDto.productDto.ModelId;
            existingProduct.Transmission = requestDto.productDto.Transmission;
            existingProduct.Drivetrain = requestDto.productDto.Drivetrain;
            existingProduct.NewUsed = requestDto.productDto.NewUsed;
            existingProduct.OemPartNumber = requestDto.productDto.OemPartNumber;
            existingProduct.PartslinkNumber = requestDto.productDto.PartslinkNumber;
            existingProduct.HollanderIc = requestDto.productDto.HollanderIc;
            existingProduct.StockNumber = requestDto.productDto.StockNumber;
            existingProduct.TagNumber = requestDto.productDto.TagNumber;
            existingProduct.Location = requestDto.productDto.Location;
            existingProduct.Site = requestDto.productDto.Site;
            existingProduct.Vin = requestDto.productDto.Vin;
            existingProduct.Core = requestDto.productDto.Core;
            existingProduct.Color = requestDto.productDto.Color;

            //existingProduct.ShopId = requestDto.productDto.ShopId;
            //existingProduct.ManufacturerId = requestDto.productDto.ManufacturerId;
            //existingProduct.SubCategoryId = requestDto.productDto.SubCategoryId;
            ////existingProduct.AuthorId = requestDto.productDto.AuthorId;
            //existingProduct.CategoryId = requestDto.productDto.CategoryId;

            if (coverImageId.HasValue)
            {
                existingProduct.ImageId = coverImageId.Value;
            }

            await _context.UpdateProductAsync(existingProduct);

            await _context.DeleteGalleriesAsync(existingProduct.Id);

            if (requestDto.GallaryImageUrls != null && requestDto.GallaryImageUrls.Any())
            {
                foreach (var galleryImageUrl in requestDto.GallaryImageUrls)
                {
                    var gallery = new Gallery
                    {
                        OriginalUrl = galleryImageUrl,
                        ProductId = existingProduct.Id,
                    };
                    await _context.CreateGalleryAsync(gallery);
                }
            }

            await _context.DeleteProductTagsByProductIdAsync(existingProduct.Id);

            if (requestDto.productDto.TagIds != null && requestDto.productDto.TagIds.Any())
            {
                var productTags = new List<ProductTag>();
                foreach (var tagId in requestDto.productDto.TagIds)
                {
                    productTags.Add(new ProductTag
                    {
                        TagId = tagId,
                        ProductId = existingProduct.Id,
                    });
                }
                await _context.CreateProductTagsAsync(productTags);
            }

            await _context.UnitOfWork.SaveChangesAsync();
        }
        public async Task<ProductDtoResponse> GetProductBySlugAsync(string slug)
        {
            var products = await _context.GetProductBySlugAsync(slug);
            var finalresult = new List<ProductDtoResponse>();

            var result = products.Select(q => new ProductDtoResponse
            {
                Id = q.Product.Id,
                Name = q.Product.Name,
                Slug = q.Product.Slug,
                Description = q.Product.Description,
                SubPrice = q.Product.Price,
                ShippingCharges = q.Category != null && q.Product.ShopId.HasValue
                    ? (_context.GetSVCRelationByShopIdAndSizeAsync(q.Product.ShopId.Value, q.Category.Size).Result?.Price ?? 0)
                    : 0,
                Price = (q.Product.Price ?? 0) + (q.Category != null && q.Product.ShopId.HasValue
                    ? (_context.GetSVCRelationByShopIdAndSizeAsync(q.Product.ShopId.Value, q.Category.Size).Result?.Price ?? 0)
                    : 0),
                ShopId = q.Product.ShopId,
                Model = q.Product.Model,
                IsDigital = q.Product.IsDigital,
                CreatedAt = q.Product.CreatedAt,
                UpdatedAt = q.Product.UpdatedAt,
                DeletedAt = q.Product.DeletedAt,
                IsExternal = q.Product.IsExternal,
                ExternalProductUrl = q.Product.ExternalProductUrl,
                ExternalProductButtonText = q.Product.ExternalProductButtonText,
                Ratings = q.Rating != null ? q.Rating.Total : 0,
                TotalReviews = q.Product.TotalReviews ?? 0,
                MyReview = q.Product.MyReview,
                IsTaxable = q.Product.IsTaxable,
                ShippingClassId = q.Product.ShippingClassId,
                Status = q.Product.Status,
                ProductType = q.Product.ProductType,
                Unit = q.Product.Unit,
                Height = q.Product.Height,
                Width = q.Product.Width,
                Length = q.Product.Length,
                ImageId = q.Product.ImageId,
                InWishlist = q.Product.InWishlist,
                SalePrice = q.Product.SalePrice,
                MinPrice = q.Product.MinPrice ?? 0,
                MaxPrice = q.Product.MaxPrice ?? 0,
                Quantity = q.Product.Quantity ?? 0,
                Sku = q.Product.Sku,
                TypeId = q.Product.TypeId,
                AuthorId = q.Product.AuthorId,
                ManufacturerId = q.Product.ManufacturerId,
                Language = q.Product.Language,
                InStock = q.Product.InStock,
                Year = q.Product.Year,
                Mdoel = q.Product.Model,
                Mileage = q.Product.Mileage,
                Grade = q.Product.Grade,
                Damage = q.Product.Damage,
                TrimLevel = q.Product.TrimLevel,
                EngineId = q.Product.EngineId,
                ModelId = q.Product.ModelId,
                Transmission = q.Product.Transmission,
                Drivetrain = q.Product.Drivetrain,
                NewUsed = q.Product.NewUsed,
                OemPartNumber = q.Product.OemPartNumber,
                PartslinkNumber = q.Product.PartslinkNumber,
                HollanderIc = q.Product.HollanderIc,
                StockNumber = q.Product.StockNumber,
                TagNumber = q.Product.TagNumber,
                Location = q.Product.Location,
                Site = q.Product.Site,
                Vin = q.Product.Vin,
                Core = q.Product.Core,
                Color = q.Product.Color,

                CategoryDto = q.Category != null ? new CategoryDto
                {
                    Id = q.Category.Id,
                    Name = q.Category.Name,
                    Slug = q.Category.Slug,
                    Language = q.Category.Language,
                    Icon = q.Category.Icon,
                    CreatedAt = q.Category.CreatedAt,
                    UpdatedAt = q.Category.UpdatedAt
                } : null ?? new CategoryDto(),

                ImageDto = q.Product.Image != null ? new ImageDto
                {
                    Id = q.Product.Image.Id,
                    OriginalUrl = q.Product.Image.OriginalUrl,
                    ThumbnailUrl = q.Product.Image.ThumbnailUrl
                } : null,

                RatingDto = q.Rating != null ? new RatingDto
                {
                    Rating1 = q.Rating.Rating1,
                    PositiveFeedbacksCount = q.Rating.PositiveFeedbacksCount,
                    NegativeFeedbacksCount = q.Rating.NegativeFeedbacksCount,
                    AbusiveReportsCount = q.Rating.AbusiveReportsCount,
                    Total = q.Rating.Total
                } : null,

                ShopDto = q.Shop != null ? new ShopDto
                {
                    Id = q.Shop.Id,
                    Name = q.Shop.Name,
                    Slug = q.Shop.Slug,
                    Description = q.Shop.Description,
                    IsActive = q.Shop.IsActive,
                    OwnerId = q.Shop.OwnerId,
                    Logo = q.LogoImage != null ? q.LogoImage.OriginalUrl : string.Empty,
                    CoverImage = q.CoverImage != null ? q.CoverImage.ThumbnailUrl : string.Empty,
                } : null,

                PromotionalSliderDto = q.Promotional != null ? new PromotionalSliderDto
                {
                    Id = q.Promotional.Id,
                    OriginalUrl = q.Promotional.OriginalUrl,
                    ThumbnailUrl = q.Promotional.ThumbnailUrl
                } : null,

                ProductGalleryImageDto = q.Gallery != null ? new ProductGalleryImageDto
                {
                    Id = q.Gallery.Id,
                    OriginalUrl = q.Gallery.OriginalUrl,
                    ThumbnailUrl = q.Gallery.ThumbnailUrl
                } : null,

                SettingDto = q.Shop?.Setting != null ? new SettingDto
                {
                    Contact = q.Shop.Setting.Contact,
                    Website = q.Shop.Setting.Website,
                    LocationLat = q.Shop.Setting.LocationLat,
                    LocationLng = q.Shop.Setting.LocationLng,
                    LocationCity = q.Shop.Setting.LocationCity,
                    LocationState = q.Shop.Setting.LocationState,
                    LocationCountry = q.Shop.Setting.LocationCountry,
                    LocationFormattedAddress = q.Shop.Setting.LocationFormattedAddress,
                    LocationZip = q.Shop.Setting.LocationZip
                } : null ?? new SettingDto(),

                ShopAddressDto = q.Shop?.Addresses != null && q.Shop.Addresses.Any() ? new ShopAddressDto
                {
                    Id = q.Shop.Addresses.FirstOrDefault()!.Id,
                    Country = q.Shop.Addresses.FirstOrDefault()!.Country,
                    City = q.Shop.Addresses.FirstOrDefault()!.City,
                    State = q.Shop.Addresses.FirstOrDefault()!.State,
                    Zip = q.Shop.Addresses.FirstOrDefault()!.Zip,
                    StreetAddress = q.Shop.Addresses.FirstOrDefault()!.StreetAddress,
                    ShopId = q.Shop.Id
                } : null,

                EngineDto = q.Product.Engine != null ? new EngineDto
                {
                    Id = q.Product.Engine.Id,
                    Year = q.Product.Engine.Year,
                    CategoryId = q.Product.Engine.CategoryId,
                    CategoryName = q.Product.Engine.Category != null ? q.Product.Engine.Category.Name : string.Empty,
                    ManufacturerId = q.Product.Engine.ManufacturerId,
                    ManufacturerName = q.Product.Engine.Manufacturer != null ? q.Product.Engine.Manufacturer.Name : string.Empty,
                    SubcategoryId = q.Product.Engine.SubcategoryId,
                    SubcategoryName = q.Product.Engine.Subcategory != null ? q.Product.Engine.Subcategory.Subcategory : string.Empty,
                    ModelId = q.Product.Engine.ModelId,
                    ModelName = q.Product.Engine.Model != null ? q.Product.Engine.Model.Model : string.Empty,
                    Engine1 = q.Product.Engine.Engine1,
                    HollanderCode = q.Product.Engine.HollanderCode,
                } : null,

                SubCategoryListDto = q.Product.SubCategory != null ? new SubCategoryListDto
                {
                    Id = q.Product.SubCategory.Id,
                    CategoryId = q.Product.SubCategory.CategoryId,
                    Subcategory = q.Product.SubCategory.Subcategory,
                    CategoryName = q.Product.SubCategory.Category != null ? q.Product.SubCategory.Category.Name : string.Empty,
                    Slug = q.Product.SubCategory.Slug
                } : null,

                ManufactureModelDto = q.Product.ModelNavigation != null ? new ManufactureModelDto
                {
                    Id = q.Product.ModelNavigation.Id,
                    ManufacturerId = q.Product.ModelNavigation.ManufacturerId,
                    ManufacturerName = q.Product.ModelNavigation.Manufacturer != null ? q.Product.ModelNavigation.Manufacturer.Name : string.Empty,
                    Model = q.Product.ModelNavigation.Model,
                    Slug = q.Product.ModelNavigation.Slug
                } : null,
                ManufacturersDto = q.Product.Manufacturer != null ? new ManufacturersDto
                {
                    Id = q.Manufacturer.Id,
                    Name = q.Manufacturer.Name,
                    Slug = q.Manufacturer.Slug,
                    Language = q.Manufacturer.Language,
                    Description = q.Manufacturer.Description,
                    Website = q.Manufacturer.Website,
                    ImageId = q.Manufacturer.ImageId,
                    SocialId = q.Manufacturer.SocialId,
                    IsApproved = q.Manufacturer.IsApproved,
                } : null ?? new ManufacturersDto(),

            }).ToList();

            return result.FirstOrDefault()!;
        }


        public async Task<bool> RemoveProductAsync(int id)
        {
            try
            {
                await _context.DeleteImages(id);
                await _context.DeleteGalleriesAsync(id);
                await _context.DeleteProductTagsByProductIdAsync(id);
                var result = await _context.RemoveProductAsync(id);
                if (!result)
                {
                    return false;
                }

                await _context.UnitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<IEnumerable<ProductDtoResponse>> GetPopularProductsAsync(int? shopId = null, int? vendorId = null)
        {
            try
            {
                var shopIds = new List<int>();

                if (vendorId.HasValue)
                {
                    var shops = await _shopRepository.GetShopsByOwnerIdAsync(vendorId.Value);
                    shopIds = shops.Select(s => s.Id).ToList();
                }

                var products = await _context.GetPopularProductsAsync(shopId, shopIds);

                var result = new List<ProductDtoResponse>();

                foreach (var product in products)
                {
                    var category = product.CategoryId.HasValue
                        ? await _context.GetCategoryByIdAsync(product.CategoryId.Value)
                        : null;

                    var shopAddress = product.ShopId.HasValue
                        ? await _context.GetShopAddressById(product.ShopId.Value)
                        : null;

                    var shop = product.ShopId.HasValue
                        ? await _context.GetShopByProductId(product.ShopId.Value)
                        : null;

                    var setting = await _context.GetSettingByProductId(product.Id);
                    var rating = await _context.GetRatingByProductId(product.Id);
                    var gallery = await _context.GetGalleryByProductId(product.Id);
                    var promotional = await _context.GetPromotionalByProductId(product.Id);

                    var image = product.Image;

                    // Fetch the shipping charges based on the product's category size and Shop ID
                    decimal shippingCharges = 0;
                    if (category != null && product.ShopId.HasValue)
                    {
                        var svcRelation = await _context.GetSVCRelationByShopIdAndSizeAsync(product.ShopId.Value, category.Size);
                        if (svcRelation != null)
                        {
                            shippingCharges = svcRelation.Price ?? 0;
                        }
                    }
                    decimal totalPrice = (product.Price ?? 0) + shippingCharges;

                    var shopAddressDto = shopAddress != null
                        ? new ShopAddressDto
                        {
                            Id = shopAddress.Id,
                            Country = shopAddress.Country,
                            City = shopAddress.City,
                            State = shopAddress.State,
                            Zip = shopAddress.Zip,
                            StreetAddress = shopAddress.StreetAddress,
                            ShopId = shopAddress.ShopId
                        }
                        : null;

                    var ratingDto = rating != null
                        ? new RatingDto
                        {
                            Rating1 = rating.Rating1,
                            PositiveFeedbacksCount = rating.PositiveFeedbacksCount,
                            NegativeFeedbacksCount = rating.NegativeFeedbacksCount,
                            AbusiveReportsCount = rating.AbusiveReportsCount,
                            Total = rating.Total
                        }
                        : null;

                    var categoryDto = category != null
                        ? new CategoryDto
                        {
                            Id = category.Id,
                            Name = category.Name,
                            Slug = category.Slug,
                            Icon = category.Icon,
                            ParentId = category.ParentId,
                            Language = category.Language,
                            CreatedAt = category.CreatedAt,
                            UpdatedAt = category.UpdatedAt,
                            DeletedAt = category.DeletedAt,
                            TypeId = category.TypeId
                        }
                        : null;

                    var settingDto = setting != null
                        ? new SettingDto
                        {
                            Contact = setting.Contact,
                            Website = setting.Website,
                            LocationLat = setting.LocationLat,
                            LocationLng = setting.LocationLng,
                            LocationCity = setting.LocationCity,
                            LocationState = setting.LocationState,
                            LocationCountry = setting.LocationCountry,
                            LocationFormattedAddress = setting.LocationFormattedAddress,
                            LocationZip = setting.LocationZip
                        }
                        : null;

                    string logoUrl = null!;
                    if (shop != null)
                    {
                        if (shop.LogoImageId.HasValue)
                        {
                            var logo = await _shopRepository.GetImageById(shop.LogoImageId.Value);
                            if (logo != null && _rootFolder != null && !string.IsNullOrEmpty(_rootFolder.ApplicationUrl))
                            {
                                logoUrl = logo;
                            }
                        }
                    }

                    var shopDto = shop != null
                        ? new ShopDto
                        {
                            Id = shop.Id,
                            Name = shop.Name,
                            Slug = shop.Slug,
                            Description = shop.Description,
                            IsActive = shop.IsActive,
                            Logo = logoUrl!,
                            OwnerId = shop.OwnerId
                        }
                        : null;

                    var promotionalSliderDto = promotional != null
                        ? new PromotionalSliderDto
                        {
                            Id = promotional.Id,
                            OriginalUrl = promotional.OriginalUrl,
                            ThumbnailUrl = promotional.ThumbnailUrl
                        }
                        : null;

                    var productGalleryImageDto = gallery != null
                        ? new ProductGalleryImageDto
                        {
                            Id = gallery.Id,
                            OriginalUrl = gallery.OriginalUrl,
                            ThumbnailUrl = gallery.ThumbnailUrl
                        }
                        : null;

                    var imageDto = image != null
                        ? new ImageDto
                        {
                            Id = image.Id,
                            OriginalUrl = image.OriginalUrl,
                            ThumbnailUrl = image.ThumbnailUrl
                        }
                        : null;

                    var res = new ProductDtoResponse
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Slug = product.Slug,
                        Description = product.Description,
                        SubPrice = product.Price,
                        TypeId = product.TypeId,
                        ShopId = product.ShopId,
                        SalePrice = product.SalePrice,
                        Language = product.Language,
                        MaxPrice = product.MaxPrice,
                        MinPrice = product.MinPrice,
                        Quantity = product.Quantity,
                        Sku = product.Sku,
                        InStock = product.InStock,
                        IsTaxable = product.IsTaxable,
                        ShippingClassId = product.ShippingClassId,
                        Status = product.Status,
                        ProductType = product.ProductType,
                        Unit = product.Unit,
                        Height = product.Height,
                        Width = product.Width,
                        Length = product.Length,
                        DeletedAt = product.DeletedAt,
                        CreatedAt = product.CreatedAt,
                        UpdatedAt = product.UpdatedAt,
                        TotalReviews = product.TotalReviews,
                        ManufacturerId = product.ManufacturerId,
                        IsDigital = product.IsDigital,
                        ExternalProductButtonText = product.ExternalProductButtonText,
                        ExternalProductUrl = product.ExternalProductUrl,
                        IsExternal = product.IsExternal,
                        Ratings = product.Ratings,
                        MyReview = product.MyReview,
                        ImageId = product.ImageId,
                        AuthorId = product.AuthorId,
                        InWishlist = product.InWishlist,
                        Year = product.Year,
                        Model = product.Model,
                        Mileage = product.Mileage,
                        Grade = product.Grade,
                        Damage = product.Damage,
                        TrimLevel = product.TrimLevel,
                        EngineId = product.EngineId,
                        Transmission = product.Transmission,
                        Drivetrain = product.Drivetrain,
                        NewUsed = product.NewUsed,
                        OemPartNumber = product.OemPartNumber,
                        PartslinkNumber = product.PartslinkNumber,
                        HollanderIc = product.HollanderIc,
                        StockNumber = product.StockNumber,
                        TagNumber = product.TagNumber,
                        Location = product.Location,
                        Site = product.Site,
                        Vin = product.Vin,
                        Core = product.Core,
                        Color = product.Color,
                        ShippingCharges = shippingCharges,
                        Price = totalPrice,

                        ImageDto = imageDto,
                        SettingDto = settingDto!,
                        ShopAddressDto = shopAddressDto,
                        CategoryDto = categoryDto!,
                        PromotionalSliderDto = promotionalSliderDto,
                        ShopDto = shopDto,
                        ProductGalleryImageDto = productGalleryImageDto,
                        RatingDto = ratingDto
                    };

                    result.Add(res);
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ProductDtoResponse>> GetBestSellingProductsAsync(int? shopId = null, int? vendorId = null)
        {
            try
            {
                var shopIds = new List<int>();

                if (vendorId.HasValue)
                {
                    var shops = await _shopRepository.GetShopsByOwnerIdAsync(vendorId.Value);
                    shopIds = shops.Select(s => s.Id).ToList();
                }

                var products = await _context.GetBestSellingProductsAsync(shopId, shopIds);
                var result = new List<ProductDtoResponse>();

                foreach (var product in products)
                {
                    var category = product.CategoryId.HasValue
                        ? await _context.GetCategoryByIdAsync(product.CategoryId.Value)
                        : null;

                    var shopAddress = product.ShopId.HasValue
                        ? await _context.GetShopAddressById(product.ShopId.Value)
                        : null;
                    var shop = product.ShopId.HasValue
                        ? await _context.GetShopByProductId(product.ShopId.Value)
                        : null;
                    var setting = await _context.GetSettingByProductId(product.Id);
                    var rating = await _context.GetRatingByProductId(product.Id);
                    var gallery = await _context.GetGalleryByProductId(product.Id);
                    var promotional = await _context.GetPromotionalByProductId(product.Id);
                    //var shop = await _context.GetShopById(product.ShopId.Value);

                    var image = product.Image;

                    decimal shippingCharges = 0;
                    if (category != null && product.ShopId.HasValue)
                    {
                        var svcRelation = await _context.GetSVCRelationByShopIdAndSizeAsync(product.ShopId.Value, category.Size);
                        if (svcRelation != null)
                        {
                            shippingCharges = svcRelation.Price ?? 0;
                        }
                    }
                    decimal totalPrice = (product.Price ?? 0) + shippingCharges;

                    var shopAddressDto = shopAddress != null
                        ? new ShopAddressDto
                        {
                            Id = shopAddress.Id,
                            Country = shopAddress.Country,
                            City = shopAddress.City,
                            State = shopAddress.State,
                            Zip = shopAddress.Zip,
                            StreetAddress = shopAddress.StreetAddress,
                            ShopId = shopAddress.ShopId
                        }
                        : null;

                    var ratingDto = rating != null
                        ? new RatingDto
                        {
                            Rating1 = rating.Rating1,
                            PositiveFeedbacksCount = rating.PositiveFeedbacksCount,
                            NegativeFeedbacksCount = rating.NegativeFeedbacksCount,
                            AbusiveReportsCount = rating.AbusiveReportsCount,
                            Total = rating.Total
                        }
                        : null;

                    var categoryDto = category != null
                        ? new CategoryDto
                        {
                            Id = category.Id,
                            Name = category.Name,
                            Slug = category.Slug,
                            Icon = category.Icon,
                            ParentId = category.ParentId,
                            Language = category.Language,
                            CreatedAt = category.CreatedAt,
                            UpdatedAt = category.UpdatedAt,
                            DeletedAt = category.DeletedAt,
                            TypeId = category.TypeId
                        }
                        : null;

                    var settingDto = setting != null
                        ? new SettingDto
                        {
                            Contact = setting.Contact,
                            Website = setting.Website,
                            LocationLat = setting.LocationLat,
                            LocationLng = setting.LocationLng,
                            LocationCity = setting.LocationCity,
                            LocationState = setting.LocationState,
                            LocationCountry = setting.LocationCountry,
                            LocationFormattedAddress = setting.LocationFormattedAddress,
                            LocationZip = setting.LocationZip
                        }
                        : null;

                    string logoUrl = null!;
                    if (shop != null)
                    {
                        if (shop.LogoImageId.HasValue)
                        {
                            var logo = await _shopRepository.GetImageById(shop.LogoImageId.Value);
                            if (logo != null && _rootFolder != null && !string.IsNullOrEmpty(_rootFolder.ApplicationUrl))
                            {
                                logoUrl = logo;
                            }
                        }
                    }

                    var shopDto = shop != null
                        ? new ShopDto
                        {
                            Id = shop.Id,
                            Name = shop.Name,
                            Slug = shop.Slug,
                            Description = shop.Description,
                            IsActive = shop.IsActive,
                            Logo = logoUrl,
                            OwnerId = shop.OwnerId
                        }
                        : null;

                    var promotionalSliderDto = promotional != null
                        ? new PromotionalSliderDto
                        {
                            Id = promotional.Id,
                            OriginalUrl = promotional.OriginalUrl,
                            ThumbnailUrl = promotional.ThumbnailUrl
                        }
                        : null;

                    var productGalleryImageDto = gallery != null
                        ? new ProductGalleryImageDto
                        {
                            Id = gallery.Id,
                            OriginalUrl = gallery.OriginalUrl,
                            ThumbnailUrl = gallery.ThumbnailUrl
                        }
                        : null;

                    var imageDto = image != null
                        ? new ImageDto
                        {
                            Id = image.Id,
                            OriginalUrl = image.OriginalUrl,
                            ThumbnailUrl = image.ThumbnailUrl
                        }
                        : null;

                    var res = new ProductDtoResponse
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Slug = product.Slug,
                        Description = product.Description,
                        SubPrice = product.Price,
                        TypeId = product.TypeId,
                        ShopId = product.ShopId,
                        SalePrice = product.SalePrice,
                        Language = product.Language,
                        MaxPrice = product.MaxPrice,
                        MinPrice = product.MinPrice,
                        Quantity = product.Quantity,
                        Sku = product.Sku,
                        InStock = product.InStock,
                        IsTaxable = product.IsTaxable,
                        ShippingClassId = product.ShippingClassId,
                        Status = product.Status,
                        ProductType = product.ProductType,
                        Unit = product.Unit,
                        Height = product.Height,
                        Width = product.Width,
                        Length = product.Length,
                        DeletedAt = product.DeletedAt,
                        CreatedAt = product.CreatedAt,
                        UpdatedAt = product.UpdatedAt,
                        TotalReviews = product.TotalReviews,
                        ManufacturerId = product.ManufacturerId,
                        IsDigital = product.IsDigital,
                        ExternalProductButtonText = product.ExternalProductButtonText,
                        ExternalProductUrl = product.ExternalProductUrl,
                        IsExternal = product.IsExternal,
                        Ratings = product.Ratings,
                        MyReview = product.MyReview,
                        ImageId = product.ImageId,
                        AuthorId = product.AuthorId,
                        InWishlist = product.InWishlist,
                        Year = product.Year,
                        Model = product.Model,
                        Mileage = product.Mileage,
                        Grade = product.Grade,
                        Damage = product.Damage,
                        TrimLevel = product.TrimLevel,
                        EngineId = product.EngineId,
                        ModelId = product.ModelId,
                        Transmission = product.Transmission,
                        Drivetrain = product.Drivetrain,
                        NewUsed = product.NewUsed,
                        OemPartNumber = product.OemPartNumber,
                        PartslinkNumber = product.PartslinkNumber,
                        HollanderIc = product.HollanderIc,
                        StockNumber = product.StockNumber,
                        TagNumber = product.TagNumber,
                        Location = product.Location,
                        Site = product.Site,
                        Vin = product.Vin,
                        Core = product.Core,
                        Color = product.Color,
                        ShippingCharges = shippingCharges,
                        Price = totalPrice,

                        ImageDto = imageDto,
                        SettingDto = settingDto!,
                        ShopAddressDto = shopAddressDto,
                        CategoryDto = categoryDto!,
                        PromotionalSliderDto = promotionalSliderDto,
                        ShopDto = shopDto,
                        ProductGalleryImageDto = productGalleryImageDto,
                        RatingDto = ratingDto
                    };

                    result.Add(res);
                }
                return result;
            }

            catch (Exception)
            {
                throw;
            }
        }


        public async Task<IEnumerable<ProductDtoResponse>> GetDraftProductsAsync()
        {
            try
            {
                var products = await _context.GetDraftProductsAsync();
                var result = new List<ProductDtoResponse>();

                foreach (var product in products)
                {
                    var category = product.CategoryId.HasValue
                        ? await _context.GetCategoryByIdAsync(product.CategoryId.Value)
                        : null;

                    var shopAddress = product.ShopId.HasValue
                        ? await _context.GetShopAddressById(product.ShopId.Value)
                        : null;
                    var shop = product.ShopId.HasValue
                        ? await _context.GetShopByProductId(product.ShopId.Value)
                        : null;
                    var setting = await _context.GetSettingByProductId(product.Id);
                    var rating = await _context.GetRatingByProductId(product.Id);
                    var gallery = await _context.GetGalleryByProductId(product.Id);
                    var promotional = await _context.GetPromotionalByProductId(product.Id);
                    //var shop = await _context.GetShopById(product.ShopId.Value);

                    var image = product.Image;


                    decimal shippingCharges = 0;
                    if (category != null && product.ShopId.HasValue)
                    {
                        var svcRelation = await _context.GetSVCRelationByShopIdAndSizeAsync(product.ShopId.Value, category.Size);
                        if (svcRelation != null)
                        {
                            shippingCharges = svcRelation.Price ?? 0;
                        }
                    }
                    decimal totalPrice = (product.Price ?? 0) + shippingCharges;

                    var shopAddressDto = shopAddress != null
                        ? new ShopAddressDto
                        {
                            Id = shopAddress.Id,
                            Country = shopAddress.Country,
                            City = shopAddress.City,
                            State = shopAddress.State,
                            Zip = shopAddress.Zip,
                            StreetAddress = shopAddress.StreetAddress,
                            ShopId = shopAddress.ShopId
                        }
                        : null;

                    var ratingDto = rating != null
                        ? new RatingDto
                        {
                            Rating1 = rating.Rating1,
                            PositiveFeedbacksCount = rating.PositiveFeedbacksCount,
                            NegativeFeedbacksCount = rating.NegativeFeedbacksCount,
                            AbusiveReportsCount = rating.AbusiveReportsCount,
                            Total = rating.Total
                        }
                        : null;

                    var categoryDto = category != null
                        ? new CategoryDto
                        {
                            Id = category.Id,
                            Name = category.Name,
                            Slug = category.Slug,
                            Icon = category.Icon,
                            ParentId = category.ParentId,
                            Language = category.Language,
                            CreatedAt = category.CreatedAt,
                            UpdatedAt = category.UpdatedAt,
                            DeletedAt = category.DeletedAt,
                            TypeId = category.TypeId
                        }
                        : null;

                    var settingDto = setting != null
                        ? new SettingDto
                        {
                            Contact = setting.Contact,
                            Website = setting.Website,
                            LocationLat = setting.LocationLat,
                            LocationLng = setting.LocationLng,
                            LocationCity = setting.LocationCity,
                            LocationState = setting.LocationState,
                            LocationCountry = setting.LocationCountry,
                            LocationFormattedAddress = setting.LocationFormattedAddress,
                            LocationZip = setting.LocationZip
                        }
                        : null;

                    string logoUrl = null!;
                    if (shop != null)
                    {
                        if (shop.LogoImageId.HasValue)
                        {
                            var logo = await _shopRepository.GetImageById(shop.LogoImageId.Value);
                            if (logo != null && _rootFolder != null && !string.IsNullOrEmpty(_rootFolder.ApplicationUrl))
                            {
                                logoUrl = logo;
                            }
                        }
                    }

                    var shopDto = shop != null
                        ? new ShopDto
                        {
                            Id = shop.Id,
                            Name = shop.Name,
                            Slug = shop.Slug,
                            Description = shop.Description,
                            IsActive = shop.IsActive,
                            Logo = logoUrl,
                            OwnerId = shop.OwnerId
                        }
                        : null;

                    var promotionalSliderDto = promotional != null
                        ? new PromotionalSliderDto
                        {
                            Id = promotional.Id,
                            OriginalUrl = promotional.OriginalUrl,
                            ThumbnailUrl = promotional.ThumbnailUrl
                        }
                        : null;

                    var productGalleryImageDto = gallery != null
                        ? new ProductGalleryImageDto
                        {
                            Id = gallery.Id,
                            OriginalUrl = gallery.OriginalUrl,
                            ThumbnailUrl = gallery.ThumbnailUrl
                        }
                        : null;

                    var imageDto = image != null
                        ? new ImageDto
                        {
                            Id = image.Id,
                            OriginalUrl = image.OriginalUrl,
                            ThumbnailUrl = image.ThumbnailUrl
                        }
                        : null;

                    var res = new ProductDtoResponse
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Slug = product.Slug,
                        Description = product.Description,
                        SubPrice = product.Price,
                        TypeId = product.TypeId,
                        ShopId = product.ShopId,
                        SalePrice = product.SalePrice,
                        Language = product.Language,
                        MaxPrice = product.MaxPrice,
                        MinPrice = product.MinPrice,
                        Quantity = product.Quantity,
                        Sku = product.Sku,
                        InStock = product.InStock,
                        IsTaxable = product.IsTaxable,
                        ShippingClassId = product.ShippingClassId,
                        Status = product.Status,
                        ProductType = product.ProductType,
                        Unit = product.Unit,
                        Height = product.Height,
                        Width = product.Width,
                        Length = product.Length,
                        DeletedAt = product.DeletedAt,
                        CreatedAt = product.CreatedAt,
                        UpdatedAt = product.UpdatedAt,
                        TotalReviews = product.TotalReviews,
                        ManufacturerId = product.ManufacturerId,
                        IsDigital = product.IsDigital,
                        ExternalProductButtonText = product.ExternalProductButtonText,
                        ExternalProductUrl = product.ExternalProductUrl,
                        IsExternal = product.IsExternal,
                        Ratings = product.Ratings,
                        MyReview = product.MyReview,
                        ImageId = product.ImageId,
                        AuthorId = product.AuthorId,
                        InWishlist = product.InWishlist,
                        Year = product.Year,
                        Model = product.Model,
                        Mileage = product.Mileage,
                        Grade = product.Grade,
                        Damage = product.Damage,
                        TrimLevel = product.TrimLevel,
                        EngineId = product.EngineId,
                        ModelId = product.ModelId,
                        Transmission = product.Transmission,
                        Drivetrain = product.Drivetrain,
                        NewUsed = product.NewUsed,
                        OemPartNumber = product.OemPartNumber,
                        PartslinkNumber = product.PartslinkNumber,
                        HollanderIc = product.HollanderIc,
                        StockNumber = product.StockNumber,
                        TagNumber = product.TagNumber,
                        Location = product.Location,
                        Site = product.Site,
                        Vin = product.Vin,
                        Core = product.Core,
                        Color = product.Color,
                        ShippingCharges = shippingCharges,
                        Price = totalPrice,

                        ImageDto = imageDto,
                        SettingDto = settingDto!,
                        ShopAddressDto = shopAddressDto,
                        CategoryDto = categoryDto!,
                        PromotionalSliderDto = promotionalSliderDto,
                        ShopDto = shopDto,
                        ProductGalleryImageDto = productGalleryImageDto,
                        RatingDto = ratingDto
                    };

                    result.Add(res);
                }
                return result;
            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ProductDto>> GetProductsStockAsync()
        {
            var products = await _context.GetProductsStockAsync();
            if (products == null || !products.Any())
            {
                throw new KeyNotFoundException("No products found in stock.");
            }
            return products.Select(p => _typeAdapter.Adapt<ProductDto>(p));
        }


        public async Task<List<LookupDto>> GetShopsByProduct()
        {
            var shops = await _context.GetShopsByProduct();
            var lookupDto = shops.Select(item => new LookupDto
            {
                Id = item.Id,
                Name = item.Name
            }).ToList();

            return lookupDto;
        }
        public async Task<List<LookupDto>> GetCategoriesByProduct(int? vendorId = null)
        {
            var shopIds = new List<int>();

            if (vendorId.HasValue)
            {
                var shops = await _shopRepository.GetShopsByOwnerIdAsync(vendorId.Value);
                shopIds = shops.Select(s => s.Id).ToList();
            }
            var categories = await _context.GetCategoriesByProduct(shopIds);
            var result = new List<ProductDtoResponse>();

            var lookupDto = categories.Select(item => new LookupDto
            {
                Id = item.Id,
                Name = item.Name
            }).ToList();

            return lookupDto;
        }
        public async Task<List<LookupDto>> GetTagsByProduct()
        {
            var tags = await _context.GetTagsByProduct();
            var lookupDto = tags.Select(item => new LookupDto
            {
                Id = item.Id,
                Name = item.Name
            }).ToList();

            return lookupDto;
        }
        public async Task<List<LookupDto>> GetAuthorsByProduct()
        {
            var authors = await _context.GetAuthorsByProduct();
            var lookupDto = authors.Select(item => new LookupDto
            {
                Id = item.Id,
                Name = item.Name
            }).ToList();

            return lookupDto;
        }
        public async Task<List<LookupDto>> GetManufacturersByProduct()
        {
            var manufacturers = await _context.GetManufacturersByProduct();
            var lookupDto = manufacturers.Select(item => new LookupDto
            {
                Id = item.Id,
                Name = item.Name
            }).ToList();

            return lookupDto;
        }




        //    public async Task<FileUploadResult> UploadFileAsync(IFormFile file, string uploadsFolder, string logFilePath)
        //    {
        //        try
        //        {
        //            Log.Logger = new LoggerConfiguration()
        //                .MinimumLevel.Information()
        //                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
        //                .CreateLogger();

        //            if (file == null || file.Length == 0)
        //            {
        //                return new FileUploadResult { Success = false, ErrorMessage = "File is not selected" };
        //            }

        //            string fileExtension = Path.GetExtension(file.FileName);
        //            string[] allowedExtensions = { ".xls", ".xlsx" };

        //            if (!allowedExtensions.Contains(fileExtension.ToLower()))
        //            {
        //                return new FileUploadResult { Success = false, ErrorMessage = "Only Excel files are allowed" };
        //            }

        //            // Ensure uploads folder exists
        //            if (!Directory.Exists(uploadsFolder))
        //            {
        //                Directory.CreateDirectory(uploadsFolder);
        //            }

        //            //// Save file details to database
        //            //var doc = new File
        //            //{
        //            //    FileName = file.FileName,
        //            //    UploadedbyId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
        //            //    UploadedDate = DateTime.Now,
        //            //};

        //            //_db.Files.Add(doc);
        //            //await _db.SaveChangesAsync();

        //            // Save file to disk
        //            var originalFileName = Path.GetFileName(file.FileName);
        //            var filePath = Path.Combine(uploadsFolder, originalFileName);

        //            using (var fileStream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await file.CopyToAsync(fileStream);
        //            }

        //            return new FileUploadResult { Success = true, FileName = originalFileName };
        //        }
        //        catch (Exception ex)
        //        {
        //            // Log error and return failure
        //            Log.Error(ex, "Error uploading file");
        //            return new FileUploadResult { Success = false, ErrorMessage = ex.Message };
        //        }
        //    }
        //}
        public async Task<FileUploadResult> UploadFileAsync(IFormFile file, int shopId)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return new FileUploadResult { Success = false, ErrorMessage = "File is not selected" };
                }

                string fileExtension = Path.GetExtension(file.FileName);
                string[] allowedExtensions = { ".xls", ".xlsx" };

                if (!allowedExtensions.Contains(fileExtension.ToLower()))
                {
                    return new FileUploadResult { Success = false, ErrorMessage = "Only Excel files are allowed" };
                }

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheet(1);

                        var manufacturerValue = GetValue<int?>(worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Manufacturer", StringComparison.OrdinalIgnoreCase)));
                        var engineValue = GetValue<int?>(worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Engine", StringComparison.OrdinalIgnoreCase)));
                        var subCategoryValue = GetValue<int?>(worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("SubCategory", StringComparison.OrdinalIgnoreCase)));
                        var categoryValue = GetValue<int?>(worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Category", StringComparison.OrdinalIgnoreCase)));
                        var manufacturerModelsValue = GetValue<int?>(worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Model", StringComparison.OrdinalIgnoreCase)));
                        var nameValue = GetValue<int?>(worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Name", StringComparison.OrdinalIgnoreCase)));
                        var descriptionValue = GetValue<int?>(worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Description", StringComparison.OrdinalIgnoreCase)));
                        var yearValue = GetValue<int?>(worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Year", StringComparison.OrdinalIgnoreCase)));
                        var mileageValue = GetValue<int?>(worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Mileage", StringComparison.OrdinalIgnoreCase)));
                        var gradeValue = GetValue<string>(worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Grade", StringComparison.OrdinalIgnoreCase)));
                        var damageValue = GetValue<short?>(worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Damage", StringComparison.OrdinalIgnoreCase)));
                        var trimLevelValue = GetValue<string>(worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Trim Level", StringComparison.OrdinalIgnoreCase)));

                        var transmissionValue = GetValue<string>(worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Transmission", StringComparison.OrdinalIgnoreCase)));
                        var drivetrainValue = GetValue<string>(worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Drivetrain", StringComparison.OrdinalIgnoreCase)));
                        var newUsedValue = GetValue<string>(worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("New/Used", StringComparison.OrdinalIgnoreCase)));
                        var oemPartNumberValue = GetValue<string>(worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("OEM Part Number", StringComparison.OrdinalIgnoreCase)));
                        var partslinkNumberValue = GetValue<string>(worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Partslink Number", StringComparison.OrdinalIgnoreCase)));
                        var hollanderIcValue = GetValue<string>(worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Hollander IC", StringComparison.OrdinalIgnoreCase)));
                        var stockNumberValue = GetValue<string>(worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Stock Number", StringComparison.OrdinalIgnoreCase)));
                        var tagNumberValue = GetValue<string>(worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Tag Number", StringComparison.OrdinalIgnoreCase)));
                        var locationValue = GetValue<double?>(worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Location", StringComparison.OrdinalIgnoreCase)));
                        var siteValue = GetValue<string>(worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Site", StringComparison.OrdinalIgnoreCase)));
                        var quantityValue = GetValue<int?>(worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Quantity", StringComparison.OrdinalIgnoreCase)));
                        var vinValue = GetValue<string>(worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("VIN", StringComparison.OrdinalIgnoreCase)));
                        var coreValue = GetValue<int?>(worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Core", StringComparison.OrdinalIgnoreCase)));
                        var colorValue = GetValue<string>(worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Color", StringComparison.OrdinalIgnoreCase)));
                        var priceValue = GetValue<int?>(worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Price", StringComparison.OrdinalIgnoreCase)));

                        // Get column numbers directly after retrieving the column

                        int manufacturerColumnNumber = worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Manufacturer", StringComparison.OrdinalIgnoreCase))?.WorksheetColumn().ColumnNumber() ?? -1;
                        int engineColumnNumber = worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Engine", StringComparison.OrdinalIgnoreCase))?.WorksheetColumn().ColumnNumber() ?? -1;
                        int subCategoryColumnNumber = worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("SubCategory", StringComparison.OrdinalIgnoreCase))?.WorksheetColumn().ColumnNumber() ?? -1;
                        int categoryColumnNumber = worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Category", StringComparison.OrdinalIgnoreCase))?.WorksheetColumn().ColumnNumber() ?? -1;
                        int manufacturerModelsColumnNumber = worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Model", StringComparison.OrdinalIgnoreCase))?.WorksheetColumn().ColumnNumber() ?? -1;

                        int nameColumnNumber = worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Name", StringComparison.OrdinalIgnoreCase))?.WorksheetColumn().ColumnNumber() ?? -1;
                        int descriptionColumnNumber = worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Description", StringComparison.OrdinalIgnoreCase))?.WorksheetColumn().ColumnNumber() ?? -1;
                        int yearColumnNumber = worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Year", StringComparison.OrdinalIgnoreCase))?.WorksheetColumn().ColumnNumber() ?? -1;
                        int mileageColumnNumber = worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Mileage", StringComparison.OrdinalIgnoreCase))?.WorksheetColumn().ColumnNumber() ?? -1;
                        int gradeColumnNumber = worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Grade", StringComparison.OrdinalIgnoreCase))?.WorksheetColumn().ColumnNumber() ?? -1;
                        int damageColumnNumber = worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Damage", StringComparison.OrdinalIgnoreCase))?.WorksheetColumn().ColumnNumber() ?? -1;
                        int trimLevelColumnNumber = worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Trim Level", StringComparison.OrdinalIgnoreCase))?.WorksheetColumn().ColumnNumber() ?? -1;

                        int transmissionColumnNumber = worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Transmission", StringComparison.OrdinalIgnoreCase))?.WorksheetColumn().ColumnNumber() ?? -1;
                        int drivetrainColumnNumber = worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Drivetrain", StringComparison.OrdinalIgnoreCase))?.WorksheetColumn().ColumnNumber() ?? -1;
                        int newUsedColumnNumber = worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("New/Used", StringComparison.OrdinalIgnoreCase))?.WorksheetColumn().ColumnNumber() ?? -1;
                        int oemPartNumberColumnNumber = worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("OEM Part Number", StringComparison.OrdinalIgnoreCase))?.WorksheetColumn().ColumnNumber() ?? -1;
                        int partslinkNumberColumnNumber = worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Partslink Number", StringComparison.OrdinalIgnoreCase))?.WorksheetColumn().ColumnNumber() ?? -1;
                        int hollanderIcColumnNumber = worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Hollander IC", StringComparison.OrdinalIgnoreCase))?.WorksheetColumn().ColumnNumber() ?? -1;
                        int stockNumberColumnNumber = worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Stock Number", StringComparison.OrdinalIgnoreCase))?.WorksheetColumn().ColumnNumber() ?? -1;
                        int tagNumberColumnNumber = worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Tag Number", StringComparison.OrdinalIgnoreCase))?.WorksheetColumn().ColumnNumber() ?? -1;
                        int locationColumnNumber = worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Location", StringComparison.OrdinalIgnoreCase))?.WorksheetColumn().ColumnNumber() ?? -1;
                        int siteColumnNumber = worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Site", StringComparison.OrdinalIgnoreCase))?.WorksheetColumn().ColumnNumber() ?? -1;
                        int quantityColumnNumber = worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Quantity", StringComparison.OrdinalIgnoreCase))?.WorksheetColumn().ColumnNumber() ?? -1;
                        int vinColumnNumber = worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("VIN", StringComparison.OrdinalIgnoreCase))?.WorksheetColumn().ColumnNumber() ?? -1;
                        int coreColumnNumber = worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Core", StringComparison.OrdinalIgnoreCase))?.WorksheetColumn().ColumnNumber() ?? -1;
                        int colorColumnNumber = worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Color", StringComparison.OrdinalIgnoreCase))?.WorksheetColumn().ColumnNumber() ?? -1;
                        int priceColumnNumber = worksheet.FirstRow().CellsUsed().FirstOrDefault(c => c.Value.ToString().Trim().Equals("Price", StringComparison.OrdinalIgnoreCase))?.WorksheetColumn().ColumnNumber() ?? -1;

                        string issue = "";

                        //var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            return new FileUploadResult { Success = false, ErrorMessage = "No worksheet found in Excel file" };
                        }
                        int[] columnNumbers = {
    manufacturerColumnNumber, engineColumnNumber, subCategoryColumnNumber,categoryColumnNumber,manufacturerModelsColumnNumber,nameColumnNumber, descriptionColumnNumber,
    yearColumnNumber, mileageColumnNumber, gradeColumnNumber, damageColumnNumber, trimLevelColumnNumber,
    transmissionColumnNumber, drivetrainColumnNumber, newUsedColumnNumber, oemPartNumberColumnNumber, partslinkNumberColumnNumber,
    hollanderIcColumnNumber, stockNumberColumnNumber, tagNumberColumnNumber, locationColumnNumber, siteColumnNumber, quantityColumnNumber,
    vinColumnNumber, coreColumnNumber, colorColumnNumber, priceColumnNumber
};


                        foreach (var colNum in columnNumbers)
                        {
                            if (colNum < 1 || colNum > 16384)
                            {
                                return new FileUploadResult { Success = false, ErrorMessage = $"Invalid column number: {colNum}" };
                            }
                        }
                        //var products = new List<Product>();


                        foreach (var row in worksheet.RowsUsed().Skip(1))
                        {
                            var manufacturerName = GetValue<string>(row.Cell(manufacturerColumnNumber));
                            var engineName = GetValue<string>(row.Cell(engineColumnNumber));
                            var categoryName = GetValue<string>(row.Cell(categoryColumnNumber));
                            var modelName = GetValue<string>(row.Cell(manufacturerModelsColumnNumber));
                            var subCategoryName = GetValue<string>(row.Cell(subCategoryColumnNumber));

                            int? manufacturerId = null;
                            int? engineId = null;
                            int? categoryId = null;
                            int? modelId = null;
                            int? subCategoryId = null;
                            var name = GetValue<string>(row.Cell(nameColumnNumber));
                            var slug = GenerateSlug(name);
                            var uniqueSlug = await EnsureUniqueSlugAsync(slug, name);

                            if (!string.IsNullOrWhiteSpace(manufacturerName))
                            {
                                manufacturerId = await GetOrCreateManufacturerIdAsync(manufacturerName);
                            }
                            if (!string.IsNullOrWhiteSpace(engineName))
                            {
                                engineId = await GetOrCreateEngineIdAsync(engineName);
                            }

                            if (!string.IsNullOrWhiteSpace(categoryName))
                            {
                                categoryId = await GetOrCreateCategoryIdAsync(categoryName);
                            }

                            if (!string.IsNullOrWhiteSpace(modelName))
                            {
                                modelId = await GetOrCreateModelIdAsync(modelName);
                            }

                            if (!string.IsNullOrWhiteSpace(subCategoryName))
                            {
                                subCategoryId = await GetOrCreateSubCategoryIdAsync(subCategoryName);
                            }

                            var product = new Product
                            {
                                ShopId = shopId,
                                ManufacturerId = manufacturerId,
                                EngineId = engineId,
                                CategoryId = categoryId,
                                SubCategoryId = subCategoryId,
                                ModelId = modelId,

                                Name = GetValue<string>(row.Cell(nameColumnNumber)),
                                Slug = uniqueSlug,
                                Description = GetValue<string>(row.Cell(descriptionColumnNumber)),
                                Year = GetValue<int?>(row.Cell(yearColumnNumber)),
                                Mileage = GetValue<int?>(row.Cell(mileageColumnNumber)),
                                Grade = GetValue<string>(row.Cell(gradeColumnNumber)),
                                Status = "publish",
                                Damage = GetValue<short?>(row.Cell(damageColumnNumber)),
                                TrimLevel = GetValue<string>(row.Cell(trimLevelColumnNumber)),
                                Transmission = GetValue<string>(row.Cell(transmissionColumnNumber)),
                                Drivetrain = GetValue<string>(row.Cell(drivetrainColumnNumber)),
                                NewUsed = GetValue<string>(row.Cell(newUsedColumnNumber)),
                                OemPartNumber = GetValue<string>(row.Cell(oemPartNumberColumnNumber)),
                                PartslinkNumber = GetValue<string>(row.Cell(partslinkNumberColumnNumber)),
                                HollanderIc = GetValue<string>(row.Cell(hollanderIcColumnNumber)),
                                StockNumber = GetValue<string>(row.Cell(stockNumberColumnNumber)),
                                TagNumber = GetValue<string>(row.Cell(tagNumberColumnNumber)),
                                Location = GetValue<double?>(row.Cell(locationColumnNumber)),
                                Site = GetValue<string>(row.Cell(siteColumnNumber)),
                                Quantity = GetValue<int?>(row.Cell(quantityColumnNumber)),
                                Vin = GetValue<string>(row.Cell(vinColumnNumber)),
                                Core = GetValue<int?>(row.Cell(coreColumnNumber)),
                                Color = GetValue<string>(row.Cell(colorColumnNumber)),
                                Price = GetValue<int>(row.Cell(priceColumnNumber)),

                            };

                            await _context.CreateProductAsync(product);
                            await _context.UnitOfWork.SaveChangesAsync();
                        }
                        return new FileUploadResult { Success = true, FileName = file.FileName };
                    }

                }
            }
            catch (Exception ex)
            {
                return new FileUploadResult { Success = false, ErrorMessage = ex.Message };
            }
            //return new FileUploadResult { Success = false, ErrorMessage = "An unexpected error occurred" };
        }

        public async Task<int?> GetOrCreateModelIdAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var model = await _manufactureModelRepository.GetModelByName(name);
            if (model == null)
            {
                model = new ManufacturerModel { Model = name };
                _manufactureModelRepository.AddModel(model);
                await _manufactureModelRepository.UnitOfWork.SaveChangesAsync();
            }
            return model.Id;
        }

        public async Task<int?> GetOrCreateSubCategoryIdAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var sub = await _subCategoryListRepository.GetsubCategoryByName(name);
            if (sub == null)
            {
                sub = new SubCategoryList { Subcategory = name };
                _subCategoryListRepository.AddSubCategory(sub);
                await _subCategoryListRepository.UnitOfWork.SaveChangesAsync();
            }
            return sub.Id;
        }

        public async Task<int?> GetOrCreateEngineIdAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var engine = await _engineRepository.GetEngineByName(name);
            if (engine == null)
            {
                engine = new Engine { Engine1 = name };
                _engineRepository.AddEngine(engine);
                await _engineRepository.UnitOfWork.SaveChangesAsync();
            }
            return engine.Id;
        }



        public async Task<int?> GetOrCreateCategoryIdAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var category = await _categoryRepository.GetCategoryByName(name);
            if (category == null)
            {
                category = new Category { Name = name };
                _categoryRepository.AddCategory(category);
                await _categoryRepository.UnitOfWork.SaveChangesAsync();
            }
            return category.Id;
        }

        public async Task<int?> GetOrCreateManufacturerIdAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var manufacturer = await _manufacturerRepository.GetManufacturerByName(name);
            if (manufacturer == null)
            {
                manufacturer = new Manufacture { Name = name };
                _manufacturerRepository.AddManufacturers(manufacturer);
                await _manufacturerRepository.UnitOfWork.SaveChangesAsync();
            }
            return manufacturer.Id;
        }


        public async Task<MemoryStream> ExportProducts(int shopId)
        {
            try
            {
                var products = await _context.GetProductsByShopId(shopId);

                if (products == null || !products.Any())
                {
                    throw new KeyNotFoundException($"No products found for shopId {shopId}");
                }

                var stream = new MemoryStream();
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.Add("Products");

                    // Headers
                    worksheet.Cells[1, 1].Value = "Manufacturer";
                    worksheet.Cells[1, 2].Value = "Engine";
                    worksheet.Cells[1, 3].Value = "Category";
                    worksheet.Cells[1, 4].Value = "Model";
                    worksheet.Cells[1, 5].Value = "SubCategory";
                    worksheet.Cells[1, 6].Value = "Name";
                    worksheet.Cells[1, 7].Value = "Description";
                    worksheet.Cells[1, 8].Value = "Year";
                    worksheet.Cells[1, 9].Value = "Mileage";
                    worksheet.Cells[1, 10].Value = "Grade";
                    worksheet.Cells[1, 11].Value = "Damage";
                    worksheet.Cells[1, 12].Value = "Trim Level";
                    worksheet.Cells[1, 13].Value = "Transmission";
                    worksheet.Cells[1, 14].Value = "Drivetrain";
                    worksheet.Cells[1, 15].Value = "New/Used";
                    worksheet.Cells[1, 16].Value = "OEM Part Number";
                    worksheet.Cells[1, 17].Value = "Partslink Number";
                    worksheet.Cells[1, 18].Value = "Hollander IC";
                    worksheet.Cells[1, 19].Value = "Stock Number";
                    worksheet.Cells[1, 20].Value = "Tag Number";
                    worksheet.Cells[1, 21].Value = "Location";
                    worksheet.Cells[1, 22].Value = "Site";
                    worksheet.Cells[1, 23].Value = "Quantity";
                    worksheet.Cells[1, 24].Value = "VIN";
                    worksheet.Cells[1, 25].Value = "Core";
                    worksheet.Cells[1, 26].Value = "Color";
                    worksheet.Cells[1, 27].Value = "Price";

                    // Data rows
                    int row = 2;
                    foreach (var product in products)
                    {
                        var manufacturerName = await _context.GetManufacturerNameById(product.Product.ManufacturerId);
                        var engineName = await _context.GetOrCreateEngineIdAsync(product.Product.EngineId);
                        var modelName = await _context.GetModelNameById(product.Product.ModelId);
                        var subCategoryName = await _context.GetSubCategoryNameById(product.Product.SubCategoryId);
                        var categoryName = await _context.GetCategoryNameById(product.Product.CategoryId);

                        worksheet.Cells[row, 1].Value = manufacturerName;
                        worksheet.Cells[row, 2].Value = engineName;
                        worksheet.Cells[row, 3].Value = categoryName;
                        worksheet.Cells[row, 4].Value = modelName;
                        worksheet.Cells[row, 5].Value = subCategoryName;
                        worksheet.Cells[row, 6].Value = product.Product.Name;
                        worksheet.Cells[row, 7].Value = product.Product.Description;
                        worksheet.Cells[row, 8].Value = product.Product.Year;
                        worksheet.Cells[row, 9].Value = product.Product.Mileage;
                        worksheet.Cells[row, 10].Value = product.Product.Grade;
                        worksheet.Cells[row, 11].Value = product.Product.Damage;
                        worksheet.Cells[row, 12].Value = product.Product.TrimLevel;
                        worksheet.Cells[row, 13].Value = product.Product.Transmission;
                        worksheet.Cells[row, 14].Value = product.Product.Drivetrain;
                        worksheet.Cells[row, 15].Value = product.Product.NewUsed;
                        worksheet.Cells[row, 16].Value = product.Product.OemPartNumber;
                        worksheet.Cells[row, 17].Value = product.Product.PartslinkNumber;
                        worksheet.Cells[row, 18].Value = product.Product.HollanderIc;
                        worksheet.Cells[row, 19].Value = product.Product.StockNumber;
                        worksheet.Cells[row, 20].Value = product.Product.TagNumber;
                        worksheet.Cells[row, 21].Value = product.Product.Location;
                        worksheet.Cells[row, 22].Value = product.Product.Site;
                        worksheet.Cells[row, 23].Value = product.Product.Quantity;
                        worksheet.Cells[row, 24].Value = product.Product.Vin;
                        worksheet.Cells[row, 25].Value = product.Product.Core;
                        worksheet.Cells[row, 26].Value = product.Product.Color;
                        worksheet.Cells[row, 27].Value = product.Product.Price;

                        row++;
                    }

                    worksheet.Cells.AutoFitColumns();
                    package.Save();
                }

                stream.Position = 0;
                return stream;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //private IActionResult File(MemoryStream stream, string v, string excelName)
        //{
        //    throw new NotImplementedException();
        //}

        private T GetValue<T>(IXLCell cell)
        {
            try
            {
                if (cell.IsEmpty())
                {
                    return default!;
                }

                return cell.GetValue<T>();
            }
            catch
            {
                return default!;
            }
        }



        public async Task<ActionResult<IEnumerable<ProductCountDto>>> GetProductsSummaryAsync()
        {
            var productCounts = new List<ProductCountDto>();


            var (categories, totalCount) = await _categoryRepository.GetAllCategoriesAsync();
            foreach (var category in categories)
            {
                var productCount = await _context.GetProductByCategoryAsync(category.Id);
                productCounts.Add(new ProductCountDto
                {
                    CatogeryId = category.Id,
                    CategoryName = category.Name,
                    NumberOfProduct = productCount
                });
            }

            var shops = await _shopRepository.GetShops();
            foreach (var shop in shops)
            {
                var productCount = await _context.GetProductByShopAsync(shop.Id);
                productCounts.Add(new ProductCountDto
                {
                    ShopId = shop.Id,
                    ShopName = shop.Name,
                    NumberOfProducts = productCount
                });
            }

            return productCounts;
        }

        public async Task<IEnumerable<ProductDtoResponse>> GetTopRatedProductsAsync(int? shopId = null, int? vendorId = null)
        {
            try
            {
                var shopIds = new List<int>();

                if (vendorId.HasValue)
                {
                    var shops = await _shopRepository.GetShopsByOwnerIdAsync(vendorId.Value);
                    shopIds = shops.Select(s => s.Id).ToList();
                }

                var products = await _context.GetTopRatedProductsAsync(shopId, shopIds);
                var result = new List<ProductDtoResponse>();

                foreach (var product in products)
                {
                    var category = product.CategoryId.HasValue
                        ? await _context.GetCategoryByIdAsync(product.CategoryId.Value)
                        : null;

                    var shopAddress = product.ShopId.HasValue
                        ? await _context.GetShopAddressById(product.ShopId.Value)
                        : null;
                    var shop = product.ShopId.HasValue
                        ? await _context.GetShopByProductId(product.ShopId.Value)
                        : null;
                    var setting = await _context.GetSettingByProductId(product.Id);
                    var rating = await _context.GetRatingByProductId(product.Id);
                    var gallery = await _context.GetGalleryByProductId(product.Id);
                    var promotional = await _context.GetPromotionalByProductId(product.Id);
                    //var shop = await _context.GetShopById(product.ShopId.Value);

                    var image = product.Image;
                    decimal shippingCharges = 0;
                    if (category != null && product.ShopId.HasValue)
                    {
                        var svcRelation = await _context.GetSVCRelationByShopIdAndSizeAsync(product.ShopId.Value, category.Size);
                        if (svcRelation != null)
                        {
                            shippingCharges = svcRelation.Price ?? 0;
                        }
                    }
                    decimal totalPrice = (product.Price ?? 0) + shippingCharges;

                    var shopAddressDto = shopAddress != null
                        ? new ShopAddressDto
                        {
                            Id = shopAddress.Id,
                            Country = shopAddress.Country,
                            City = shopAddress.City,
                            State = shopAddress.State,
                            Zip = shopAddress.Zip,
                            StreetAddress = shopAddress.StreetAddress,
                            ShopId = shopAddress.ShopId
                        }
                        : null;

                    var ratingDto = rating != null
                        ? new RatingDto
                        {
                            Rating1 = rating.Rating1,
                            PositiveFeedbacksCount = rating.PositiveFeedbacksCount,
                            NegativeFeedbacksCount = rating.NegativeFeedbacksCount,
                            AbusiveReportsCount = rating.AbusiveReportsCount,
                            Total = rating.Total
                        }
                        : null;

                    var categoryDto = category != null
                        ? new CategoryDto
                        {
                            Id = category.Id,
                            Name = category.Name,
                            Slug = category.Slug,
                            Icon = category.Icon,
                            ParentId = category.ParentId,
                            Language = category.Language,
                            CreatedAt = category.CreatedAt,
                            UpdatedAt = category.UpdatedAt,
                            DeletedAt = category.DeletedAt,
                            TypeId = category.TypeId
                        }
                        : null;

                    var settingDto = setting != null
                        ? new SettingDto
                        {
                            Contact = setting.Contact,
                            Website = setting.Website,
                            LocationLat = setting.LocationLat,
                            LocationLng = setting.LocationLng,
                            LocationCity = setting.LocationCity,
                            LocationState = setting.LocationState,
                            LocationCountry = setting.LocationCountry,
                            LocationFormattedAddress = setting.LocationFormattedAddress,
                            LocationZip = setting.LocationZip
                        }
                        : null;

                    string logoUrl = null!;
                    if (shop != null)
                    {
                        if (shop.LogoImageId.HasValue)
                        {
                            var logo = await _shopRepository.GetImageById(shop.LogoImageId.Value);
                            if (logo != null && _rootFolder != null && !string.IsNullOrEmpty(_rootFolder.ApplicationUrl))
                            {
                                logoUrl = _rootFolder.ApplicationUrl + logo;
                            }
                        }
                    }

                    var shopDto = shop != null
                        ? new ShopDto
                        {
                            Id = shop.Id,
                            Name = shop.Name,
                            Slug = shop.Slug,
                            Description = shop.Description,
                            IsActive = shop.IsActive,
                            Logo = logoUrl!,
                            OwnerId = shop.OwnerId
                        }
                        : null;

                    var promotionalSliderDto = promotional != null
                        ? new PromotionalSliderDto
                        {
                            Id = promotional.Id,
                            OriginalUrl = promotional.OriginalUrl,
                            ThumbnailUrl = promotional.ThumbnailUrl
                        }
                        : null;

                    var productGalleryImageDto = gallery != null
                        ? new ProductGalleryImageDto
                        {
                            Id = gallery.Id,
                            OriginalUrl = gallery.OriginalUrl,
                            ThumbnailUrl = gallery.ThumbnailUrl
                        }
                        : null;

                    var imageDto = image != null
                        ? new ImageDto
                        {
                            Id = image.Id,
                            OriginalUrl = image.OriginalUrl,
                            ThumbnailUrl = image.ThumbnailUrl
                        }
                        : null;

                    var res = new ProductDtoResponse
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Slug = product.Slug,
                        Description = product.Description,
                        SubPrice = product.Price,
                        TypeId = product.TypeId,
                        ShopId = product.ShopId,
                        SalePrice = product.SalePrice,
                        Language = product.Language,
                        MaxPrice = product.MaxPrice,
                        MinPrice = product.MinPrice,
                        Quantity = product.Quantity,
                        Sku = product.Sku,
                        InStock = product.InStock,
                        IsTaxable = product.IsTaxable,
                        ShippingClassId = product.ShippingClassId,
                        Status = product.Status,
                        ProductType = product.ProductType,
                        Unit = product.Unit,
                        Height = product.Height,
                        Width = product.Width,
                        Length = product.Length,
                        DeletedAt = product.DeletedAt,
                        CreatedAt = product.CreatedAt,
                        UpdatedAt = product.UpdatedAt,
                        TotalReviews = product.TotalReviews,
                        ManufacturerId = product.ManufacturerId,
                        IsDigital = product.IsDigital,
                        ExternalProductButtonText = product.ExternalProductButtonText,
                        ExternalProductUrl = product.ExternalProductUrl,
                        IsExternal = product.IsExternal,
                        Ratings = product.Ratings,
                        MyReview = product.MyReview,
                        ImageId = product.ImageId,
                        AuthorId = product.AuthorId,
                        InWishlist = product.InWishlist,
                        Year = product.Year,
                        Model = product.Model,
                        Mileage = product.Mileage,
                        Grade = product.Grade,
                        Damage = product.Damage,
                        TrimLevel = product.TrimLevel,
                        EngineId = product.EngineId,
                        Transmission = product.Transmission,
                        Drivetrain = product.Drivetrain,
                        NewUsed = product.NewUsed,
                        OemPartNumber = product.OemPartNumber,
                        PartslinkNumber = product.PartslinkNumber,
                        HollanderIc = product.HollanderIc,
                        StockNumber = product.StockNumber,
                        TagNumber = product.TagNumber,
                        Location = product.Location,
                        Site = product.Site,
                        Vin = product.Vin,
                        Core = product.Core,
                        Color = product.Color,
                        ShippingCharges = shippingCharges,
                        Price = totalPrice,

                        ImageDto = imageDto,
                        SettingDto = settingDto!,
                        ShopAddressDto = shopAddressDto,
                        CategoryDto = categoryDto!,
                        PromotionalSliderDto = promotionalSliderDto,
                        ShopDto = shopDto,
                        ProductGalleryImageDto = productGalleryImageDto,
                        RatingDto = ratingDto
                    };

                    result.Add(res);
                }
                return result;
            }

            catch (Exception)
            {
                throw;
            }
        }

        private string GenerateSlug(string name)
        {
            return name.ToLower().Replace(" ", "-");
        }

        //private async Task<string> EnsureUniqueSlugAsync(string slug)
        //{
        //    int counter = 1;
        //    string originalSlug = slug;

        //    while (await _context.SlugExistsAsync(slug))
        //    {
        //        slug = $"{originalSlug}-{counter}";
        //        counter++;
        //    }

        //    return slug;
        //}
        private async Task<string> EnsureUniqueSlugAsync(string name, string requestedSlug)
        {
            // Generate the initial slug from the requested name
            string slug = GenerateSlug(requestedSlug);

            // If the requested slug is not found in the database, return it directly
            if (!await _context.SlugExistsAsync(slug))
            {
                return slug;
            }

            // Otherwise, append a counter until a unique slug is found
            int counter = 1;
            string originalSlug = slug;

            while (await _context.SlugExistsAsync(slug))
            {
                slug = $"{originalSlug}-{counter}";
                counter++;
            }

            return slug;
        }

        //

    }
}
