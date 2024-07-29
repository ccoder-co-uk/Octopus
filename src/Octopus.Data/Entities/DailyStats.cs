using System.ComponentModel.DataAnnotations;

namespace Octopus.Data.Entities;
public class DailyStats
{
    [Key]
    public int Id { get; set; }

    public string MeterPointReference { get; set; }

    public Guid MeterId { get; set; }
    public DateTime Date { get; set; }
    public decimal OctopusUnits { get; set; }
    public decimal HAUnitValue { get; set; }
    public decimal OctopusStandingCharge { get; set; }
    public decimal OctopusUnitCost { get; set; }
    public decimal OctopusTotalCost { get; set; }
    public decimal HAUnitCost { get; set; }
    public decimal HATotalCost { get; set; }

    public Meter Meter { get; set; }
}