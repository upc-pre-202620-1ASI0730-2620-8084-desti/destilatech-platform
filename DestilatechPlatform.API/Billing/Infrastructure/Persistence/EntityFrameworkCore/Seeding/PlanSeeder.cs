using DestilatechPlatform.API.Billing.Domain.Model.Aggregates;
using DestilatechPlatform.API.Billing.Domain.Model.ValueObjects;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.Billing.Infrastructure.Persistence.EntityFrameworkCore.Seeding;

public static class PlanSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Solo siembra si la tabla está vacía
        if (await context.Set<Plan>().AnyAsync()) return;

        context.Set<Plan>().AddRange(
            new Plan(EPlanCode.Basic, 60, EBillingCycle.Monthly, 1, false,
                ["billing.features.inventory", "billing.features.stock-alerts"]),
            new Plan(EPlanCode.Professional, 110, EBillingCycle.Monthly, 5, true,
            [
                "billing.features.iot-monitoring", "billing.features.batch-traceability",
                "billing.features.estimations"
            ]),
            new Plan(EPlanCode.Enterprise, 200, EBillingCycle.Monthly, null, false,
                ["billing.features.everything-pro", "billing.features.priority-support"]));

        await context.SaveChangesAsync();
    }
}