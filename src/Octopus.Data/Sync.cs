using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Octopus.Data;

public partial class OctopusSync(IConfiguration config)
{
    public async ValueTask MigrateDb()
    {
        var octopusDbConnection = config.GetConnectionString("Energy");
        using OctopusDbContext db = new(octopusDbConnection);
        await db.Database.EnsureCreatedAsync();
        await db.Database.MigrateAsync();
    }

    public async ValueTask Account()
    {
        Console.WriteLine("Synching Account Information ...");

        var octopusDbConnection = config.GetConnectionString("Energy");
        using OctopusDbContext db = new(octopusDbConnection);

        await ImportMeters(db);
        await ImportStandingCharges(db);
        await ImportRates(db);
        await ImportConsumption(db);

        Console.WriteLine("Synching Account Information Complete!");
    }

    public async ValueTask ComputeDailyStats(DateTime from)
    {
        Console.WriteLine("Computing Daily Stats ...");

        var octopusDbConnection = config.GetConnectionString("Energy");
        using OctopusDbContext db = new(octopusDbConnection);
        await db.ComputeDailyStats(from);

        Console.WriteLine("Computing Daily Stats Complete!");
    }
}