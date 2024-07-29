namespace HomeAssistant.Data.Entitities;

public partial class RecorderRun
{
    public int RunId { get; set; }

    public DateTime Start { get; set; }

    public DateTime? End { get; set; }

    public bool ClosedIncorrect { get; set; }

    public DateTime Created { get; set; }
}
