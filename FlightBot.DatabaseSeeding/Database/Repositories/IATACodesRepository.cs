using FlightBot.DatabaseSeeding.Database.Entities;
using FlightBot.DatabaseSeeding.Database.Repositories.Abstractions;
using FlightBot.DatabaseSeeding.DataModels;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlightBot.DatabaseSeeding.Database.Repositories
{
    public class IATACodesRepository : IIATACodesRepository
    {
        readonly FlightBotDBContext _flightBotDBContext;
        readonly ILogger<IATACodesRepository> _log;

        public IATACodesRepository(FlightBotDBContext flightBotDBContext, ILogger<IATACodesRepository> log)
        {
            _flightBotDBContext = flightBotDBContext;
            _log = log;
        }

        public IATACodeEntity[] SearchIATACodes(string airport, string geonameId)
        {
            var query = Regex.Replace(airport, @"[^0-9a-zA-Z]+", " ").
                ToLower().Split(new char[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);

            var searchResults = _flightBotDBContext.IATACode.Where(x =>
                x.geonameId.Equals(geonameId) && HasMatchedQuery(query, x.Country, x.CityAirport,
                (int)Math.Max(1, (float)query.Length / 2)));

            if (searchResults.Count() == 0)
            {
                searchResults = _flightBotDBContext.IATACode.Where(x =>
                    x.Country.Contains(airport) | x.CityAirport.Contains(airport));
            }

            if (searchResults.Count() > 0)
            {
                return searchResults.ToArray();
            }

            return _flightBotDBContext.IATACode.AsEnumerable().Where(x => 
                HasMatchedQuery(query, x.Country, x.CityAirport, query.Length)).ToArray();
        }

        bool HasMatchedQuery(string[]query, string country, string cityAirport, int maxMatches) 
        {
            var countrySegments = Regex.Replace(country, @"[^0-9a-zA-Z]+", " ").
                ToLower().Split(new char[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);

            int matches = 0;

            foreach (var q in query) 
            {
                foreach(var c in countrySegments)
                {
                    if (c.Contains(q))
                    {
                        matches++;
                        break;
                    }
                }

                if (matches.Equals(maxMatches))
                {
                    return true;
                }
            }

            var cityAirportSegments = Regex.Replace(cityAirport, @"[^0-9a-zA-Z]+", " ").
                ToLower().Split(new char[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);

            matches = 0;

            foreach (var q in query)
            {
                foreach (var c in cityAirportSegments)
                {
                    if (c.Contains(q))
                    {
                        matches++;
                        break;
                    }
                }

                if (matches.Equals(maxMatches))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<int> UpdateIATACodes(IATACodeRequest iataCodes)
        {
            int recordsEffected = 0;

            foreach (var i in iataCodes.IATACodes)
            {
                var iataCode = i.ToIATACodeEntity();

                var foundIATAEntry = _flightBotDBContext.IATACode.Where(x => x.IATACode.Equals(iataCode.IATACode)).FirstOrDefault();

                if (foundIATAEntry == null)
                {
                    _log.LogInformation($"Adding IATA Code {i.IATACode}, for {i.Airport}.");

                    try
                    {
                        await _flightBotDBContext.AddAsync(iataCode);
                        recordsEffected++;
                    }
                    catch (Exception)
                    {
                        _log.LogError($"Unable to process IATA Code {i.IATACode}");
                    }
                }
                else
                {
                    foundIATAEntry.IATACode = iataCode.IATACode;
                    foundIATAEntry.CityAirport = iataCode.CityAirport;
                    foundIATAEntry.Country = iataCode.Country;

                    recordsEffected++;
                }
            }

            await _flightBotDBContext.SaveChangesAsync();

            return recordsEffected;
        }
    }
}
