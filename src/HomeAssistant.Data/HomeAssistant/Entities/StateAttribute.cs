namespace HomeAssistant.Data.Entitities;

public partial class StateAttribute
{
    public int AttributesId { get; set; }

    public long? Hash { get; set; }

    public string SharedAttrs { get; set; }

    public virtual ICollection<State> States { get; set; } = new List<State>();
}
