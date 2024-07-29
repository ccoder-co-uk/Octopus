namespace HomeAssistant.Data.Entitities;

public partial class StatesMetum
{
    public int MetadataId { get; set; }

    public string EntityId { get; set; }

    public virtual ICollection<State> States { get; set; } = new List<State>();
}
