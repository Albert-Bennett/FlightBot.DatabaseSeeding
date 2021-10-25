using FlightBot.DatabaseSeeding.Services.DataModels;
using System.Threading.Tasks;

namespace FlightBot.DatabaseSeeding.Services.Abstractions
{
    public interface IGeonamesAPIService
    {
        Task<GeonamesSearchResult> SearchForGeonameId(string airport);
    }
}
