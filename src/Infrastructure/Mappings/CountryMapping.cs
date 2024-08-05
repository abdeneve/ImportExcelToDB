using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Mappings;
public class CountryMapping : EntityMapping<Country>
{
    protected override void AppendConfig(EntityTypeBuilder<Country> builder)
    {
        builder.Property(p => p.Code)
            .HasMaxLength(10)
            .IsRequired();
        builder.Property(p => p.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasMany(p => p.Sales)
            .WithOne (p => p.Country)
            .HasForeignKey(p => p.CountryId)
            .HasConstraintName("FK_Country_Sale");
    }
}
