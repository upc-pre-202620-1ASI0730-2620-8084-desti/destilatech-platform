using DestilatechPlatform.API.AlertsNotifications.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using DestilatechPlatform.API.AnalyticsEstimations.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using DestilatechPlatform.API.OrdersReplenishment.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;
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
        
        builder.ApplyAlertsNotificationsConfiguration();
        builder.ApplyOrdersReplenishmentConfiguration();
        builder.ApplyAnalyticsEstimationsConfiguration();

        builder.UseSnakeCaseNamingConvention();
        
        builder.ApplyInventoryStockConfiguration();
        
        builder.UseSnakeCaseNamingConvention();
        
        builder.ApplyInventoryStockConfiguration();
        
        builder.ApplyProductionMonitoringConfiguration();
        
        builder.UseSnakeCaseNamingConvention();
    }
}