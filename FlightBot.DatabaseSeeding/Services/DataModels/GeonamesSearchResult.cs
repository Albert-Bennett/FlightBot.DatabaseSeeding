namespace FlightBot.DatabaseSeeding.Services.DataModels
{
    public class GeonamesSearchResult
    {
        public Geoname[] geonames { get; set; }
    }

    public class Geoname
    {
        public string geonameId { get; set; }
    }
}
