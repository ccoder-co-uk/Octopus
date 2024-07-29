namespace HomeAssistant.Data.Entitities;

public partial class StatisticsMetum
{
    public int Id { get; set; }

    public string StatisticId { get; set; }

    public string Source { get; set; }

    public string UnitOfMeasurement { get; set; }

    public bool? HasMean { get; set; }

    public bool? HasSum { get; set; }

    public string Name { get; set; }

    public virtual ICollection<Statistic> Statistics { get; set; } = new List<Statistic>();

    public virtual ICollection<StatisticsShortTerm> StatisticsShortTerms { get; set; } = new List<StatisticsShortTerm>();
}
