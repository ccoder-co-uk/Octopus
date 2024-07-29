using Octopus.Data.ApiResponses;
using Octopus.Data.Entities;

namespace Octopus.Data;

public partial class DataMapper
{
    IEnumerable<Meter> GetMeterInfoForProperty(Property property)
    {
        List<Meter> meters = [];

        foreach (var meterPoint in property.electricity_meter_points)
            foreach (var currentAgreement in meterPoint.agreements)
                foreach (var octopusMeter in meterPoint.meters)
                    meters.Add(BuildMeter(meterPoint, "electricity", currentAgreement, octopusMeter));

        foreach (var meterPoint in property.gas_meter_points)
            foreach (var currentAgreement in meterPoint.agreements)
                foreach (var octopusMeter in meterPoint.meters)
                    meters.Add(BuildMeter(meterPoint, "gas", currentAgreement, octopusMeter));


        return DedupeMeters(meters);
    }

    private static IEnumerable<Meter> DedupeMeters(List<Meter> meters)
    {
        var groups = meters.GroupBy(m => $"{m.MPAN}_{m.SerialNumber}_{m.ProductCode}_{m.TarrifCode}");

        foreach (var meterGroup in groups)
        {
            var parts = meterGroup.Key.Split('_');

            yield return new Meter
            {
                MPAN = parts[0],
                SerialNumber = parts[1],
                ProductCode = parts[2],
                TarrifCode = parts[3], 
                SupplyType = meterGroup.First().SupplyType, 
                IsExport= meterGroup.First().IsExport,
                From = meterGroup.Min(m => m.From),
                To = meterGroup.Any(m => m.To is null) ? null : meterGroup.Max(m => m.To)
            };
        }
    }

    static Meter BuildMeter(MeterPoint meterPoint, string supplyType, Agreement agreement, OctopusMeter octopusMeter) => new()
    {
        MPAN = meterPoint.mpan ?? meterPoint.mprn,
        IsExport = meterPoint.is_export,
        SupplyType = supplyType,
        ProductCode = BuildProductCode(agreement.tariff_code),
        TarrifCode = agreement.tariff_code,
        SerialNumber = octopusMeter.serial_number,
        From = agreement.valid_from, 
        To = agreement.valid_to
    };

    private static string BuildProductCode(string tarrifCode)
    {
        var productCodeWithSuffix = 
            string.Join("-", tarrifCode.Split('-').Skip(2));

        return productCodeWithSuffix[..^2];
    }
}