using System.Threading.Tasks;

namespace FlightBot.DatabaseSeeding.Database.Repositories.Abstractions
{
    public interface IIATACodesRepository
    {
        Task<bool> UpdateIATACodes(IATACodeRequest iataCodes);
    }
}
