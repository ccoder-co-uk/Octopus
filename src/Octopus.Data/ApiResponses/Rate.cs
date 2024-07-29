namespace Octopus.Data.ApiResponses;

public class Rate
{
    public decimal value_exc_vat { get; set; }
    public decimal value_inc_vat { get; set; }
    public DateTimeOffset valid_from { get; set; }
    public DateTimeOffset? valid_to { get; set; }
}