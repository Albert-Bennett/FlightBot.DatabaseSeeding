using FlightBot.DatabaseSeeding.Database.Entities;

namespace FlightBot.DatabaseSeeding
{
    public class IATACodeRequest
    {
        public IATACodes[] IATACodes { get; set; }
    }

    public class IATACodes
    {
        public string Airport { get; set; }
        public string Country { get; set; }
        public string IATACode { get; set; }

        public IATACodeEntity ToIATACodeEntity()
        {
            return new IATACodeEntity
            {
                CityAirport = Airport,
                Country = Country,
                IATACode = IATACode
            };
        }
    }
}
