using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Application.Models
{
    public class GetQuestionDto
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int ShopId { get; set; }
        public int ProductId { get; set; } = 0;
        public string Question { get; set; }
        public string Answer { get; set; }
        public int? PositiveFeedbacksCount { get; set; }

        public int? NegativeFeedbacksCount { get; set; }
        public int? AbusiveReportsCount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string MyFeedback { get; set; }

        public RatingProductsDto Product { get; set; }
        public RatingUsersDto User { get; set; }
    }
}
