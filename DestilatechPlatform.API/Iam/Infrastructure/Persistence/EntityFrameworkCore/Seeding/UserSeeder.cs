using System.Globalization;
using DestilatechPlatform.API.Iam.Application.Internal.OutboundServices;
using DestilatechPlatform.API.Iam.Domain.Model.Aggregates;
using DestilatechPlatform.API.Iam.Domain.Model.ValueObjects;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.Iam.Infrastructure.Persistence.EntityFrameworkCore.Seeding;

public static class UserSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Solo siembra si la tabla está vacía.
        // Demo accounts: the producer has a paid plan for one year and the retailer an ended trial without a plan,
        // so both states of the subscription can be shown.
        if (await context.Set<User>().AnyAsync()) return;

        var producer = User.CreateSeed("Mario Fernández", "productor@destilatech.pe", "Bodega Santa Rosa",
            EBusinessType.Producer, "PROFESSIONAL",
            At("2026-08-22T15:00:00Z"), At("2026-09-05T15:00:00Z"), DateTimeOffset.UtcNow.AddYears(1));

        var retailer = User.CreateSeed("Carlos Mendoza", "comercializador@destilatech.pe", "Licorería El Cóndor",
            EBusinessType.Retailer, "BASIC",
            At("2026-09-20T15:00:00Z"), At("2026-10-04T04:59:00Z"), null);

        context.Set<User>().AddRange(producer, retailer);
        await context.SaveChangesAsync();

        // El interceptor de auditoría pone CreatedAt = ahora al insertar.
        // Se restauran las fechas del front en un segundo guardado (estado Modified no las pisa).
        producer.CreatedAt = At("2026-08-22T15:00:00Z");
        retailer.CreatedAt = At("2026-09-20T15:00:00Z");
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Gives a password to the demo accounts that have none, so they can sign in like any other account.
    /// </summary>
    /// <remarks>
    /// The password comes from the <c>DemoAccounts__Password</c> environment variable (or the user secrets in local
    /// development), never from the repository. Without it the demo accounts stay without password and cannot sign in.
    /// </remarks>
    public static async Task AssignDemoPasswordsAsync(AppDbContext context, IHashingService hashingService,
        string? demoPassword, ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(demoPassword))
        {
            logger.LogWarning("DemoAccounts:Password is not set: the demo accounts cannot sign in.");
            return;
        }

        var demoAccounts = await context.Set<User>()
            .Where(user => DemoEmails.Contains(user.Email) && user.PasswordHash == string.Empty)
            .ToListAsync();
        if (demoAccounts.Count == 0) return;

        var passwordHash = hashingService.HashPassword(demoPassword);
        foreach (var account in demoAccounts) account.AssignPasswordHash(passwordHash);
        await context.SaveChangesAsync();
        logger.LogInformation("Password assigned to {Count} demo account(s).", demoAccounts.Count);
    }

    private static readonly string[] DemoEmails = ["productor@destilatech.pe", "comercializador@destilatech.pe"];

    private static DateTimeOffset At(string iso)
    {
        return DateTimeOffset.Parse(iso, CultureInfo.InvariantCulture);
    }
}