using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.OrdersReplenishment.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

/// <summary>
/// Entity Framework Core mapping of the Orders &amp; Replenishment bounded context.
/// Tables and columns get snake case names from the shared naming convention:
/// <c>customers</c>, <c>orders</c>, <c>order_lines</c> and <c>replenishment_orders</c>.
/// </summary>
public static class OrdersReplenishmentModelBuilderExtensions
{
    private const int NameLength = 120;
    private const int ProductNameLength = 160;
    private const int PhoneLength = 30;
    private const int AddressLength = 200;
    private const int CodeLength = 20;
    private const int EnumLength = 20;
    private const int NotesLength = 500;

    public static void ApplyOrdersReplenishmentConfiguration(this ModelBuilder builder)
    {
        builder.ApplyCustomerConfiguration();
        builder.ApplyOrderConfiguration();
        builder.ApplyReplenishmentOrderConfiguration();
    }

    private static void ApplyCustomerConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Customer>().HasKey(c => c.Id);
        builder.Entity<Customer>().Property(c => c.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Customer>().Property(c => c.UserId).IsRequired();
        builder.Entity<Customer>().Property(c => c.Name).IsRequired().HasMaxLength(NameLength);
        builder.Entity<Customer>().Property(c => c.ContactName).IsRequired().HasMaxLength(NameLength);
        builder.Entity<Customer>().Property(c => c.Phone).IsRequired().HasMaxLength(PhoneLength);
        builder.Entity<Customer>().Property(c => c.Email).IsRequired().HasMaxLength(NameLength);
        builder.Entity<Customer>().Property(c => c.Address).IsRequired().HasMaxLength(AddressLength);
        builder.Entity<Customer>().Property(c => c.CustomerType)
            .HasConversion<string>()
            .HasMaxLength(EnumLength)
            .IsRequired();
        builder.Entity<Customer>().HasIndex(c => c.UserId);
    }

    private static void ApplyOrderConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Order>().HasKey(o => o.Id);
        builder.Entity<Order>().Property(o => o.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Order>().Property(o => o.UserId).IsRequired();
        builder.Entity<Order>().Property(o => o.Code).IsRequired().HasMaxLength(CodeLength);
        builder.Entity<Order>().Property(o => o.CustomerId).IsRequired();
        builder.Entity<Order>().Property(o => o.CustomerName).IsRequired().HasMaxLength(NameLength);
        builder.Entity<Order>().Property(o => o.OrderDate).IsRequired();
        builder.Entity<Order>().Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(EnumLength)
            .IsRequired();
        builder.Entity<Order>().Property(o => o.Notes).IsRequired().HasMaxLength(NotesLength);
        builder.Entity<Order>().HasIndex(o => new { o.UserId, o.Code }).IsUnique();
        builder.Entity<Order>().HasIndex(o => o.CustomerId);

        // The lines belong to the order: they are always loaded and deleted with it.
        builder.Entity<Order>()
            .HasMany(o => o.Lines)
            .WithOne()
            .HasForeignKey(l => l.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Order>().Navigation(o => o.Lines)
            .HasField("_lines")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .AutoInclude();

        builder.Entity<OrderLine>().HasKey(l => l.Id);
        builder.Entity<OrderLine>().Property(l => l.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<OrderLine>().Property(l => l.ProductId).IsRequired();
        builder.Entity<OrderLine>().Property(l => l.ProductName).IsRequired().HasMaxLength(ProductNameLength);
        builder.Entity<OrderLine>().Property(l => l.Quantity).IsRequired();
        builder.Entity<OrderLine>().Property(l => l.UnitPrice).IsRequired().HasPrecision(12, 2);
    }

    private static void ApplyReplenishmentOrderConfiguration(this ModelBuilder builder)
    {
        builder.Entity<ReplenishmentOrder>().HasKey(r => r.Id);
        builder.Entity<ReplenishmentOrder>().Property(r => r.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<ReplenishmentOrder>().Property(r => r.UserId).IsRequired();
        builder.Entity<ReplenishmentOrder>().Property(r => r.Code).IsRequired().HasMaxLength(CodeLength);
        builder.Entity<ReplenishmentOrder>().Property(r => r.SupplierName).IsRequired().HasMaxLength(NameLength);
        builder.Entity<ReplenishmentOrder>().Property(r => r.SupplierContact).IsRequired().HasMaxLength(NameLength);
        builder.Entity<ReplenishmentOrder>().Property(r => r.ProductId).IsRequired();
        builder.Entity<ReplenishmentOrder>().Property(r => r.ProductName).IsRequired()
            .HasMaxLength(ProductNameLength);
        builder.Entity<ReplenishmentOrder>().Property(r => r.Quantity).IsRequired();
        builder.Entity<ReplenishmentOrder>().Property(r => r.OrderDate).IsRequired();
        builder.Entity<ReplenishmentOrder>().Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(EnumLength)
            .IsRequired();
        builder.Entity<ReplenishmentOrder>().Property(r => r.Notes).IsRequired().HasMaxLength(NotesLength);
        builder.Entity<ReplenishmentOrder>().HasIndex(r => new { r.UserId, r.Code }).IsUnique();
    }
}
