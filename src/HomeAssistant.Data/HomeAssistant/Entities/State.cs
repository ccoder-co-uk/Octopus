namespace HomeAssistant.Data.Entitities;

public partial class State
{
    public int StateId { get; set; }

    public string EntityId { get; set; }

    public string State1 { get; set; }

    public string Attributes { get; set; }

    public int? EventId { get; set; }

    public DateTime? LastChanged { get; set; }

    public double? LastChangedTs { get; set; }

    public DateTime? LastUpdated { get; set; }

    public double? LastUpdatedTs { get; set; }

    public int? OldStateId { get; set; }

    public int? AttributesId { get; set; }

    public string ContextId { get; set; }

    public string ContextUserId { get; set; }

    public string ContextParentId { get; set; }

    public short? OriginIdx { get; set; }

    public byte[] ContextIdBin { get; set; }

    public byte[] ContextUserIdBin { get; set; }

    public byte[] ContextParentIdBin { get; set; }

    public int? MetadataId { get; set; }

    public virtual StateAttribute AttributesNavigation { get; set; }

    public virtual ICollection<State> InverseOldState { get; set; } = new List<State>();

    public virtual StatesMetum Metadata { get; set; }

    public virtual State OldState { get; set; }
}
