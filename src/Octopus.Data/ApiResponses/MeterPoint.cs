namespace Octopus.Data.ApiResponses;

public class MeterPoint
{ 
    public int consumption_standard { get; set; }
    public int profile_class { get; set; }
    public string mprn { get; set; }
    public string mpan { get; set; }
    public bool is_export { get; set; }

    public ICollection<Agreement> agreements { get; set; }
    public ICollection<OctopusMeter> meters { get; set; }
}
