namespace Octopus.Data.ApiResponses;

public class Agreement
{ 
    public string tariff_code { get; set; }
    public DateTimeOffset valid_from { get; set; }
    public DateTimeOffset? valid_to { get; set; }
}
