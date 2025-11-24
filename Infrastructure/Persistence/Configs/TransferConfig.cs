using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configs;

public class TransferConfig : IEntityTypeConfiguration<Transfer>
{
    public void Configure(EntityTypeBuilder<Transfer> b)
    {
        b.ToTable("Transfers", "dbo");
        b.HasKey(x => x.TransferId).HasName("PK_Transfers");
        
        b.Property(x => x.FromAccountId).IsRequired().HasMaxLength(34);
        b.Property(x => x.ToAccountId).IsRequired().HasMaxLength(34);
        b.Property(x => x.ReviewedByEmployeeId).HasColumnName("ReviewedBy");
        b.Property(x => x.CreatedAt).IsRequired();
        b.Property(x => x.Status).IsRequired().HasMaxLength(30);
        b.Property(x => x.Amount).IsRequired().HasColumnType("decimal(18,2)");
        b.Property(x => x.Currency).IsRequired().HasMaxLength(3).IsFixedLength();
        b.Property(x => x.Reason).HasMaxLength(500);
        b.Property(x => x.CompletedAt);
        b.Property(x => x.PolicyVersion).HasMaxLength(20);

        b.HasIndex(x => x.FromAccountId).HasDatabaseName("IX_Transfers_FromAccountId");
        b.HasIndex(x => x.ToAccountId).HasDatabaseName("IX_Transfers_ToAccountId");
        b.HasIndex(x => new { x.Status, x.CreatedAt }).HasDatabaseName("IX_Transfers_StatusCreated");
        
        b.HasOne(x => x.FromAccount)
            .WithMany()
            .HasForeignKey(x => x.FromAccountId)
            .HasConstraintName("FK_Transfers_FromAccount")
            .OnDelete(DeleteBehavior.NoAction);
        
        b.HasOne(x => x.ToAccount)
            .WithMany()
            .HasForeignKey(x => x.ToAccountId)
            .HasConstraintName("FK_Transfers_ToAccount")
            .OnDelete(DeleteBehavior.NoAction);

        b.HasOne(x => x.ReviewedBy)
            .WithMany(e => e.ReviewedTransfers)
            .HasForeignKey(x => x.ReviewedByEmployeeId)
            .HasConstraintName("FK_Transfers_ReviewedBy")
            .OnDelete(DeleteBehavior.NoAction);
    }
}