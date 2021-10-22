using FlightBot.DatabaseSeeding.Database.Entities;
using FlightBot.DatabaseSeeding.Database.Repositories.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
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

        public async Task<bool> UpdateIATACodes(IATACodeRequest iataCodes)
        {

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
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    foundIATAEntry.IATACode = iataCode.IATACode;
                    foundIATAEntry.CityAirport = iataCode.CityAirport;
                    foundIATAEntry.Country = iataCode.Country;
                }
            }

            await _flightBotDBContext.SaveChangesAsync();

            return true;
        }
    }
}
