using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FlightBot.DatabaseSeeding.Database.Repositories.Abstractions;

namespace FlightBot.DatabaseSeeding
{
    public class SeedDB
    {
        readonly IIATACodesRepository _iataCodesRepository;

        public SeedDB(IIATACodesRepository iataCodesRepository)
        {
            _iataCodesRepository = iataCodesRepository;
        }

        [FunctionName("SeedDB")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<IATACodeRequest>(requestBody);

            await _iataCodesRepository.UpdateIATACodes(data);

            return new OkObjectResult("");
        }
    }
}
