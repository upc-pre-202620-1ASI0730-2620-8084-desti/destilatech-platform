using DestilatechPlatform.API.Billing.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.Billing.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class BillingModelBuilderExtensions
{
    public static void ApplyBillingConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Plan>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).ValueGeneratedOnAdd();
            entity.Property(p => p.Code).HasConversion<string>().HasMaxLength(20).IsRequired();
            entity.HasIndex(p => p.Code).IsUnique();
            entity.Property(p => p.Price).HasPrecision(10, 2).IsRequired();
            entity.Property(p => p.BillingCycle).HasConversion<string>().HasMaxLength(20).IsRequired();
            entity.Property(p => p.FeatureKeysCsv).HasMaxLength(500).IsRequired();
            entity.Ignore(p => p.FeatureKeys);
        });

        builder.Entity<Subscription>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Id).ValueGeneratedOnAdd();
            entity.Property(s => s.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
            entity.Property(s => s.PaymentReference).HasMaxLength(255).IsRequired();
            entity.HasIndex(s => s.PaymentReference).IsUnique();
            entity.HasIndex(s => s.UserId);
            // UserId es referencia lógica a IAM: no hay foreign key hacia users
            entity.HasOne<Plan>().WithMany().HasForeignKey(s => s.PlanId);
        });

        builder.Entity<Payment>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).ValueGeneratedOnAdd();
            entity.Property(p => p.Amount).HasPrecision(10, 2).IsRequired();
            entity.Property(p => p.Currency).HasMaxLength(3).IsRequired();
            entity.Property(p => p.Provider).HasConversion<string>().HasMaxLength(20).IsRequired();
            entity.Property(p => p.Reference).HasMaxLength(255).IsRequired();
            entity.HasIndex(p => p.Reference).IsUnique();
            entity.Property(p => p.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
            entity.Property(p => p.CardBrand).HasMaxLength(30);
            entity.Property(p => p.CardLast4).HasMaxLength(4);
            entity.HasIndex(p => p.UserId);
            entity.HasOne<Plan>().WithMany().HasForeignKey(p => p.PlanId);
        });
    }
}