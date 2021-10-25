using FlightBot.DatabaseSeeding;
using FlightBot.DatabaseSeeding.Database.Entities;
using FlightBot.DatabaseSeeding.Database.Repositories;
using FlightBot.DatabaseSeeding.Database.Repositories.Abstractions;
using FlightBot.DatabaseSeeding.Services;
using FlightBot.DatabaseSeeding.Services.Abstractions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: WebJobsStartup(typeof(Startup))]

namespace FlightBot.DatabaseSeeding
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.Services.AddHttpClient();
            builder.Services.AddLogging();
            builder.Services.AddDbContext<FlightBotDBContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("SQLConnectionString")));
            builder.Services.AddScoped<IIATACodesRepository, IATACodesRepository>();
            builder.Services.AddScoped<IGeonamesAPIService, GeonamesAPIService>();
        }
    }
}
