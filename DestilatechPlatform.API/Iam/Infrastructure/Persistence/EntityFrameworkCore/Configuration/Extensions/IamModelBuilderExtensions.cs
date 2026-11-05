using DestilatechPlatform.API.Iam.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.Iam.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class IamModelBuilderExtensions
{
    public static void ApplyIamConfiguration(this ModelBuilder builder)
    {
        builder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).ValueGeneratedOnAdd();
            entity.Property(u => u.Name).IsRequired().HasMaxLength(120);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(180);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.BusinessName).IsRequired().HasMaxLength(150);
            entity.Property(u => u.UserType).HasConversion<string>().HasMaxLength(20).IsRequired();
            entity.Property(u => u.PreferredPlan).HasMaxLength(50);
            entity.Property(u => u.TrialStartedAt).IsRequired();
            entity.Property(u => u.TrialEndsAt).IsRequired();
        });
    }
}