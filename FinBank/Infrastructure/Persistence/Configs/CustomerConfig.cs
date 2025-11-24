using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configs;

public class CustomerConfig : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> b)
    {
        b.ToTable("Customers", "dbo");
        b.HasKey(x => x.CustomerId).HasName("PK_Customers");


        b.Property(x => x.CustomerId).IsRequired();
        b.Property(x => x.CreatedAt).IsRequired();
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.Property(x => x.PhoneNumber).IsRequired().HasMaxLength(50);
        b.Property(x => x.Country).HasMaxLength(100);
        b.Property(x => x.Birthday).HasColumnType("date");
        b.Property(x => x.Address).HasMaxLength(300);
    }
}