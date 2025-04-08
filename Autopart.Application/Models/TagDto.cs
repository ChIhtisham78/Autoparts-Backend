namespace Autopart.Application.Models
{
    public class TagDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Language { get; set; }

        public string[] TranslatedLanguages { get; set; }

        public string Slug { get; set; }

        public string Details { get; set; }

        public string Icon { get; set; }

        public int? TypeId { get; set; }

        public int? ImageId { get; set; }

    }
}
