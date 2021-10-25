using FlightBot.DatabaseSeeding.Database.Entities;
using FlightBot.DatabaseSeeding.DataModels;
using System.Threading.Tasks;

namespace FlightBot.DatabaseSeeding.Database.Repositories.Abstractions
{
    public interface IIATACodesRepository
    {
        Task<int> UpdateIATACodes(IATACodeRequest iataCodes);
        IATACodeEntity[] SearchIATACodes(string airport, string geonameId);
    }
}
