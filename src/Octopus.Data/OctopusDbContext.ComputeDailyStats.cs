using Microsoft.EntityFrameworkCore;
using Octopus.Data.Entities;

namespace Octopus.Data;

public partial class OctopusDbContext : DbContext
{
    public async ValueTask ComputeDailyStats(DateTime from)
    {
        var exportMeters = Meters.Where(m => m.SupplyType == "electricity" && m.IsExport);
        var importMeters = Meters.Where(m => m.SupplyType == "electricity" && !m.IsExport);
        var gasMeters = Meters.Where(m => m.SupplyType == "gas" && !m.IsExport);

        var feedinSensor = "feedin_daily";
        var gridSensor = "grid_daily";
        var gasSensor = "";

        Console.WriteLine("  Computing Electricity Export Dailies ...");
        foreach (var meter in exportMeters)
            await AddRangeAsync(await GetDailyStats(meter, from, feedinSensor));

        Console.WriteLine("  Computing Electricity Import Dailies ...");
        foreach (var meter in importMeters)
            await AddRangeAsync(await GetDailyStats(meter, from, gridSensor));

        Console.WriteLine("  Computing Gas Import Dailies ...");
        foreach (var meter in gasMeters)
            await AddRangeAsync(await GetDailyStats(meter, from, gasSensor));


        await ApplyChanges(from);
    }

    async ValueTask ApplyChanges(DateTime from)
    {
        Console.WriteLine("  Dropping old data ...");
        await DailyStats
            .Where(i => i.Date >= from)
            .ExecuteDeleteAsync();

        Console.WriteLine("  Adding new data ...");
        await SaveChangesAsync();
    }

    private async ValueTask<IEnumerable<DailyStats>> GetDailyStats(Meter meter, DateTime startDate, string sensorName)
    {
        List<DailyStats> dailyStats = [];

        var currentDate = startDate;
        var endDate = DateTime.Now.Date;

        while (currentDate <= endDate)
        {
            if (meter.From <= startDate && (meter.To is null || meter.To >= currentDate))
            {

                var standingCharge = await StandingCharges
                    .Where(sc => sc.MeterId == meter.Id && sc.From <= currentDate && (sc.To == null || sc.To >= currentDate))
                    .Select(sc => sc.IncVAT / 100)
                    .FirstOrDefaultAsync();

                var dailyData = await GetDailyMeterData(meter.Id, currentDate, sensorName);

                var dailyStat = new DailyStats
                {
                    MeterPointReference = $"{meter.MPAN}_{meter.SerialNumber}",
                    MeterId = meter.Id,
                    Date = currentDate,
                    OctopusUnits = dailyData.DailyOctopusUnitsUsed,
                    HAUnitValue = dailyData.DailyHAUnitsUsed,
                    OctopusStandingCharge = standingCharge,
                    OctopusUnitCost = dailyData.DailyOctopusCost,
                    OctopusTotalCost = dailyData.DailyOctopusCost + standingCharge,
                    HAUnitCost = dailyData.DailyHACost,
                    HATotalCost = dailyData.DailyHACost + standingCharge
                };

                dailyStats.Add(dailyStat);
            }

            currentDate = currentDate.AddDays(1);
        }

        return dailyStats;
    }

    private async Task<DailyMeterData> GetDailyMeterData(Guid meterId, DateTime date, string sensorName)
    {
        var hourlyData = await GetHourlyMeterData(meterId, date, sensorName);

        var dailyMeterData = new DailyMeterData
        {
            DailyHAUnitsUsed = hourlyData.Sum(hd => hd.HAUsage),
            DailyOctopusUnitsUsed = hourlyData.Sum(hd => hd.OctopusUsage),
            DailyHACost = hourlyData.Sum(hd => hd.HACost) / 100,
            DailyOctopusCost = hourlyData.Sum(hd => hd.OctopusCost) / 100
        };

        return dailyMeterData;
    }

    private async Task<List<HourlyMeterData>> GetHourlyMeterData(Guid meterId, DateTime date, string sensorName)
    {
        var hourlyValues = Enumerable.Range(0, 24)
            .Select(hour => date.AddHours(hour))
            .ToList();

        var haStats = await HAStatItems
            .Where(ha => ha.EntityName == sensorName && ha.Timestamp >= date && ha.Timestamp < date.AddDays(1))
            .OrderBy(ha => ha.Timestamp)
            .ToListAsync();

        var meterData = await Consumption
            .Where(md => md.MeterId == meterId && md.From >= date && md.To < date.AddDays(1))
            .ToListAsync();

        var meterRates = await MeterRates
            .Where(mr => mr.MeterId == meterId)
            .ToListAsync();

        var hourlyMeterData = new List<HourlyMeterData>();

        foreach (var hour in hourlyValues)
        {
            var previousUsage = hourlyMeterData.Any() 
                ? hourlyMeterData[^1].HAUsage 
                : 0;

            var haUsage = haStats
                .Where(hs => hs.Timestamp >= hour && hs.Timestamp < hour.AddHours(1))
                .Sum(hs => Math.Max((decimal)hs.State - previousUsage, 0));

            var octopusUsage = meterData
                .Where(md => md.From >= hour && md.From < hour.AddHours(1))
                .Sum(md => md.Value);  // Replace 'Value' with the actual column you want to sum

            var meterRate = meterRates
                .FirstOrDefault(mr => mr.From <= hour && mr.To > hour);

            var hourlyDatum = new HourlyMeterData
            {
                MeterId = meterId,
                HourValue = hour,
                HAUsage = haUsage,
                OctopusUsage = octopusUsage,
                IncVAT = meterRate?.IncVAT ?? 0,
                ExVAT = meterRate?.ExVAT ?? 0,
                HACost = haUsage * (meterRate?.IncVAT ?? 0),
                OctopusCost = octopusUsage * (meterRate?.IncVAT ?? 0)
            };

            hourlyMeterData.Add(hourlyDatum);
        }

        return hourlyMeterData;
    }

    public class DailyMeterData
    {
        public decimal DailyHAUnitsUsed { get; set; }
        public decimal DailyOctopusUnitsUsed { get; set; }
        public decimal DailyHACost { get; set; }
        public decimal DailyOctopusCost { get; set; }
    }

    public class HourlyMeterData
    {
        public Guid MeterId { get; set; }
        public DateTime HourValue { get; set; }
        public decimal HAUsage { get; set; }
        public decimal OctopusUsage { get; set; }
        public decimal IncVAT { get; set; }
        public decimal ExVAT { get; set; }
        public decimal HACost { get; set; }
        public decimal OctopusCost { get; set; }
    }
}