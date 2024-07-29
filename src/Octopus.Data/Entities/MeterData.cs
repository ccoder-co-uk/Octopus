namespace Octopus.Data.Entities;

public class MeterData
{
    public int Id { get; set; }
    public Guid MeterId { get; set; }
    public decimal Value { get; set; }
    public DateTimeOffset From { get; set; }
    public DateTimeOffset To { get; set; }

    public Meter Meter { get; set; }
}