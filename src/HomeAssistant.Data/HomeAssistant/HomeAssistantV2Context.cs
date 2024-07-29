using HomeAssistant.Data.Entitities;
using Microsoft.EntityFrameworkCore;

namespace HomeAssistant.Data;

public partial class HomeAssistantDBContext(string filePath) : DbContext
{
    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventDatum> EventData { get; set; }

    public virtual DbSet<EventType> EventTypes { get; set; }

    public virtual DbSet<RecorderRun> RecorderRuns { get; set; }

    public virtual DbSet<SchemaChange> SchemaChanges { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<StateAttribute> StateAttributes { get; set; }

    public virtual DbSet<StatesMetum> StatesMeta { get; set; }

    public virtual DbSet<Statistic> Statistics { get; set; }

    public virtual DbSet<StatisticsMetum> StatisticsMeta { get; set; }

    public virtual DbSet<StatisticsRun> StatisticsRuns { get; set; }

    public virtual DbSet<StatisticsShortTerm> StatisticsShortTerms { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite($"data source={filePath};Cache=Shared");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("events");

            entity.HasIndex(e => e.ContextId, "ix_events_context_id");

            entity.HasIndex(e => e.ContextIdBin, "ix_events_context_id_bin");

            entity.HasIndex(e => e.DataId, "ix_events_data_id");

            entity.HasIndex(e => new { e.EventTypeId, e.TimeFiredTs }, "ix_events_event_type_id_time_fired_ts");

            entity.HasIndex(e => e.TimeFiredTs, "ix_events_time_fired_ts");

            entity.Property(e => e.EventId)
                .ValueGeneratedNever()
                .HasColumnName("event_id");
            entity.Property(e => e.ContextId)
                .HasColumnType("VARCHAR(36)")
                .HasColumnName("context_id");
            entity.Property(e => e.ContextIdBin).HasColumnName("context_id_bin");
            entity.Property(e => e.ContextParentId)
                .HasColumnType("VARCHAR(36)")
                .HasColumnName("context_parent_id");
            entity.Property(e => e.ContextParentIdBin).HasColumnName("context_parent_id_bin");
            entity.Property(e => e.ContextUserId)
                .HasColumnType("VARCHAR(36)")
                .HasColumnName("context_user_id");
            entity.Property(e => e.ContextUserIdBin).HasColumnName("context_user_id_bin");
            entity.Property(e => e.DataId).HasColumnName("data_id");
            entity.Property(e => e.EventData).HasColumnName("event_data");
            entity.Property(e => e.EventType)
                .HasColumnType("VARCHAR(64)")
                .HasColumnName("event_type");
            entity.Property(e => e.EventTypeId).HasColumnName("event_type_id");
            entity.Property(e => e.Origin)
                .HasColumnType("VARCHAR(32)")
                .HasColumnName("origin");
            entity.Property(e => e.OriginIdx)
                .HasColumnType("SMALLINT")
                .HasColumnName("origin_idx");
            entity.Property(e => e.TimeFired)
                .HasColumnType("DATETIME")
                .HasColumnName("time_fired");
            entity.Property(e => e.TimeFiredTs)
                .HasColumnType("FLOAT")
                .HasColumnName("time_fired_ts");

            entity.HasOne(d => d.Data).WithMany(p => p.Events).HasForeignKey(d => d.DataId);

            entity.HasOne(d => d.EventTypeNavigation).WithMany(p => p.Events).HasForeignKey(d => d.EventTypeId);
        });

        modelBuilder.Entity<EventDatum>(entity =>
        {
            entity.HasKey(e => e.DataId);

            entity.ToTable("event_data");

            entity.HasIndex(e => e.Hash, "ix_event_data_hash");

            entity.Property(e => e.DataId)
                .ValueGeneratedNever()
                .HasColumnName("data_id");
            entity.Property(e => e.Hash)
                .HasColumnType("BIGINT")
                .HasColumnName("hash");
            entity.Property(e => e.SharedData).HasColumnName("shared_data");
        });

        modelBuilder.Entity<EventType>(entity =>
        {
            entity.ToTable("event_types");

            entity.HasIndex(e => e.EventType1, "ix_event_types_event_type").IsUnique();

            entity.Property(e => e.EventTypeId)
                .ValueGeneratedNever()
                .HasColumnName("event_type_id");
            entity.Property(e => e.EventType1)
                .HasColumnType("VARCHAR(64)")
                .HasColumnName("event_type");
        });

        modelBuilder.Entity<RecorderRun>(entity =>
        {
            entity.HasKey(e => e.RunId);

            entity.ToTable("recorder_runs");

            entity.HasIndex(e => new { e.Start, e.End }, "ix_recorder_runs_start_end");

            entity.Property(e => e.RunId)
                .ValueGeneratedNever()
                .HasColumnName("run_id");
            entity.Property(e => e.ClosedIncorrect)
                .HasColumnType("BOOLEAN")
                .HasColumnName("closed_incorrect");
            entity.Property(e => e.Created)
                .HasColumnType("DATETIME")
                .HasColumnName("created");
            entity.Property(e => e.End)
                .HasColumnType("DATETIME")
                .HasColumnName("end");
            entity.Property(e => e.Start)
                .HasColumnType("DATETIME")
                .HasColumnName("start");
        });

        modelBuilder.Entity<SchemaChange>(entity =>
        {
            entity.HasKey(e => e.ChangeId);

            entity.ToTable("schema_changes");

            entity.Property(e => e.ChangeId)
                .ValueGeneratedNever()
                .HasColumnName("change_id");
            entity.Property(e => e.Changed)
                .HasColumnType("DATETIME")
                .HasColumnName("changed");
            entity.Property(e => e.SchemaVersion).HasColumnName("schema_version");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.ToTable("states");

            entity.HasIndex(e => e.AttributesId, "ix_states_attributes_id");

            entity.HasIndex(e => e.ContextId, "ix_states_context_id");

            entity.HasIndex(e => e.ContextIdBin, "ix_states_context_id_bin");

            entity.HasIndex(e => e.LastUpdatedTs, "ix_states_last_updated_ts");

            entity.HasIndex(e => new { e.MetadataId, e.LastUpdatedTs }, "ix_states_metadata_id_last_updated_ts");

            entity.HasIndex(e => e.OldStateId, "ix_states_old_state_id");

            entity.Property(e => e.StateId)
                .ValueGeneratedNever()
                .HasColumnName("state_id");
            entity.Property(e => e.Attributes).HasColumnName("attributes");
            entity.Property(e => e.AttributesId).HasColumnName("attributes_id");
            entity.Property(e => e.ContextId)
                .HasColumnType("VARCHAR(36)")
                .HasColumnName("context_id");
            entity.Property(e => e.ContextIdBin).HasColumnName("context_id_bin");
            entity.Property(e => e.ContextParentId)
                .HasColumnType("VARCHAR(36)")
                .HasColumnName("context_parent_id");
            entity.Property(e => e.ContextParentIdBin).HasColumnName("context_parent_id_bin");
            entity.Property(e => e.ContextUserId)
                .HasColumnType("VARCHAR(36)")
                .HasColumnName("context_user_id");
            entity.Property(e => e.ContextUserIdBin).HasColumnName("context_user_id_bin");
            entity.Property(e => e.EntityId)
                .HasColumnType("VARCHAR(255)")
                .HasColumnName("entity_id");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.LastChanged)
                .HasColumnType("DATETIME")
                .HasColumnName("last_changed");
            entity.Property(e => e.LastChangedTs)
                .HasColumnType("FLOAT")
                .HasColumnName("last_changed_ts");
            entity.Property(e => e.LastUpdated)
                .HasColumnType("DATETIME")
                .HasColumnName("last_updated");
            entity.Property(e => e.LastUpdatedTs)
                .HasColumnType("FLOAT")
                .HasColumnName("last_updated_ts");
            entity.Property(e => e.MetadataId).HasColumnName("metadata_id");
            entity.Property(e => e.OldStateId).HasColumnName("old_state_id");
            entity.Property(e => e.OriginIdx)
                .HasColumnType("SMALLINT")
                .HasColumnName("origin_idx");
            entity.Property(e => e.State1)
                .HasColumnType("VARCHAR(255)")
                .HasColumnName("state");

            entity.HasOne(d => d.AttributesNavigation).WithMany(p => p.States).HasForeignKey(d => d.AttributesId);

            entity.HasOne(d => d.Metadata).WithMany(p => p.States).HasForeignKey(d => d.MetadataId);

            entity.HasOne(d => d.OldState).WithMany(p => p.InverseOldState).HasForeignKey(d => d.OldStateId);
        });

        modelBuilder.Entity<StateAttribute>(entity =>
        {
            entity.HasKey(e => e.AttributesId);

            entity.ToTable("state_attributes");

            entity.HasIndex(e => e.Hash, "ix_state_attributes_hash");

            entity.Property(e => e.AttributesId)
                .ValueGeneratedNever()
                .HasColumnName("attributes_id");
            entity.Property(e => e.Hash)
                .HasColumnType("BIGINT")
                .HasColumnName("hash");
            entity.Property(e => e.SharedAttrs).HasColumnName("shared_attrs");
        });

        modelBuilder.Entity<StatesMetum>(entity =>
        {
            entity.HasKey(e => e.MetadataId);

            entity.ToTable("states_meta");

            entity.HasIndex(e => e.EntityId, "ix_states_meta_entity_id").IsUnique();

            entity.Property(e => e.MetadataId)
                .ValueGeneratedNever()
                .HasColumnName("metadata_id");
            entity.Property(e => e.EntityId)
                .HasColumnType("VARCHAR(255)")
                .HasColumnName("entity_id");
        });

        modelBuilder.Entity<Statistic>(entity =>
        {
            entity.ToTable("statistics");

            entity.HasIndex(e => e.Start, "ix_statistics_start");

            entity.HasIndex(e => e.StartTs, "ix_statistics_start_ts");

            entity.HasIndex(e => new { e.MetadataId, e.StartTs }, "ix_statistics_statistic_id_start_ts").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Created)
                .HasColumnType("DATETIME")
                .HasColumnName("created");
            entity.Property(e => e.CreatedTs)
                .HasColumnType("FLOAT")
                .HasColumnName("created_ts");
            entity.Property(e => e.LastReset)
                .HasColumnType("DATETIME")
                .HasColumnName("last_reset");
            entity.Property(e => e.LastResetTs)
                .HasColumnType("FLOAT")
                .HasColumnName("last_reset_ts");
            entity.Property(e => e.Max)
                .HasColumnType("FLOAT")
                .HasColumnName("max");
            entity.Property(e => e.Mean)
                .HasColumnType("FLOAT")
                .HasColumnName("mean");
            entity.Property(e => e.MetadataId).HasColumnName("metadata_id");
            entity.Property(e => e.Min)
                .HasColumnType("FLOAT")
                .HasColumnName("min");
            entity.Property(e => e.Start)
                .HasColumnType("DATETIME")
                .HasColumnName("start");
            entity.Property(e => e.StartTs)
                .HasColumnType("FLOAT")
                .HasColumnName("start_ts");
            entity.Property(e => e.State)
                .HasColumnType("FLOAT")
                .HasColumnName("state");
            entity.Property(e => e.Sum)
                .HasColumnType("FLOAT")
                .HasColumnName("sum");

            entity.HasOne(d => d.Metadata).WithMany(p => p.Statistics)
                .HasForeignKey(d => d.MetadataId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StatisticsMetum>(entity =>
        {
            entity.ToTable("statistics_meta");

            entity.HasIndex(e => e.StatisticId, "ix_statistics_meta_statistic_id").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.HasMean)
                .HasColumnType("BOOLEAN")
                .HasColumnName("has_mean");
            entity.Property(e => e.HasSum)
                .HasColumnType("BOOLEAN")
                .HasColumnName("has_sum");
            entity.Property(e => e.Name)
                .HasColumnType("VARCHAR(255)")
                .HasColumnName("name");
            entity.Property(e => e.Source)
                .HasColumnType("VARCHAR(32)")
                .HasColumnName("source");
            entity.Property(e => e.StatisticId)
                .HasColumnType("VARCHAR(255)")
                .HasColumnName("statistic_id");
            entity.Property(e => e.UnitOfMeasurement)
                .HasColumnType("VARCHAR(255)")
                .HasColumnName("unit_of_measurement");
        });

        modelBuilder.Entity<StatisticsRun>(entity =>
        {
            entity.HasKey(e => e.RunId);

            entity.ToTable("statistics_runs");

            entity.HasIndex(e => e.Start, "ix_statistics_runs_start");

            entity.Property(e => e.RunId)
                .ValueGeneratedNever()
                .HasColumnName("run_id");
            entity.Property(e => e.Start)
                .HasColumnType("DATETIME")
                .HasColumnName("start");
        });

        modelBuilder.Entity<StatisticsShortTerm>(entity =>
        {
            entity.ToTable("statistics_short_term");

            entity.HasIndex(e => e.Start, "ix_statistics_short_term_start");

            entity.HasIndex(e => e.StartTs, "ix_statistics_short_term_start_ts");

            entity.HasIndex(e => new { e.MetadataId, e.StartTs }, "ix_statistics_short_term_statistic_id_start_ts").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Created)
                .HasColumnType("DATETIME")
                .HasColumnName("created");
            entity.Property(e => e.CreatedTs)
                .HasColumnType("FLOAT")
                .HasColumnName("created_ts");
            entity.Property(e => e.LastReset)
                .HasColumnType("DATETIME")
                .HasColumnName("last_reset");
            entity.Property(e => e.LastResetTs)
                .HasColumnType("FLOAT")
                .HasColumnName("last_reset_ts");
            entity.Property(e => e.Max)
                .HasColumnType("FLOAT")
                .HasColumnName("max");
            entity.Property(e => e.Mean)
                .HasColumnType("FLOAT")
                .HasColumnName("mean");
            entity.Property(e => e.MetadataId).HasColumnName("metadata_id");
            entity.Property(e => e.Min)
                .HasColumnType("FLOAT")
                .HasColumnName("min");
            entity.Property(e => e.Start)
                .HasColumnType("DATETIME")
                .HasColumnName("start");
            entity.Property(e => e.StartTs)
                .HasColumnType("FLOAT")
                .HasColumnName("start_ts");
            entity.Property(e => e.State)
                .HasColumnType("FLOAT")
                .HasColumnName("state");
            entity.Property(e => e.Sum)
                .HasColumnType("FLOAT")
                .HasColumnName("sum");

            entity.HasOne(d => d.Metadata).WithMany(p => p.StatisticsShortTerms)
                .HasForeignKey(d => d.MetadataId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
