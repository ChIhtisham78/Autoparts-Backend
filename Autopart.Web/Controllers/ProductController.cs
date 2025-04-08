using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Application.Models.Products;
using Autopart.Domain.CommonDTO;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Autopart.Api.Controllers
{
    [Route("api/Product")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _context;
        public ProductsController(IProductService context)
        {
            _context = context;
        }

        [HttpGet("mywishlist/{userId}/{productId}")]
        public async Task<IActionResult> CheckProductInWishlistAsync(int userId, int productId)
        {
            if (userId <= 0 || productId <= 0)
            {
                return BadRequest("Invalid user ID or product ID.");
            }

            var isInWishlist = await _context.IsProductInWishlistAsync(userId, productId);

            if (isInWishlist)
            {
                return Ok(true);
            }

            return Ok(new List<object>()); // Return an empty array `[]` with 200 status
        }





        [HttpGet("my-wishlist/{userId}")]
        public async Task<IActionResult> GetWishlistByUserIdAsync(int userId)
        {
            var products = await _context.GetWishlistProductsAsync(userId);

            if (products == null || !products.Any())
            {
                return Ok(new List<ProductDtoResponse>());
            }

            return Ok(products);
        }


        [HttpGet("mywishlist/{productId}")]
        public async Task<IActionResult> GetWishlistByProductIdAsync(int productId)
        {
            if (productId <= 0)
            {
                return BadRequest("Invalid user ID.");
            }

            var products = await _context.GetWishlistProductsByProductIdAsync(productId);

            if (products == null || !products.Any())
            {
                return Ok(new List<ProductDtoResponse>());
            }

            return Ok(products);
        }



        [HttpDelete("wishlist/{productId}")]
        public async Task<IActionResult> DeleteWishlistByProductIdAsync(int productId)
        {
            if (productId <= 0)
            {
                return BadRequest("Invalid product ID.");
            }

            var deletionResult = await _context.DeleteWishlistProductsByProductIdAsync(productId);

            if (!deletionResult)
            {
                return NotFound("No wishlist entries found for the given product ID.");
            }

            return Ok($"Product with ID {productId} deleted successfully.");
        }




        [HttpPost("mywishlist")]
        public async Task<IActionResult> AddProductToWishlistAsync([FromBody] WishlistRequest request)
        {
            if (request == null || request.ProductId <= 0 || request.UserId <= 0)
            {
                return BadRequest("Invalid product or user information.");
            }

            try
            {
                await _context.AddProductToWishlistAsync(request.UserId, request.ProductId);
                return Ok("Product added to wishlist.");
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }




        [HttpGet("products")]
        public async Task<ActionResult> GetProducts([FromQuery] GetProductsDto getProductsDto)
        {
            var (products, totalCount) = await _context.GetProductsAsync(getProductsDto);

            var response = new
            {
                result = products,
                total = totalCount,
                currentPage = getProductsDto.pageNumber,
                count = products.Count(),
                lastPage = (int)Math.Ceiling((double)totalCount / getProductsDto.pageSize),
                firstItem = (getProductsDto.pageNumber - 1) * getProductsDto.pageSize + 1,
                lastItem = (getProductsDto.pageNumber - 1) * getProductsDto.pageSize + products.Count(),
                perPage = getProductsDto.pageSize,
            };

            return new JsonResult(response)
            {
                StatusCode = (int)HttpStatusCode.OK
            };
        }




        [HttpGet("lowStockProducts")]
        public async Task<ActionResult<IEnumerable<ProductDtoResponse>>> GetLowStockProducts()
        {
            var lowStockThreshold = 10;
            var products = await _context.GetLowStockProductsAsync(lowStockThreshold);

            if (products == null || !products.Any())
            {
                return Ok(new List<ProductDtoResponse>());
            }

            return Ok(products);
        }



        [HttpGet("product-by-shopId/{shopId}")]
        public async Task<ActionResult<IEnumerable<ProductDtoResponse>>> GetProductsByShopId(int shopId)
        {
            var products = await _context.GetProductsByShopIdAsync(shopId);

            if (products == null || !products.Any())
            {
                return Ok(new List<ProductDtoResponse>());
            }

            return Ok(products);
        }






        [Authorize]
        [HttpPost("product")]
        public async Task<ActionResult<Domain.Models.Product>> CreateProduct([FromForm] ProductRequestDto requestDto)
        {

            var product = await _context.CreateProductAsync(requestDto);
            return CreatedAtAction(
                nameof(GetProductBySlug),
                new { slug = product.Slug },
                product
            );
        }

        //[Authorize]
        [HttpPut("product/{id}")]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductRequestDto requestDto, int id)
        {
            requestDto.productDto.Id = id;
            await _context.UpdateProductAsync(requestDto);
            return Ok();
        }
        [Authorize]
        [HttpDelete("product/{id}")]
        public async Task<IActionResult> RemoveProduct(int id)
        {
            await _context.RemoveProductAsync(id);
            return Ok();
        }

        [HttpGet("product/{slug}")]
        public async Task<ActionResult<ProductDtoResponse>> GetProductBySlug(string slug)
        {
            var productDto = await _context.GetProductBySlugAsync(slug);
            return Ok(productDto);
        }



        [HttpGet("products/{id}")]
        public async Task<ActionResult<ProductDtoResponse>> GetProductById(int id)
        {
            var product = await _context.GetProductByIdAsync(id);
            if (product == null) return NotFound();

            return Ok(product);
        }

        //[Authorize]
        [HttpPost("upload/image")]
        public async Task<ActionResult> UploadImage(IFormFile file)
        {
            var image = await _context.UploadImage(file);
            if (image == null)
            {
                return NotFound();
            }

            return Ok(image);
        }


        [HttpGet("products/popularproducts")]
        public async Task<ActionResult<IEnumerable<ProductDtoResponse>>> GetPopularProducts(int? shopId = null, int? vendorId = null)
        {
            var products = await _context.GetPopularProductsAsync(shopId, vendorId);
            return Ok(products);
        }


        [HttpGet("products/getbestsellingproducts")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetBestSellingProducts(int? shopId = null, int? vendorId = null)
        {
            var products = await _context.GetBestSellingProductsAsync(shopId, vendorId);
            return Ok(products);
        }

        [HttpGet("products/gettopratedproducts")]
        public async Task<ActionResult<IEnumerable<TopRatedProducts>>> GetTopRatedProducts(int? shopId = null, int? vendorId = null)
        {
            var products = await _context.GetTopRatedProductsAsync(shopId, vendorId);
            return Ok(products);
        }

        [HttpGet("products/getdraftproducts")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetDraftProducts()
        {
            var products = await _context.GetDraftProductsAsync();
            return Ok(products);
        }

        [HttpGet("products/getproductsstock")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsStock()
        {
            var products = await _context.GetProductsStockAsync();
            return Ok(products);
        }

        [HttpGet("products/ProductsCount")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsCountCategoryWise()
        {
            var products = await _context.GetProductsSummaryAsync();
            return Ok(products);
        }

        //[Authorize]
        [HttpPost("upload-product")]

        public async Task<IActionResult> UploadFileAsync(IFormFile file, int shopId)
        {
            var result = await _context.UploadFileAsync(file, shopId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("export/{shopId}")]
        public async Task<IActionResult> ExportProducts(int shopId)
        {
            try
            {
                var stream = await _context.ExportProducts(shopId);
                string excelName = $"Products_{shopId}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("category-wise-product-count")]
        public async Task<IActionResult> ProductCount(GetProductsDto getProductsDto)
        {
            var result = await _context.GetShopsWithCategoryProductCountsAsync(getProductsDto);
            return Ok(result);

        }

    }
}
