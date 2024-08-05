using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Mappings;
public class SaleMapping : EntityMapping<Sale>
{
    protected override void AppendConfig(EntityTypeBuilder<Sale> builder)
    {
        builder.Property(p => p.CountryId)
            .IsRequired();
        builder.Property(p => p.CustomerId)
            .IsRequired();
        builder.Property(p => p.StoreCustomerCode)
            .HasMaxLength(20);
        builder.Property(p => p.CustomerItemCode)
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(p => p.Date)
            .IsRequired();
        builder.Property(p => p.Quantity)
            .IsRequired();
        builder.Property(p => p.Amount)
            .HasPrecision(18, 2)
            .IsRequired();
    }
}
