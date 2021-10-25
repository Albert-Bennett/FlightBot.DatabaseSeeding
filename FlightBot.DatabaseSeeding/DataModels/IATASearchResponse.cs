using FlightBot.DatabaseSeeding.Database.Entities;

namespace FlightBot.DatabaseSeeding.DataModels
{
    public class IATASearchResponse
    {
        public IATACodeEntity[] SearchResults { get; set; }
    }
}
