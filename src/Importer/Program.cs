using HomeAssistant.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Octopus.Data;

// Get configuration
IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var octopusSync = new OctopusSync(config);

var jobs = config
    .GetValue<string>("jobs")
    .Split(',');

if (jobs.Contains("Setup"))
    await octopusSync.MigrateDb();

if (jobs.Contains("Octopus"))
    await octopusSync.Account();

if (jobs.Contains("HomeAssistant"))
{
    var haSync = new HASync(config);
    await haSync.ImportBackups();
}

if (jobs.Contains("Compute"))
{
    var aYearAgo = DateTime.Now.AddYears(-1).Date;
    await octopusSync.ComputeDailyStats(from: aYearAgo);
}

if (jobs.Contains("Close"))
    return;

Console.WriteLine("All Configured Workloads Complete, press any key to close.");

/*
var octopusConfig = config.GetSection("Octopus");
var octopus = new OctopusApiClient(octopusConfig["apiKey"], octopusConfig["apiUrl"]);

var data = await octopus.GetConsumption("electricity", "2700002352726", "17K0197892");

var gasData = await octopus.GetConsumption("gas", "7715188307", "G4W00336952227");

foreach(var item in data.results)
    Console.WriteLine(JsonConvert.SerializeObject(item));
*/

Console.ReadKey();