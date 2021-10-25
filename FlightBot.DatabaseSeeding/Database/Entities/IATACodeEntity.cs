using System.ComponentModel.DataAnnotations;

namespace FlightBot.DatabaseSeeding.Database.Entities
{
    public class IATACodeEntity
    {
        [Key]
        public string IATACode { get; set; }
        public string CityAirport { get; set; }
        public string Country { get; set; }
        public string geonameId { get; set; }
    }
}
