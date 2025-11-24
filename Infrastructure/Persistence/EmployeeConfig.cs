using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence;

public class EmployeeConfig : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> b)
    {
        b.ToTable("Employees", "dbo");
        b.HasKey(x => x.EmployeeId).HasName("PK_Employee");


        b.Property(x => x.EmployeeId).IsRequired();
        b.Property(x => x.Role).IsRequired().HasMaxLength(50);
        b.Property(x => x.CreatedAt).IsRequired();
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.Property(x => x.PhoneNumber).IsRequired().HasMaxLength(50);
        b.Property(x => x.Country).HasMaxLength(100);
        b.Property(x => x.Birthday).HasColumnType("date");
        b.Property(x => x.Address).HasMaxLength(300);
    }
}