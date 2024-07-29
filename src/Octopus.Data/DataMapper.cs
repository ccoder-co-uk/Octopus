using Newtonsoft.Json;
using Octopus.Data.ApiResponses;
using Octopus.Data.Entities;

namespace Octopus.Data;

public partial class DataMapper(OctopusApiClient octopus)
{
    public async ValueTask<Meter[]> GetMeterDetails(string accountNumber)
    {
        var account = await octopus.GetAccount(accountNumber);

        Console.WriteLine($"Octopus Account details:\n{JsonConvert.SerializeObject(account, Formatting.Indented)}");

        return account.properties
            .SelectMany(GetMeterInfoForProperty)
            .ToArray();
    }

    public async ValueTask<IEnumerable<StandingCharge>> GetStandingCharges(Meter meter, DateTimeOffset from)
    {
        var chargeSet = await octopus.GetStandingCharges(meter.SupplyType, meter.ProductCode, meter.TarrifCode);

        List<StandingCharge> results = [];

        bool done = false;

        while (chargeSet.results?.Length > 0 && !done)
        {
            foreach (var charge in chargeSet.results)
            {
                if (charge.valid_to is null || charge.valid_to > from)
                    results.Add(MapCharge(meter, charge));
                else
                {
                    done = true;
                    break;
                }
            }

            if (chargeSet.next is not null)
                chargeSet = await octopus.Get<OctopusDataResponse<Charge>>(chargeSet.next);
            else
                chargeSet = new OctopusDataResponse<Charge>();
        }

        return results;
    }

    public async ValueTask<IEnumerable<MeterRate>> GetMeterRates(Meter meter, DateTimeOffset from)
    {
        var rateSet = await octopus.GetUnitRates(meter.SupplyType, meter.ProductCode, meter.TarrifCode);
        List<MeterRate> results = [];
        bool done = false;

        while (rateSet.results?.Length > 0 && !done)
        {
            foreach (var rate in rateSet.results)
            {
                if (rate.valid_to is null || rate.valid_to > from)
                    results.Add(MapRate(meter, rate));
                else
                {
                    done = true;
                    break;
                }
            }

            if (rateSet.next is not null)
                rateSet = await octopus.Get<OctopusDataResponse<Rate>>(rateSet.next);
            else
                rateSet = new OctopusDataResponse<Rate>();
        }

        return results;
    }

    public async ValueTask<IEnumerable<MeterData>> GetConsumption(Meter meter, DateTimeOffset from)
    {
        var consumptionData = await octopus.GetConsumption(meter.SupplyType, meter.MPAN, meter.SerialNumber);
        List<MeterData> results = [];

        bool done = false;

        while (consumptionData.results?.Length > 0 && !done)
        {
            foreach (var rate in consumptionData.results)
                results.Add(MapConsumption(meter, rate));

            if (consumptionData.next is not null)
                consumptionData = await octopus.Get<OctopusDataResponse<ConsumptionItem>>(consumptionData.next);
            else
                consumptionData = new OctopusDataResponse<ConsumptionItem>();
        }

        if(results.Count > 0)
            Console.WriteLine($"Fetched Consumption data for Meter {meter.Id}: data range {results.Min(i => i.From):yyyy-MM-dd} - {results.Max(i => i.To):yyyy-MM-dd}.");
        else
            Console.WriteLine($"Fetched Consumption data for Meter {meter.Id}: None found.");

        return results;
    }

    private static MeterData MapConsumption(Meter meter, ConsumptionItem data) => new()
    {
        From = data.interval_start,
        To = data.interval_end,
        MeterId = meter.Id, 
        Value = meter.SupplyType == "gas" 
            ? ((data.consumption * 1.02264m) * 40m) / 3.6m 
            : data.consumption
    };

    private static StandingCharge MapCharge(Meter meter, Charge charge) => new()
    {
        MeterId = meter.Id,
        PaymentMethod = charge.payment_method,
        From = charge.valid_from,
        To = charge.valid_to,
        ExVAT = charge.value_exc_vat,
        IncVAT = charge.value_inc_vat
    };

    private static MeterRate MapRate(Meter meter, Rate rate) => new()
    {
        ExVAT = rate.value_exc_vat,
        IncVAT = rate.value_inc_vat,

        MeterId = meter.Id,
        ProductCode = meter.ProductCode,
        SupplyType = meter.SupplyType,
        TarrifCode = meter.TarrifCode,

        From = rate.valid_from,
        To = rate.valid_to
    };
}