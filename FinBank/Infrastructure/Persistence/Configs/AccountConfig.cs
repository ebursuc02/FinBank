using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configs;

public class AccountConfig : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> b)
    {
        b.ToTable("Accounts", "dbo");
        b.HasKey(x => x.Iban).HasName("PK_Accounts");

        b.Property(x => x.Iban).HasColumnName("IBAN").IsRequired().HasMaxLength(34);
        b.Property(x => x.CustomerId).IsRequired();
        b.Property(x => x.IsClosed).HasDefaultValue(false);
        b.Property(x => x.CreatedAt).IsRequired();
        b.Property(x => x.Currency).IsRequired().HasMaxLength(3).IsFixedLength();

        b.HasIndex(x => x.CustomerId).HasDatabaseName("IX_Accounts_CustomerId");

        b.HasOne(x => x.Customer)
            .WithMany(c => c.Accounts)
            .HasForeignKey(x => x.CustomerId)
            .HasConstraintName("FK_Accounts_Customers")
            .OnDelete(DeleteBehavior.NoAction);
    }
}