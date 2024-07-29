using HomeAssistant.Data.Tools;
using Microsoft.Extensions.Configuration;
using Octopus.Data;
using Octopus.Data.Entities;

namespace HomeAssistant.Data;

public class HASync(IConfiguration config)
{
    public async ValueTask ImportBackups()
    {
        var haConfig = config.GetSection("HomeAssistant");
        var homeAssistantBackups = haConfig["homeAssistantBackups"];
        var backupRootFolder = haConfig["workingDir"];

        Downloader.Download(homeAssistantBackups, backupRootFolder);

        foreach (var backupPackage in Directory.GetFiles(backupRootFolder))
        {
            Console.WriteLine($"Unpacking backup {backupPackage}");

            var dbFilePath = await Unpacker.Unpack(backupPackage);

            if (dbFilePath is not null)
            {
                int sensorId = 1;
                var stats = GetStatsForSensor(sensorId, dbFilePath);

                while (stats.Length != 0)
                {
                    Console.WriteLine($"Copying stats for sensor {stats[0].EntityName} to SQL Server");
                    await Dump(stats);

                    sensorId++;
                    stats = GetStatsForSensor(sensorId, dbFilePath);
                }
            }

            Console.WriteLine($"Processing backup {backupPackage} Complete.");
        }

        await DropDuplicates();

        Console.WriteLine($"Done.");
    }

    static HAStatItem[] GetStatsForSensor(int sensorId, string fromDbFile)
    {
        using var db = new HomeAssistantDBContext(fromDbFile);

        return db.Statistics
            .Select(s => new
            {
                s.Id,
                s.MetadataId,
                s.Metadata,
                Created = s.CreatedTs != null ? (DateTimeOffset?)DateTimeOffset.FromUnixTimeSeconds((long)s.CreatedTs) : null,
                Timestamp = s.StartTs != null ? (DateTimeOffset?)DateTimeOffset.FromUnixTimeSeconds((long)s.StartTs) : null,
                LastReset = s.LastResetTs != null ? (DateTimeOffset?)DateTimeOffset.FromUnixTimeSeconds((long)s.LastResetTs) : null,
                s.State,
                s.Min,
                s.Max,
                s.Mean,
                s.Sum
            })
            .Where(s => s.MetadataId == sensorId)
        .AsEnumerable()
            .Select(s => new HAStatItem
            {
                HARowId = s.Id,
                MetaId = s.MetadataId.Value,
                EntityName = s.Metadata.StatisticId.Replace("sensor.", ""),
                Unit = s.Metadata.UnitOfMeasurement,
                Timestamp = s.Timestamp.Value,
                State = s.State,
                Min = s.Min,
                Max = s.Max,
                Mean = s.Mean,
                Sum = s.Sum
            }).ToArray();
    }

    async Task Dump(HAStatItem[] stats)
    {
        var dbConnectionString = config.GetConnectionString("Energy");
        using OctopusDbContext db = new(dbConnectionString);
        await db.AddRangeAsync(stats);
        await db.SaveChangesAsync();
    }

    async Task DropDuplicates()
    {
        var dbConnectionString = config.GetConnectionString("Energy");
        using OctopusDbContext db = new(dbConnectionString);
        await db.DropDuplicateHAData();
    }
}
