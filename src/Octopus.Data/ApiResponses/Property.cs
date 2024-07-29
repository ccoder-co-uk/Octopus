namespace Octopus.Data.ApiResponses;

public class Property
{ 
    public int id { get; set; }

    public string address_line_1 { get; set; }
    public string address_line_2 { get; set; }
    public string address_line_3 { get; set; }
    public string town { get; set; }
    public string county { get; set; }
    public string postcode { get; set; }
    
    public DateTimeOffset moved_in_at { get; set; }
    public DateTimeOffset? moved_out_at { get; set; }

    public ICollection<MeterPoint> electricity_meter_points { get; set; }
    public ICollection<MeterPoint> gas_meter_points { get; set; }
}
