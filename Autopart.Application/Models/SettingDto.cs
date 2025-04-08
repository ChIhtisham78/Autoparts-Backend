namespace Autopart.Application.Models.Dto
{
    public class SettingDto
    {       // Address-related fields
        public string Contact { get; set; }
        public string Website { get; set; }
        public decimal? LocationLat { get; set; }
        public decimal? LocationLng { get; set; }
        public string LocationZip { get; set; }
        public string LocationCity { get; set; }
        public string LocationState { get; set; }
        public string LocationCountry { get; set; }
        public string LocationFormattedAddress { get; set; }
    }
}
