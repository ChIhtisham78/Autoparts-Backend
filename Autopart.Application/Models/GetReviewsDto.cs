using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Application.Models
{
    public class GetReviewsDto
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public int UserId { get; set; }
        public int ShopId { get; set; }
        public int ProductId { get; set; }
        public int? VariationOptionId { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public int PositiveFeedbacksCount { get; set; }
        public int NegativeFeedbacksCount { get; set; }
        public string MyFeedback { get; set; }
        public int Reports { get; set; }
        public int AbusiveReportsCount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public RatingProductsDto Product { get; set; }
        public RatingUsersDto User { get; set; }
    }

    public class RatingProductsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public int TypeId { get; set; }
        public decimal Price { get; set; }
        public int ShopId { get; set; }
        public decimal SalePrice { get; set; }
        public string Language { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public string Sku { get; set; }
        public int Quantity { get; set; }
    }

    public class RatingUsersDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime? EmailVerifiedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
