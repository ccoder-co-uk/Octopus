namespace Octopus.Data.Entities;

public class StandingCharge
{ 
    public int Id { get; set; }
    public Guid MeterId { get; set; }

    public string PaymentMethod { get; set; }

    public DateTimeOffset From { get; set; }
    public DateTimeOffset? To { get; set; }
    public decimal ExVAT { get; set; }
    public decimal IncVAT { get; set; }

    public Meter Meter { get; set; }
}