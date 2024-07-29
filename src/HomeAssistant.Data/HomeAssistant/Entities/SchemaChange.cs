namespace HomeAssistant.Data.Entitities;

public partial class SchemaChange
{
    public int ChangeId { get; set; }

    public int? SchemaVersion { get; set; }

    public DateTime Changed { get; set; }
}
