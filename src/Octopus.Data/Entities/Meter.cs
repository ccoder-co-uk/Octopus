namespace Octopus.Data.Entities;

public class Meter
{
    public Guid Id { get; set; }
    public string MPAN { get; set; }
    public string SerialNumber { get; set; }
    public string ProductCode { get; set; }
    public string TarrifCode { get; set; }
    public string SupplyType { get; set; }

    public bool IsExport { get; set; }

    public DateTimeOffset From { get; set; }
    public DateTimeOffset? To { get; set; }

    public ICollection<MeterRate> Rates { get; set; }
    public ICollection<MeterData> Consumption { get; set; }
    public ICollection<StandingCharge> StandingCharges { get; set; }
}
