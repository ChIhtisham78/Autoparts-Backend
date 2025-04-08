namespace Autopart.Application.Models
{
	public class CouponDto
	{
		public int Id { get; set; }

		public string Code { get; set; }

		public string Language { get; set; }

		public decimal? Amount { get; set; }

		public decimal? MinimumCartAmount { get; set; }

		public bool IsActive { get; set; }

		public int? ShopId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
