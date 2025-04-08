using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Application.Models
{
    public class QuestionDto
    {
        public int? UserId { get; set; }
        public int? ShopId { get; set; }
        public int ProductId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public int? PositiveFeedbacksCount { get; set; }

        public int? NegativeFeedbacksCount { get; set; }
    }
}
