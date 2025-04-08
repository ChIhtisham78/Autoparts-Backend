namespace Autopart.Application.Models
{
	public class RefundReasonDto
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string Slug { get; set; }

		public string Language { get; set; }
		public DateTime? CreatedAt { get; set; }

		public DateTime? UpdatedAt { get; set; }

		public DateTime? DeletedAt { get; set; }
	}
}
