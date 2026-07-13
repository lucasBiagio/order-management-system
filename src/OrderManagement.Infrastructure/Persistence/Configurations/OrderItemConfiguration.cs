using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Persistence.Configurations;

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(orderItem => orderItem.Id);

        builder.Property(orderItem => orderItem.Quantity)
            .IsRequired();

        builder.Property(orderItem => orderItem.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Ignore(orderItem => orderItem.Total);
    }
}