namespace Octopus.Data.Entities;

public class MeterRate
{
    public int Id { get; set; }
    public Guid MeterId { get; set; }
    public string SupplyType { get; set; }
    public string ProductCode { get; set; }
    public string TarrifCode { get; set; }
    public decimal ExVAT { get; set; }
    public decimal IncVAT { get;set;}
    public DateTimeOffset From { get; set; }
    public DateTimeOffset? To { get; set; }

    public Meter Meter { get; set; }
}