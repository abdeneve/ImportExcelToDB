using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Mappings;
public class CustomerMapping : EntityMapping<Customer>
{
    protected override void AppendConfig(EntityTypeBuilder<Customer> builder)
    {
        builder.Property(p => p.Region)
            .HasMaxLength(10)
            .IsRequired();
        builder.Property(p => p.Name)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(p => p.DisplayName)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasMany(p => p.Sales)
            .WithOne(p => p.Customer)
            .HasForeignKey(p => p.CustomerId)
            .HasConstraintName("FK_Customer_Sale");
    }
}
