using FlightBot.DatabaseSeeding.Database.Repositories.Abstractions;
using FlightBot.DatabaseSeeding.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace FlightBot.DatabaseSeeding
{
    public class AddIATACodes
    {
        readonly IIATACodesRepository _iataCodesRepository;

        public AddIATACodes(IIATACodesRepository iataCodesRepository)
        {
            _iataCodesRepository = iataCodesRepository;
        }

        [FunctionName("AddIATACodes")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<IATACodeRequest>(requestBody);

            var recordsEffected = await _iataCodesRepository.UpdateIATACodes(data);

            return new OkObjectResult($"Updated {recordsEffected} records");
        }
    }
}
