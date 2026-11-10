using System.Globalization;
using DestilatechPlatform.API.Iam.Domain.Model.Aggregates;
using DestilatechPlatform.API.Iam.Domain.Model.ValueObjects;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.Iam.Infrastructure.Persistence.EntityFrameworkCore.Seeding;

public static class UserSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Solo siembra si la tabla está vacía
        if (await context.Set<User>().AnyAsync()) return;

        var producer = User.CreateSeed("Mario Fernández", "productor@destilatech.pe", "Bodega Santa Rosa",
            EBusinessType.Producer, "PROFESSIONAL",
            At("2026-08-22T15:00:00Z"), At("2026-09-05T15:00:00Z"), At("2026-10-19T15:00:00Z"));

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

    private static DateTimeOffset At(string iso)
    {
        return DateTimeOffset.Parse(iso, CultureInfo.InvariantCulture);
    }
}