namespace HomeAssistant.Data.Entitities;

public partial class EventType
{
    public int EventTypeId { get; set; }

    public string EventType1 { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
