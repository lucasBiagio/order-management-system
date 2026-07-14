using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        OrderManagementDbContext context,
        CancellationToken cancellationToken = default)
    {
        await context.Database.MigrateAsync(cancellationToken);

        if (!await context.Customers.AnyAsync(cancellationToken))
        {
            var customers = new[]
            {
                new Customer(
                    "Lucas Martins",
                    "lucas.seed@email.com"),

                new Customer(
                    "Maria Oliveira",
                    "maria.seed@email.com"),

                new Customer(
                    "João Santos",
                    "joao.seed@email.com")
            };

            await context.Customers.AddRangeAsync(
                customers,
                cancellationToken);
        }

        if (!await context.Products.AnyAsync(cancellationToken))
        {
            var products = new[]
            {
                new Product(
                    "Notebook Dell",
                    5499.90m,
                    10),

                new Product(
                    "Mouse Sem Fio",
                    129.90m,
                    25),

                new Product(
                    "Teclado Mecânico",
                    349.90m,
                    15),

                new Product(
                    "Monitor 24 Polegadas",
                    899.90m,
                    8)
            };

            await context.Products.AddRangeAsync(
                products,
                cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}