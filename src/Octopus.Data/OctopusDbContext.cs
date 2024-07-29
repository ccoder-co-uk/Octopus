using Microsoft.EntityFrameworkCore;
using Octopus.Data.Entities;

namespace Octopus.Data;

public partial class OctopusDbContext(string connection) : DbContext
{
    public DbSet<Meter> Meters { get; set; }
    public DbSet<StandingCharge> StandingCharges { get; set; }
    public DbSet<MeterRate> MeterRates { get; set; }
    public DbSet<MeterData> Consumption { get; set; }
    public DbSet<DailyStats> DailyStats { get; set; }
    public DbSet<HAStatItem> HAStatItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlServer(connection);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var schema = "Octopus";

        modelBuilder.Entity<Meter>(entity =>
        {
            entity.ToTable("Meters", schema);
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("varchar(50)");
            entity.Property(e => e.MPAN).HasColumnType("varchar(50)");
            entity.Property(e => e.ProductCode).HasColumnType("varchar(50)");
            entity.Property(e => e.SerialNumber).HasColumnType("varchar(50)");
            entity.Property(e => e.TarrifCode).HasColumnType("varchar(50)");
            entity.Property(e => e.From).HasColumnType("datetimeoffset");
            entity.Property(e => e.To).HasColumnType("datetimeoffset");
        });

        modelBuilder.Entity<MeterData>(entity =>
        {
            entity.ToTable("MeterData", schema);
            entity.HasKey(e => e.Id);

            entity.Property(e => e.From).HasColumnType("datetimeoffset");
            entity.Property(e => e.To).HasColumnType("datetimeoffset");
            entity.Property(e => e.MeterId).HasColumnType("varchar(50)");
            entity.Property(e => e.Value).HasColumnType("decimal(14,6)");

            entity.HasOne(e => e.Meter).WithMany(e => e.Consumption);
        });

        modelBuilder.Entity<MeterRate>(entity =>
        {
            entity.ToTable("MeterRates", schema);
            entity.HasKey(e => e.Id);

            entity.Property(e => e.From).HasColumnType("datetimeoffset");
            entity.Property(e => e.To).HasColumnType("datetimeoffset");
            entity.Property(e => e.MeterId).HasColumnType("varchar(50)");
            entity.Property(e => e.SupplyType).HasColumnType("varchar(50)");
            entity.Property(e => e.TarrifCode).HasColumnType("varchar(50)");
            entity.Property(e => e.ProductCode).HasColumnType("varchar(50)");

            entity.HasOne(e => e.Meter).WithMany(e => e.Rates);
        });

        modelBuilder.Entity<StandingCharge>(entity =>
        {
            entity.ToTable("StandingCharges", schema);
            entity.HasKey(e => e.Id);

            entity.Property(e => e.From).HasColumnType("datetimeoffset");
            entity.Property(e => e.To).HasColumnType("datetimeoffset");
            entity.Property(e => e.MeterId).HasColumnType("varchar(50)");
            entity.Property(e => e.PaymentMethod).HasColumnType("varchar(50)");

            entity.HasOne(e => e.Meter).WithMany(e => e.StandingCharges);
        });

        modelBuilder.Entity<DailyStats>(entity => {
            entity.ToTable("DailyStats", schema);
            entity.HasKey(e => e.Id);

            entity.Property(d => d.MeterPointReference).HasColumnType("nvarchar(101)");
            entity.Property(d => d.OctopusUnits).HasColumnType("decimal(14,6)");
            entity.Property(d => d.HAUnitValue).HasColumnType("decimal(14,6)");
            entity.Property(d => d.OctopusStandingCharge).HasColumnType("decimal(14,6)");
            entity.Property(d => d.OctopusUnitCost).HasColumnType("decimal(14,6)");
            entity.Property(d => d.OctopusTotalCost).HasColumnType("decimal(14,6)");
            entity.Property(d => d.HAUnitCost).HasColumnType("decimal(14,6)");
            entity.Property(d => d.HATotalCost).HasColumnType("decimal(14,6)");
        });

        modelBuilder.Entity<HAStatItem>(entity => {
            entity.ToTable("HAStats", "HomeAssistant");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.HARowId).HasColumnType("bigint");
            entity.Property(e => e.MetaId).HasColumnType("bigint");
            entity.Property(e => e.EntityName).HasColumnType("varchar(200)");
            entity.Property(e => e.Unit).HasColumnType("varchar(20)");
            entity.Property(e => e.Timestamp).HasColumnType("datetimeoffset");

            entity.Property(d => d.State).HasColumnType("decimal(14,6)");
            entity.Property(d => d.Min).HasColumnType("decimal(14,6)");
            entity.Property(d => d.Max).HasColumnType("decimal(14,6)");
            entity.Property(d => d.Mean).HasColumnType("decimal(14,6)");
            entity.Property(d => d.Sum).HasColumnType("decimal(14,6)");
        });
    }

    public async ValueTask DropDuplicateHAData() =>
        await Database.ExecuteSqlAsync(@$"
            WITH RankedStats AS (
                SELECT 
                    [Id],
                    ROW_NUMBER() OVER (PARTITION BY [HARowId] ORDER BY [Timestamp] DESC) AS rn
                FROM 
                    [Energy].[HomeAssistant].[HAStats]
            )
            DELETE FROM [Energy].[HomeAssistant].[HAStats]
            WHERE [Id] NOT IN (
                SELECT [Id]
                FROM RankedStats
                WHERE rn = 1
            );
            WITH RankedStats AS (
                    SELECT 
                    [Id],
                    ROW_NUMBER() OVER (PARTITION BY [HARowId] ORDER BY [Timestamp] DESC) AS rn
                FROM 
                    [Energy].[HomeAssistant].[HAStats]
            )
            DELETE FROM [Energy].[HomeAssistant].[HAStats]
            WHERE [Id] NOT IN (
                SELECT [Id]
                FROM RankedStats
                WHERE rn = 1
            );");
}