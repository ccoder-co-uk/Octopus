using Microsoft.EntityFrameworkCore;

namespace Octopus.Data;

public partial class OctopusSync
{
    async ValueTask ImportMeters(OctopusDbContext db)
    {
        Console.WriteLine("Importing Meter Information ..");

        var octopusConfig = config.GetSection("Octopus");
        var octopus = new OctopusApiClient(octopusConfig["apiKey"], octopusConfig["apiUrl"]);
        var dataMapper = new DataMapper(octopus);
        var octopusMeters = await dataMapper.GetMeterDetails(octopusConfig["accountNumber"]);

        var dbMeters = await db.Meters.ToArrayAsync();

        foreach (var meter in octopusMeters)
        {
            if (Array.Find(dbMeters, m => m.Id == meter.Id) is null)
            {
                await db.AddAsync(meter);
                await db.SaveChangesAsync();
            }
        }

        Console.WriteLine("Importing Meter Information Complete!");
    }

    async ValueTask ImportRates(OctopusDbContext db)
    {
        Console.WriteLine("Importing Rate Information ..");

        var octopusConfig = config.GetSection("Octopus");
        var octopus = new OctopusApiClient(octopusConfig["apiKey"], octopusConfig["apiUrl"]);
        var dataMapper = new DataMapper(octopus);

        var dbMeters = await db.Meters.ToArrayAsync();

        foreach (var meter in dbMeters)
        {
            DateTimeOffset from = (await db.MeterRates
                .Where(r => r.MeterId == meter.Id)
                .OrderByDescending(r => r.To).FirstOrDefaultAsync())?
                .To
                    ??
                DateTimeOffset.UtcNow.AddYears(-1);

            await db.MeterRates
                .Where(r => r.MeterId == meter.Id && r.To == null)
                .ExecuteDeleteAsync();

            var rates = await dataMapper.GetMeterRates(meter, from);

            await db.AddRangeAsync(rates);
            await db.SaveChangesAsync();
        }

        Console.WriteLine("Importing Rate Information Complete!");
    }

    async ValueTask ImportStandingCharges(OctopusDbContext db)
    {
        Console.WriteLine("Importing Standing Charges ..");

        var octopusConfig = config.GetSection("Octopus");
        var octopus = new OctopusApiClient(octopusConfig["apiKey"], octopusConfig["apiUrl"]);
        var dataMapper = new DataMapper(octopus);

        var dbMeters = await db.Meters.ToArrayAsync();

        foreach (var meter in dbMeters)
        {
            DateTimeOffset from = (await db.StandingCharges
                .Where(r => r.MeterId == meter.Id)
                .OrderByDescending(r => r.To).FirstOrDefaultAsync())?
                .To
                    ??
                DateTimeOffset.UtcNow.AddYears(-1);

            await db.StandingCharges
                .Where(r => r.MeterId == meter.Id && r.To == null)
                .ExecuteDeleteAsync();

            await db.SaveChangesAsync();

            var charges = await dataMapper.GetStandingCharges(meter, from);

            await db.AddRangeAsync(charges);
            await db.SaveChangesAsync();
        }

        Console.WriteLine("Importing Standing Charges Complete!");
    }

    async ValueTask ImportConsumption(OctopusDbContext db)
    {
        Console.WriteLine("Importing Consumption Information ..");

        var octopusConfig = config.GetSection("Octopus");
        var octopus = new OctopusApiClient(octopusConfig["apiKey"], octopusConfig["apiUrl"]);
        var dataMapper = new DataMapper(octopus);

        var dbMeters = await db.Meters.ToArrayAsync();

        foreach (var meter in dbMeters)
        {
            DateTimeOffset from = (await db.Consumption
                .Where(r => r.MeterId == meter.Id)
                .OrderByDescending(r => r.To).FirstOrDefaultAsync())?
                .To
                    ??
                DateTimeOffset.UtcNow.AddYears(-5);

            Console.WriteLine($"Importing Consumption data for meter {meter.Id}");
            var consumptionData = await dataMapper.GetConsumption(meter, from);

            await db.Consumption
                .Where(c => c.MeterId == meter.Id)
                .ExecuteDeleteAsync();

            await db.AddRangeAsync(consumptionData);
            await db.SaveChangesAsync();
        }

        Console.WriteLine("Importing Consumption Information Complete!");
    }
}