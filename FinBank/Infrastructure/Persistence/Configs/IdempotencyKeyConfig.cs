using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configs;

public class IdempotencyKeyConfig : IEntityTypeConfiguration<IdempotencyKey>
{
    public void Configure(EntityTypeBuilder<IdempotencyKey> b)
    {
        b.ToTable("IdempotencyKeys", "dbo");
        b.HasKey(x => x.IdempotencyKeyValue).HasName("PK_IdempotencyKeys");


        b.Property(x => x.IdempotencyKeyValue).HasColumnName("Key").HasMaxLength(100).IsRequired();
        b.Property(x => x.RequestHash).HasMaxLength(200);
        b.Property(x => x.ResponseJson);
        b.Property(x => x.FirstProcessedAt);

        b.HasIndex(x => x.RequestHash)
            .IsUnique()
            .HasDatabaseName("UX_Idem_RequestHash")
            .HasFilter("[RequestHash] IS NOT NULL");
    }
}