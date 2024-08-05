using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Mappings;
public abstract class EntityMapping<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : Entity
{
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        var tableName = typeof(TEntity).Name;
        builder.ToTable(tableName);

        builder.HasKey(x => x.Id);

        builder.Property(p => p.Id)
                .HasColumnName($"{tableName}Id")
                .ValueGeneratedOnAdd();

        AppendConfig(builder);
    }

    protected abstract void AppendConfig(EntityTypeBuilder<TEntity> builder);
}
