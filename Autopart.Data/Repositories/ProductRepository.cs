using Autopart.Domain.CommonDTO;
using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Autopart.Domain.Enum.EnumAndConsonent;

namespace Autopart.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly autopartContext _context;

        public IUnitOfWork UnitOfWork => _context;

        public ProductRepository(autopartContext context)
        {
            _context = context;
        }
        public async Task<Svcrelation> GetSVCRelationByShopIdAndSizeAsync(int shopId, string size)
        {
            return await _context.Svcrelations
                .Where(s => s.ShopId == shopId && s.Size == size)
                .FirstOrDefaultAsync() ?? new Svcrelation();
        }

        public async Task<Product> GetWishlistProductAsync(int userId, int productId)
        {
            return await _context.UserWishlists
                .Where(uw => uw.UserId == userId && uw.ProductId == productId)
                .Include(uw => uw.Product)
                .ThenInclude(p => p.Image)
                .Select(uw => uw.Product)
                .FirstOrDefaultAsync() ?? new Product();
        }

        public async Task<SubCategoryList> GetSubCategoryByIdAsync(int id)
        {
            return await _context.SubCategoryLists.FindAsync(id) ?? new SubCategoryList();
        }

        public async Task<ManufacturerModel> GetManufactureModelByIdAsync(int id)
        {
            return await _context.ManufacturerModels.FindAsync(id) ?? new ManufacturerModel();
        }

        public async Task<Engine> GetEngineByIdAsync(int id)
        {
            return await _context.Engines.FindAsync(id) ?? new Engine();
        }

        public async Task<IEnumerable<GetWishlistProductsDto>> GetWishlistProductsByUserIdAsync(int userId)
        {
            var query = from uw in _context.UserWishlists
                        join p in _context.Products on uw.ProductId equals p.Id into productJoined
                        from p in productJoined.DefaultIfEmpty()
                        join i in _context.Images on p.ImageId equals i.Id into imageJoined
                        from i in imageJoined.DefaultIfEmpty()
                        where uw.UserId == userId
                        select new GetWishlistProductsDto
                        {
                            UserWishlist = uw,
                            Product = p,
                            Image = i
                        };

            return await query.ToListAsync();
        }



        public async Task<IEnumerable<Product>> GetWishlistProductsByProductIdAsync(int productId)
        {

            var result = await _context.UserWishlists
                .Where(uw => uw.ProductId == productId)
                    .Include(uw => uw.Product)
                .ThenInclude(p => p.Image)
                .Select(uw => uw.Product)
                .ToListAsync();

            return result;
        }




        public async Task<bool> DeleteWishlistProductsByProductIdAsync(int productId)
        {
            var wishlistEntries = _context.UserWishlists.Where(uw => uw.ProductId == productId).ToList();

            if (!wishlistEntries.Any())
            {
                return false;
            }
            _context.UserWishlists.RemoveRange(wishlistEntries);
            await _context.SaveChangesAsync();

            return true;
        }





        public async Task AddProductToWishlistAsync(int userId, int productId)
        {
            if (await _context.UserWishlists.AnyAsync(uw => uw.UserId == userId && uw.ProductId == productId))
            {
                throw new InvalidOperationException("Product is already in the wishlist.");
            }

            var wishlistItem = new UserWishlist
            {
                UserId = userId,
                ProductId = productId
            };

            _context.UserWishlists.Add(wishlistItem);
            await _context.SaveChangesAsync();
        }

        public async Task<(IEnumerable<ProductJoinDto> Products, int TotalCount)> GetProductsAsync(GetProductsDto getProductsDto)
        {
            IQueryable<ProductJoinDto> query = from product in _context.Products
                                               join category in _context.Categories on product.CategoryId equals category.Id into categoryJoined
                                               from category in categoryJoined.DefaultIfEmpty()
                                               join shop in _context.Shops on product.ShopId equals shop.Id into shopJoined
                                               from shop in shopJoined.DefaultIfEmpty()
                                               join rating in _context.Ratings on product.Id equals rating.ProductId into ratingJoined
                                               from rating in ratingJoined.DefaultIfEmpty()
                                               join gallery in _context.Galleries on product.Id equals gallery.ProductId into galleryJoined
                                               from gallery in galleryJoined.DefaultIfEmpty()
                                               join promotional in _context.PromotionalSliders
                                               on product.PromotionalSliderId equals promotional.Id into promotionalJoined
                                               from promotional in promotionalJoined.DefaultIfEmpty()
                                               join manufacture in _context.Manufactures
                                               on product.ManufacturerId equals manufacture.Id into manufacturerJoined
                                               from manufacture in manufacturerJoined.DefaultIfEmpty()
                                               join setting in _context.Settings on shop.Id equals setting.ShopId into settingGrouped
                                               from setting in settingGrouped.DefaultIfEmpty()
                                               join coverImage in _context.Images on shop.CoverImageId equals coverImage.Id into coverImageGrouped
                                               from coverImage in coverImageGrouped.DefaultIfEmpty()
                                               join logoImage in _context.Images on shop.LogoImageId equals logoImage.Id into logoImageGrouped
                                               from logoImage in logoImageGrouped.DefaultIfEmpty()
                                               join address in _context.Addresses on shop.Id equals address.ShopId into addressGrouped
                                               from address in addressGrouped.DefaultIfEmpty()
                                               join productImage in _context.Images on product.ImageId equals productImage.Id into productImageJoined
                                               from productImage in productImageJoined.DefaultIfEmpty()
                                               select new ProductJoinDto
                                               {
                                                   Product = product,
                                                   Category = category,
                                                   Shop = shop,
                                                   Rating = rating,
                                                   Gallery = gallery,
                                                   Promotional = promotional,
                                                   Manufacturer = manufacture,
                                                   Setting = setting,
                                                   CoverImage = coverImage,
                                                   LogoImage = logoImage,
                                                   Addresss = address,
                                                   ProductImage = productImage
                                               };

            query = query.Where(q => !string.IsNullOrEmpty(q.Product.HollanderIc));

            if (getProductsDto.isHome && !(getProductsDto.shopId.HasValue || !string.IsNullOrEmpty(getProductsDto.searchByName) || getProductsDto.categoryId.HasValue || !string.IsNullOrEmpty(getProductsDto.model) || getProductsDto.subCategoryId.HasValue || !string.IsNullOrEmpty(getProductsDto.vin) || !string.IsNullOrEmpty(getProductsDto.manufacturer) || getProductsDto.modelId.HasValue || getProductsDto.manufacturerId.HasValue || getProductsDto.year.HasValue || getProductsDto.engineId.HasValue))
            {
                return (Enumerable.Empty<ProductJoinDto>(), 0);
            }
            if (getProductsDto.isHome)
            {
                if (getProductsDto.shopId.HasValue)
                {
                    query = query.Where(q => q.Product.ShopId == getProductsDto.shopId.Value);
                }
                if (!string.IsNullOrEmpty(getProductsDto.searchByName))
                {
                    query = query.Where(q => q.Product.Name.ToLower().Contains(getProductsDto.searchByName.ToLower()));
                }
                if (getProductsDto.modelId.HasValue)
                {
                    query = query.Where(q => q.Product.ModelId == getProductsDto.modelId.Value);
                }
                if (getProductsDto.manufacturerId.HasValue)
                {
                    query = query.Where(q => q.Product.ManufacturerId == getProductsDto.manufacturerId.Value);
                }
                if (getProductsDto.year.HasValue)
                {
                    query = query.Where(q => q.Product.Year == getProductsDto.year.Value);
                }
                if (getProductsDto.engineId.HasValue)
                {
                    query = query.Where(q => q.Product.EngineId == getProductsDto.engineId.Value);
                }
                if (getProductsDto.categoryId.HasValue)
                {
                    query = query.Where(q => q.Product.CategoryId == getProductsDto.categoryId.Value);
                }
                if (!string.IsNullOrEmpty(getProductsDto.model))
                {
                    query = query.Where(q => q.Product.Model.ToLower().Contains(getProductsDto.model.ToLower()));
                }
                if (getProductsDto.subCategoryId.HasValue)
                {
                    query = query.Where(q => q.Product.SubCategoryId == getProductsDto.subCategoryId.Value);
                }
                if (!string.IsNullOrEmpty(getProductsDto.vin))
                {
                    query = query.Where(q => q.Product.Vin.ToLower().Contains(getProductsDto.vin.ToLower()));
                }
                if (!string.IsNullOrEmpty(getProductsDto.manufacturer))
                {
                    query = query.Where(q => q.Manufacturer.Slug.ToLower().Contains(getProductsDto.manufacturer.ToLower()));
                }
            }




            if (getProductsDto.shopId.HasValue)
            {
                query = query.Where(q => q.Product.ShopId == getProductsDto.shopId.Value);
            }
            if (!string.IsNullOrEmpty(getProductsDto.searchByName))
            {
                query = query.Where(q => q.Product.Name.ToLower().Contains(getProductsDto.searchByName.ToLower()));
            }
            if (getProductsDto.modelId.HasValue)
            {
                query = query.Where(q => q.Product.ModelId == getProductsDto.modelId.Value);
            }
            if (getProductsDto.manufacturerId.HasValue)
            {
                query = query.Where(q => q.Product.ManufacturerId == getProductsDto.manufacturerId.Value);
            }
            if (getProductsDto.year.HasValue)
            {
                query = query.Where(q => q.Product.Year == getProductsDto.year.Value);
            }
            if (getProductsDto.engineId.HasValue)
            {
                query = query.Where(q => q.Product.EngineId == getProductsDto.engineId.Value);
            }
            if (getProductsDto.categoryId.HasValue)
            {
                query = query.Where(q => q.Product.CategoryId == getProductsDto.categoryId.Value);
            }
            if (!string.IsNullOrEmpty(getProductsDto.model))
            {
                query = query.Where(q => q.Product.Model.ToLower().Contains(getProductsDto.model.ToLower()));
            }
            if (getProductsDto.subCategoryId.HasValue)
            {
                query = query.Where(q => q.Product.SubCategoryId == getProductsDto.subCategoryId.Value);
            }
            if (!string.IsNullOrEmpty(getProductsDto.vin))
            {
                query = query.Where(q => q.Product.Vin.ToLower().Contains(getProductsDto.vin.ToLower()));
            }
            if (!string.IsNullOrEmpty(getProductsDto.manufacturer))
            {
                query = query.Where(q => q.Manufacturer.Slug.ToLower().Contains(getProductsDto.manufacturer.ToLower()));
            }

            switch (getProductsDto.sortedBy)
            {
                case SortedByProductName.ProductName:
                    query = getProductsDto.orderBy == OrderBy.Ascending ? query.OrderBy(q => q.Product.Name) : query.OrderByDescending(q => q.Product.Name);
                    break;
                default:
                    query = query.OrderBy(q => q.Product.Id);
                    break;
            }

            var totalCount = await query.CountAsync();

            var products = await query.Skip((getProductsDto.pageNumber - 1) * getProductsDto.pageSize)
                                      .Take(getProductsDto.pageSize)
                                      .ToListAsync();

            return (products, totalCount);
        }



        public async Task<IEnumerable<ProductJoinDto>> GetProductsBelowStockAsync(int threshold)
        {
            var query = from product in _context.Products
                        where product.Quantity < threshold
                        join category in _context.Categories on product.CategoryId equals category.Id into categoryJoined
                        from category in categoryJoined.DefaultIfEmpty()
                        join shop in _context.Shops on product.ShopId equals shop.Id into shopJoined
                        from shop in shopJoined.DefaultIfEmpty()
                        join rating in _context.Ratings on product.Id equals rating.ProductId into ratingJoined
                        from rating in ratingJoined.DefaultIfEmpty()
                        join gallery in _context.Galleries on product.Id equals gallery.ProductId into galleryJoined
                        from gallery in galleryJoined.DefaultIfEmpty()
                        join promotional in _context.PromotionalSliders on product.PromotionalSliderId equals promotional.Id into promotionalJoined
                        from promotional in promotionalJoined.DefaultIfEmpty()
                        join manufacture in _context.Manufactures on product.ManufacturerId equals manufacture.Id into manufacturerJoined
                        from manufacture in manufacturerJoined.DefaultIfEmpty()
                        join setting in _context.Settings on shop.Id equals setting.ShopId into settinggrouped
                        from setting in settinggrouped.DefaultIfEmpty()
                        join coverImage in _context.Images on shop.CoverImageId equals coverImage.Id into coverImagegrouped
                        from coverImage in coverImagegrouped.DefaultIfEmpty()
                        join logoImage in _context.Images on shop.LogoImageId equals logoImage.Id into logoImagegrouped
                        from logoImage in logoImagegrouped.DefaultIfEmpty()
                        join address in _context.Addresses on shop.Id equals address.ShopId into addressgrouped
                        from address in addressgrouped.DefaultIfEmpty()
                        join productImage in _context.Images on product.ImageId equals productImage.Id into productImageJoined
                        from productImage in productImageJoined.DefaultIfEmpty()
                        join subcategory in _context.SubCategoryLists on product.SubCategoryId equals subcategory.Id into subcategoryJoined
                        from subcategory in subcategoryJoined.DefaultIfEmpty()
                        select new ProductJoinDto
                        {
                            Product = product,
                            Category = category,
                            Shop = shop,
                            Rating = rating,
                            Gallery = gallery,
                            Promotional = promotional,
                            Manufacturer = manufacture,
                            Setting = setting,
                            CoverImage = coverImage,
                            LogoImage = logoImage,
                            Addresss = address,
                            ProductImage = productImage
                        };

            return await query.ToListAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int categoryId)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId) ?? new Category();
        }

        public async Task<List<Shop>> GetShopsWithCategoriesAndProductsAsync()
        {
            return await _context.Shops
                .Include(s => s.Products)
                    .ThenInclude(p => p.Category)
                .ToListAsync();
        }

        public async Task<List<Category>> GetCategorieAsync()
        {
            return await _context.Categories.Include(c => c.Products).ToListAsync();
        }


        public async Task<PromotionalSlider> GetPromotionalByProductId(int productId)
        {
            return await _context.PromotionalSliders.FirstOrDefaultAsync(c => c.Id == productId) ?? new PromotionalSlider();
        }

        public async Task<string> GetImageById(int? imageId)
        {
            var image = await _context.Images.FirstOrDefaultAsync(x => x.Id == imageId);
            return image.OriginalUrl;
        }

        public async Task<Gallery> GetGalleryByProductId(int productId)
        {
            var result = await _context.Galleries.FirstOrDefaultAsync(c => c.Id == productId) ?? new Gallery();
            return result;
        }



        public async Task<Shop> GetShopByProductId(int productId)
        {
            return await _context.Shops.FirstOrDefaultAsync(c => c.Id == productId) ?? new Shop();
        }

        public async Task<Shop> GetShopById(int id)
        {
            return await _context.Shops.FirstOrDefaultAsync(c => c.Id == id) ?? new Shop();
        }
        public async Task<List<Shop>> GetShopsByProduct()
        {
            return await _context.Shops.ToListAsync();
        }
        public async Task<List<Category>> GetCategoriesByProduct(List<int> shopIds = null)
        {
            return await _context.Categories.ToListAsync();
        }
        public async Task<List<Tag>> GetTagsByProduct()
        {
            return await _context.Tags.ToListAsync();
        }
        public async Task<List<Domain.Models.Author>> GetAuthorsByProduct()
        {
            return await _context.Authors.ToListAsync();
        }
        public async Task<List<Manufacture>> GetManufacturersByProduct()
        {
            return await _context.Manufactures.ToListAsync();
        }

        public async Task<string> GetManufacturerNameById(int? manufacturerId)
        {
            if (manufacturerId == null)
            {
                return null!;
            }
            var manufacturer = await _context.Manufactures.FirstOrDefaultAsync(m => m.Id == manufacturerId.Value);

            return manufacturer!.Name;
        }


        public async Task<string> GetOrCreateEngineIdAsync(int? engineId)
        {
            if (engineId == null)
            {
                return null;
            }

            var engine = await _context.Engines
                .FirstOrDefaultAsync(m => m.Id == engineId.Value);

            return engine!.Engine1;
        }
        public async Task<string> GetModelNameById(int? modelId)
        {
            if (modelId == null)
            {
                return null;
            }

            var model = await _context.ManufacturerModels
                .FirstOrDefaultAsync(m => m.Id == modelId.Value);

            return model?.Model;
        }

        public async Task<string> GetSubCategoryNameById(int? subCategoryId)
        {
            if (subCategoryId == null)
            {
                return null;
            }

            var subCatgory = await _context.SubCategoryLists
                .FirstOrDefaultAsync(m => m.Id == subCategoryId.Value);

            return subCatgory?.Subcategory;
        }


        public async Task<string> GetCategoryNameById(int? categoryId)
        {
            if (categoryId == null)
            {
                return null;
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == categoryId.Value);

            return category?.Name;
        }

        public async Task<Address> GetShopAddressById(int productId)
        {
            return await _context.Addresses.FirstOrDefaultAsync(a => a.Id == productId) ?? new Address();
        }


        public async Task<Shop> GetsShopById(int productId)
        {
            return await _context.Shops.FirstOrDefaultAsync(s => s.Id == productId) ?? new Shop();
        }
        public async Task<IEnumerable<ProductJoinDto>> GetProductsByShopId(int shopId)
        {
            var query = from product in _context.Products
                        join category in _context.Categories on product.CategoryId equals category.Id into categoryJoined
                        from category in categoryJoined.DefaultIfEmpty()
                        join shop in _context.Shops on product.ShopId equals shop.Id into shopJoined
                        from shop in shopJoined.DefaultIfEmpty()
                        join rating in _context.Ratings on product.Id equals rating.ProductId into ratingJoined
                        from rating in ratingJoined.DefaultIfEmpty()
                        join gallery in _context.Galleries on product.Id equals gallery.ProductId into galleryJoined
                        from gallery in galleryJoined.DefaultIfEmpty()
                        join promotional in _context.PromotionalSliders on product.PromotionalSliderId equals promotional.Id into promotionalJoined
                        from promotional in promotionalJoined.DefaultIfEmpty()
                        join manufacture in _context.Manufactures on product.ManufacturerId equals manufacture.Id into manufacturerJoined
                        from manufacture in manufacturerJoined.DefaultIfEmpty()
                        join setting in _context.Settings on shop.Id equals setting.ShopId into settinggrouped
                        from setting in settinggrouped.DefaultIfEmpty()
                        join coverImage in _context.Images on shop.CoverImageId equals coverImage.Id into coverImagegrouped
                        from coverImage in coverImagegrouped.DefaultIfEmpty()
                        join logoImage in _context.Images on shop.LogoImageId equals logoImage.Id into logoImagegrouped
                        from logoImage in logoImagegrouped.DefaultIfEmpty()
                        join address in _context.Addresses on shop.Id equals address.ShopId into addressgrouped
                        from address in addressgrouped.DefaultIfEmpty()
                        join productImage in _context.Images on product.ImageId equals productImage.Id into productImageJoined
                        from productImage in productImageJoined.DefaultIfEmpty()
                        where product.ShopId == shopId
                        select new ProductJoinDto
                        {
                            Product = product,
                            Category = category,
                            Shop = shop,
                            Rating = rating,
                            Gallery = gallery,
                            Promotional = promotional,
                            Manufacturer = manufacture,
                            Setting = setting,
                            CoverImage = coverImage,
                            LogoImage = logoImage,
                            Addresss = address,
                            ProductImage = productImage
                        };

            return await query.ToListAsync();
        }





        public async Task<Setting> GetSettingByProductId(int productId)
        {
            return await _context.Settings.FirstOrDefaultAsync(s => s.ShopId == productId) ?? new Setting();
        }

        public async Task<Image> GetImageByProductId(int productId)
        {
            return await _context.Images.FirstOrDefaultAsync(s => s.Id == productId) ?? new Image();
        }

        public async Task<Image?> GetImageById(int id)
        {
            return await _context.Images.FindAsync(id);
        }

        public async Task<Rating> GetRatingByProductId(int productId)
        {
            return await _context.Ratings.FirstOrDefaultAsync(s => s.ProductId == productId) ?? new Rating();
        }

        public async Task<Category> AddCategoryAsync(Category category)
        {
            try
            {
                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();
                return category;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Domain.Models.Author> AddAuthorAsync(Domain.Models.Author author)
        {
            try
            {
                await _context.Authors.AddAsync(author);
                await _context.SaveChangesAsync();
                return author;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Shop> AddShopAsync(Shop shop)
        {
            try
            {
                await _context.Shops.AddAsync(shop);
                await _context.SaveChangesAsync();
                return shop;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Manufacture> AddManufactureAsync(Manufacture manufacture)
        {
            try
            {
                await _context.Manufactures.AddAsync(manufacture);
                await _context.SaveChangesAsync();
                return manufacture;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Tag> AddTagAsync(Tag tag)
        {
            try
            {
                await _context.Tags.AddAsync(tag);
                await _context.SaveChangesAsync();
                return tag;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            try
            {
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
                return product;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<ProductTag>> CreateProductTagsAsync(List<ProductTag> productTag)
        {
            await _context.ProductTags.AddRangeAsync(productTag);
            await _context.SaveChangesAsync();
            return productTag;
        }

        public async Task<Gallery> CreateGalleryAsync(Gallery gallery)
        {
            try
            {
                await _context.Galleries.AddAsync(gallery);
                await _context.SaveChangesAsync();
                return gallery;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Galleries)
                .FirstOrDefaultAsync(p => p.Id == id) ?? new Product();
        }



        public async Task UpdateProductAsync(Product product)
        {
            try
            {
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<Image> UpdateImage(Image image)
        {
            _context.Images.Update(image);
            return image;
        }

        public async Task<Image> AddImage(Image image)
        {
            await _context.Images.AddAsync(image);
            await _context.SaveChangesAsync();
            return image;
        }


        public async Task UpdateCategoryAsync(Category category)
        {
            try
            {
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task UpdateAuthorAsync(Domain.Models.Author author)
        {
            try
            {
                _context.Authors.Update(author);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task UpdateTagAsync(Tag tag)
        {
            try
            {
                _context.Tags.Update(tag);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task UpdateGalleryAsync(Gallery gallery)
        {
            try
            {
                _context.Galleries.Update(gallery);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task DeleteImages(int productId)
        {
            var product = await _context.Products.Where(i => i.Id == productId).FirstOrDefaultAsync();
            var imagesToDelete = new List<Image>();
            var coverImage = await _context.Images.Where(x => x.Id == product.ImageId).FirstOrDefaultAsync();
            if (coverImage != null)
            {
                imagesToDelete.Add(coverImage);
            }
        }

        public async Task DeleteGalleriesAsync(int productId)
        {
            try
            {
                var galleries = await _context.Galleries.Where(g => g.ProductId == productId).ToListAsync();
                if (galleries.Any())
                {
                    _context.Galleries.RemoveRange(galleries);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task DeleteProductTagsByProductIdAsync(int productId)
        {
            try
            {
                var productTags = await _context.ProductTags.Where(pt => pt.ProductId == productId).ToListAsync();
                if (productTags.Any())
                {
                    _context.ProductTags.RemoveRange(productTags);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        public async Task<bool> RemoveProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }


        public async Task<IEnumerable<Product>> GetBestSellingProductsAsync(int? shopId = null, List<int> shopIds = null)
        {
            IQueryable<Product> query = _context.Products.Where(p => p.Ratings >= 3);
            //return await _context.Products.Where(p => p.Ratings >= 3).ToListAsync();
            if (shopId.HasValue)
            {
                query = query.Where(p => p.ShopId == shopId);
            }
            else if (shopIds != null && shopIds.Any())
            {
                query = query.Where(p => shopIds.Contains(p.ShopId.Value));
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetDraftProductsAsync()
        {
            return await _context.Products.Where(p => p.Status == "draft").ToListAsync();
        }




        public async Task<IEnumerable<Product>> GetPopularProductsAsync(int? shopId = null, List<int> shopIds = null)
        {
            IQueryable<Product> query = _context.Products.Where(p => p.Ratings >= 3);
            //return await _context.Products.Where(p => p.Ratings >= 3).ToListAsync();
            if (shopId.HasValue)
            {
                query = query.Where(p => p.ShopId == shopId);
            }
            else if (shopIds != null && shopIds.Any())
            {
                query = query.Where(p => shopIds.Contains(p.ShopId.Value));
            }

            return await query.ToListAsync();
        }


        public async Task<List<Tag>> GetTagsByProductIdAsync(int productId)
        {
            return await _context.Tags
                .Where(t => t.Products.Any(p => p.Id == productId))
                .ToListAsync();
        }


        public async Task<IEnumerable<ProductJoinDto>> GetProductBySlugAsync(string slug)
        {
            var query = from product in _context.Products
                        join category in _context.Categories on product.CategoryId equals category.Id into categoryJoined
                        from category in categoryJoined.DefaultIfEmpty()
                        join shop in _context.Shops on product.ShopId equals shop.Id into shopJoined
                        from shop in shopJoined.DefaultIfEmpty()
                        join rating in _context.Ratings on product.Id equals rating.ProductId into ratingJoined
                        from rating in ratingJoined.DefaultIfEmpty()
                        join gallery in _context.Galleries on product.Id equals gallery.ProductId into galleryJoined
                        from gallery in galleryJoined.DefaultIfEmpty()
                        join promotional in _context.PromotionalSliders
                        on product.PromotionalSliderId equals promotional.Id into promotionalJoined
                        from promotional in promotionalJoined.DefaultIfEmpty()
                        join manufacture in _context.Manufactures on product.ManufacturerId equals manufacture.Id into manufacturerJoined
                        from manufacture in manufacturerJoined.DefaultIfEmpty()
                        join setting in _context.Settings on shop.Id equals setting.ShopId into settinggrouped
                        from setting in settinggrouped.DefaultIfEmpty()
                        join coverImage in _context.Images on shop.CoverImageId equals coverImage.Id into coverImagegrouped
                        from coverImage in coverImagegrouped.DefaultIfEmpty()
                        join logoImage in _context.Images on shop.LogoImageId equals logoImage.Id into logoImagegrouped
                        from logoImage in logoImagegrouped.DefaultIfEmpty()
                        join address in _context.Addresses on shop.Id equals address.ShopId into addressgrouped
                        from address in addressgrouped.DefaultIfEmpty()
                        join productImage in _context.Images on product.ImageId equals productImage.Id into productImageJoined
                        from productImage in productImageJoined.DefaultIfEmpty()
                        join subcategoryList in _context.SubCategoryLists
                        on product.SubCategoryId equals subcategoryList.Id into subcategoryJoined
                        from subcategoryList in subcategoryJoined.DefaultIfEmpty()
                        join mm in _context.ManufacturerModels on product.ModelId equals mm.Id into mmJoined
                        from mm in mmJoined.DefaultIfEmpty()
                        join engine in _context.Engines on product.EngineId equals engine.Id into enginegrouped
                        from engine in enginegrouped.DefaultIfEmpty()
                        where product.Slug == slug
                        select new ProductJoinDto
                        {
                            Product = product,
                            Category = category,
                            Shop = shop,
                            Rating = rating,
                            Gallery = gallery,
                            Promotional = promotional,
                            Manufacturer = manufacture,
                            Setting = setting,
                            CoverImage = coverImage,
                            LogoImage = logoImage,
                            Addresss = address,
                            SubCategoryList = subcategoryList,
                            Engine = engine,
                            ManufacturerModel = mm,
                            ProductImage = productImage
                        };

            return await query.ToListAsync();
        }


        public async Task<IEnumerable<Product>> GetProductsStockAsync()
        {
            return await _context.Products.Where(p => p.Quantity > 0).ToListAsync();
        }

        public async Task<int> GetProductByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId && p.Quantity > 0)
                .CountAsync();
        }

        public async Task<int> GetProductByShopAsync(int shopId)
        {
            return await _context.Products
                .Where(p => p.ShopId == shopId && p.Quantity > 0)
                .CountAsync();
        }

        public async Task<IEnumerable<Product>> GetTopRatedProductsAsync(int? shopId = null, List<int> shopIds = null)
        {
            IQueryable<Product> query = _context.Products.Where(p => p.Ratings >= 3).OrderByDescending(p => p.Ratings).Take(10);
            //return await _context.Products.Where(p => p.Ratings >= 3).OrderByDescending(p => p.Ratings)  .Take(10).ToListAsync();

            if (shopId.HasValue)
            {
                query = query.Where(p => p.ShopId == shopId);
            }
            else if (shopIds != null && shopIds.Any())
            {
                query = query.Where(p => shopIds.Contains(p.ShopId.Value));
            }

            return await query.ToListAsync();
        }

        public async Task<bool> SlugExistsAsync(string slug)
        {
            return await _context.Products.AnyAsync(p => p.Slug == slug);
        }
    }
}
