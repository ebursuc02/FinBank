using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configs;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("Users", "dbo");
        b.HasKey(x => x.UserId).HasName("PK_User");

        b.Property(x => x.Email).IsRequired();
        b.Property(x => x.Password).IsRequired();
        b.Property(x => x.UserId).IsRequired();
        b.Property(x => x.Role).IsRequired().HasMaxLength(50);
        b.Property(x => x.CreatedAt).IsRequired();
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.Property(x => x.PhoneNumber).IsRequired().HasMaxLength(50);
        b.Property(x => x.Country).HasMaxLength(100);
        b.Property(x => x.Birthday).HasColumnType("date");
        b.Property(x => x.Address).HasMaxLength(300);
    }
}