using FlightBot.DatabaseSeeding.Services.Abstractions;
using FlightBot.DatabaseSeeding.Services.DataModels;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlightBot.DatabaseSeeding.Services
{
    public class GeonamesAPIService : BaseAPIService, IGeonamesAPIService
    {
        readonly string geonamesUsername;

        public GeonamesAPIService(IConfiguration configuration, IHttpClientFactory httpClientFactory):
            base(httpClientFactory, configuration["geonames_base_url"])
        {
            geonamesUsername = configuration["geonames_username"];
        }

        public async Task<GeonamesSearchResult> SearchForGeonameId(string airport) 
        {
            string endpoint = $"searchJSON?maxRows=10&q={airport}&username={geonamesUsername}&fcode=AIRP";

            return await GetAsync<GeonamesSearchResult>(endpoint);
        }
    }
}
