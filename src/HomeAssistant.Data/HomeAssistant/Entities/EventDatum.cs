namespace HomeAssistant.Data.Entitities;

public partial class EventDatum
{
    public int DataId { get; set; }

    public long? Hash { get; set; }

    public string SharedData { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
