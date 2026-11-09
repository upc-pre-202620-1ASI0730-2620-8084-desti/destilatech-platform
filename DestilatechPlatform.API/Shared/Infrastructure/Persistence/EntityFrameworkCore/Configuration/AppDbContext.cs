using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;
using DestilatechPlatform.API.Iam.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;//iam
using DestilatechPlatform.API.Billing.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using Microsoft.EntityFrameworkCore;

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
        
        builder.UseSnakeCaseNamingConvention();
    }
}