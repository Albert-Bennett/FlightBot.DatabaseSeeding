using FlightBot.DatabaseSeeding.Database.Repositories.Abstractions;
using FlightBot.DatabaseSeeding.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace FlightBot.DatabaseSeeding
{
    public class SearchIATACodes
    {
        readonly IIATACodesRepository _iataCodesRepository;

        public SearchIATACodes(IIATACodesRepository iataCodesRepository)
        {
            _iataCodesRepository = iataCodesRepository;
        }

        [FunctionName("SearchIATACodes")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            string query = req.Query["airport"];

            var searchResults = new IATASearchResponse 
            {
                SearchResults = _iataCodesRepository.SearchIATACodes(query)
            };

            return new JsonResult(searchResults);
        }
    }
}
