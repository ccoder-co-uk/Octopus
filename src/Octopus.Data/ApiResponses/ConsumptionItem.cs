namespace Octopus.Data.ApiResponses;

public class ConsumptionItem
{ 
    public decimal consumption { get; set; }
    public DateTimeOffset interval_start { get; set; }
    public DateTimeOffset interval_end { get; set; }
}
