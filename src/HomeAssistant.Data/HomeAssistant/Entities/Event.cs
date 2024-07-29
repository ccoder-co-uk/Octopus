namespace HomeAssistant.Data.Entitities;

public partial class Event
{
    public int EventId { get; set; }

    public string EventType { get; set; }

    public string EventData { get; set; }

    public string Origin { get; set; }

    public short? OriginIdx { get; set; }

    public DateTime? TimeFired { get; set; }

    public double? TimeFiredTs { get; set; }

    public string ContextId { get; set; }

    public string ContextUserId { get; set; }

    public string ContextParentId { get; set; }

    public int? DataId { get; set; }

    public byte[] ContextIdBin { get; set; }

    public byte[] ContextUserIdBin { get; set; }

    public byte[] ContextParentIdBin { get; set; }

    public int? EventTypeId { get; set; }

    public virtual EventDatum Data { get; set; }

    public virtual EventType EventTypeNavigation { get; set; }
}
