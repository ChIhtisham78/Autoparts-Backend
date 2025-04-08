namespace Autopart.Application.Models
{
    public class TaxDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal? Rate { get; set; }

        //public bool? IsGlobal { get; set; }

        //public string Country { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string City { get; set; }

        public string Priority { get; set; }

        public bool? OnShipping { get; set; }
        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
