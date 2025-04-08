namespace Autopart.Application.Models
{
    public class AttributeDto
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public int ShopId { get; set; }
        public string Language { get; set; }
        public int UserId { get; set; }

        public string Slug { get; set; }

        public IEnumerable<ValueDto> Values { get; set; } // Include the collection of values
    }

    public class ValueDto
    {
        public int Id { get; set; }

        public string Value1 { get; set; }

        public int? AttributeId { get; set; }

        public string Slug { get; set; }

        public string Meta { get; set; }

        public string Language { get; set; }
    }
}
