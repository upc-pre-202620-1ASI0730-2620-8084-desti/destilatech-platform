using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Aggregates;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.ProductionMonitoring.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ProductionModelBuilderExtensions
{
    public static void ApplyProductionMonitoringConfiguration(this ModelBuilder builder)
    {
        var batch = builder.Entity<ProductionBatch>();
        batch.HasIndex(x => new { x.UserId, x.Code }).IsUnique();
        batch.Property(x => x.Code).HasMaxLength(80);
        batch.Property(x => x.Variety).HasMaxLength(80);
        batch.Property(x => x.Tank).HasMaxLength(80);
        batch.Property(x => x.Stage).HasMaxLength(20);
        batch.Property(x => x.Notes).HasMaxLength(255);
        batch.Property(x => x.EstimatedQuantity).HasPrecision(12, 3);
        batch.Property(x => x.Version).IsConcurrencyToken();
        batch.Property(x => x.StartDate)
            .HasConversion(x => x, x => DateTime.SpecifyKind(x, DateTimeKind.Utc));
        batch.HasMany(x => x.StageHistory).WithOne().HasForeignKey("BatchId");
        batch.Navigation(x => x.StageHistory).HasField("_stageHistory")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        var history = builder.Entity<BatchStageChange>();
        history.Property(x => x.Stage).HasMaxLength(20);
        history.Property(x => x.Notes).HasMaxLength(255);
        history.Property(x => x.ChangedAt)
            .HasConversion(x => x, x => DateTime.SpecifyKind(x, DateTimeKind.Utc));

        var variable = builder.Entity<ProcessVariable>();
        variable.HasOne<ProductionBatch>().WithMany().HasForeignKey(x => x.BatchId);
        variable.HasIndex(x => new { x.BatchId, x.Type }).IsUnique();
        variable.HasIndex(x => new { x.BatchId, x.SensorId }).IsUnique();
        variable.HasIndex(x => x.UserId);
        variable.Property(x => x.Type).HasMaxLength(20);
        variable.Property(x => x.SensorId).HasMaxLength(80);
        variable.Property(x => x.MinRange).HasPrecision(9, 4);
        variable.Property(x => x.MaxRange).HasPrecision(9, 4);

        var reading = builder.Entity<SensorReading>();
        reading.HasOne<ProcessVariable>().WithMany().HasForeignKey(x => x.ProcessVariableId);
        reading.HasIndex(x => new { x.UserId, x.RecordedAt });
        reading.HasIndex(x => new { x.ProcessVariableId, x.SourceKey }).IsUnique();
        reading.Property(x => x.SourceKey).HasMaxLength(80);
        reading.Property(x => x.RecordedAt)
            .HasConversion(x => x, x => DateTime.SpecifyKind(x, DateTimeKind.Utc));
        reading.Property(x => x.Value).HasPrecision(9, 4);
    }
}