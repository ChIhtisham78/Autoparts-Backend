namespace Autopart.Application.Models
{
    public class RatingDto
    {
        public int Rating1 { get; set; }

        public int? Total { get; set; }

        public int? PositiveFeedbacksCount { get; set; }

        public int? NegativeFeedbacksCount { get; set; }
        public int? AbusiveReportsCount { get; set; }
    }
}
