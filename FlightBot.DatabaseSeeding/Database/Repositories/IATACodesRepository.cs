using FlightBot.DatabaseSeeding.Database.Entities;
using FlightBot.DatabaseSeeding.Database.Repositories.Abstractions;
using FlightBot.DatabaseSeeding.DataModels;
using FlightBot.DatabaseSeeding.Services.Abstractions;
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
        readonly IGeonamesAPIService _geonamesAPIService;

        public IATACodesRepository(FlightBotDBContext flightBotDBContext,
            IGeonamesAPIService geonamesAPIService, ILogger<IATACodesRepository> log)
        {
            _flightBotDBContext = flightBotDBContext;
            _geonamesAPIService = geonamesAPIService;
            _log = log;
        }

        public IATACodeEntity[] SearchIATACodes(string airport)
        {
            var searchResults = _flightBotDBContext.IATACode.Where(x =>
                x.Country.Contains(airport) | x.CityAirport.Contains(airport));

            if (searchResults.Count() > 0)
            {
                return searchResults.ToArray();
            }

            var query = Regex.Replace(airport, @"[^0-9a-zA-Z]+", " ").
                ToLower().Split(new char[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);

            return _flightBotDBContext.IATACode.AsEnumerable().Where(x =>
                HasMatchedQuery(query, x.Country, x.CityAirport)).ToArray();
        }

        bool HasMatchedQuery(string[] query, string country, string cityAirport)
        {
            var countrySegments = Regex.Replace(country, @"[^0-9a-zA-Z]+", " ").
                ToLower().Split(new char[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);

            int matches = 0;

            foreach (var q in query)
            {
                foreach (var c in countrySegments)
                {
                    if (c.Contains(q))
                    {
                        matches++;
                        break;
                    }
                }

                if (matches.Equals(query.Length))
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

                if (matches.Equals(query.Length))
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
                iataCode.geonameId = await SearchForGeonameId(iataCode.CityAirport);

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
                    foundIATAEntry.geonameId = iataCode.geonameId;

                    recordsEffected++;
                }
            }

            await _flightBotDBContext.SaveChangesAsync();

            return recordsEffected;
        }

        async Task<string> SearchForGeonameId(string name)
        {
            var result = await _geonamesAPIService.SearchForGeonameId(name);

            if (result.geonames.Length > 0)
            {
                return result.geonames.First().geonameId;
            }

            _log.LogWarning($"Couldn't find geoname id for {name}");

            return string.Empty;
        }
    }
}
