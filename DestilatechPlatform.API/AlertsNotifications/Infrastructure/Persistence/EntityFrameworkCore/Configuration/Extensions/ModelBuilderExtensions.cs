using DestilatechPlatform.API.AlertsNotifications.Domain.Model.Aggregates;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.AlertsNotifications.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyAlertsNotificationsConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Alert>().HasKey(a => a.Id);
        builder.Entity<Alert>().Property(a => a.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Alert>().Property(a => a.AccountId)
            .HasConversion(valueObject => valueObject.Value, value => new AccountId(value))
            .IsRequired();
        builder.Entity<Alert>().Property(a => a.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
        builder.Entity<Alert>().Property(a => a.SourceId)
            .HasConversion(valueObject => valueObject.Value, value => new SourceId(value))
            .IsRequired();
        builder.Entity<Alert>().Property(a => a.Message)
            .HasConversion(valueObject => valueObject.Value, value => new AlertMessage(value))
            .HasMaxLength(255)
            .IsRequired();
        builder.Entity<Alert>().Property(a => a.Severity)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
        builder.Entity<Alert>().Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
        builder.Entity<Alert>().Property(a => a.EpisodeKey).HasMaxLength(120);
        builder.Entity<Alert>().HasIndex(a => a.EpisodeKey).IsUnique();
        builder.Entity<Alert>().HasIndex(a => a.AccountId);
    }
}
