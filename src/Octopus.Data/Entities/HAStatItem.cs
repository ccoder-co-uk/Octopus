namespace Octopus.Data.Entities;

public class HAStatItem
{
    public Guid Id { get; set; }
    public long HARowId { get; set; }
    public long MetaId { get; set; }
    public string EntityName { get; set; }
    public string Unit { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public double? State { get; set; }
    public double? Min { get; set; }
    public double? Max { get; set; }
    public double? Mean { get; set; }
    public double? Sum { get; set; }
}