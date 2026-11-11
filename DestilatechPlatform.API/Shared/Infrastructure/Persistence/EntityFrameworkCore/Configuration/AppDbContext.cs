using DestilatechPlatform.API.AlertsNotifications.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using DestilatechPlatform.API.AnalyticsEstimations.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using DestilatechPlatform.API.OrdersReplenishment.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;
using DestilatechPlatform.API.Iam.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;//iam
using DestilatechPlatform.API.Billing.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using Microsoft.EntityFrameworkCore;
using DestilatechPlatform.API.InventoryStock.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using DestilatechPlatform.API.ProductionMonitoring.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

namespace DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.AddInterceptors(new AuditableEntityInterceptor());
        base.OnConfiguring(builder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // IAM Context
        builder.ApplyIamConfiguration();

        // Billing Context
        builder.ApplyBillingConfiguration();

        // Alerts & Notifications Context
        builder.ApplyAlertsNotificationsConfiguration();

        // Inventory & Stock Context
        builder.ApplyInventoryStockConfiguration();

        // Production & Monitoring Context
        builder.ApplyProductionMonitoringConfiguration();

        // Orders & Replenishment Context
        builder.ApplyOrdersReplenishmentConfiguration();

        // Analytics & Estimations Context
        builder.ApplyAnalyticsEstimationsConfiguration();

        builder.UseSnakeCaseNamingConvention();
    }
}