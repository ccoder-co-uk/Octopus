namespace Octopus.Data.ApiResponses;

public class OctopusMeter
{ 
    public string serial_number { get; set; }
    public ICollection<Register> registers { get; set; }
}
