using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configs;

public class CustomerRiskConfig : IEntityTypeConfiguration<CustomerRisk>
{
    public void Configure(EntityTypeBuilder<CustomerRisk> b)
    {
        b.ToTable("CustomerRisk", "dbo");
        b.HasKey(x => x.Cnp).HasName("PK_CustomerRisk");

        b.Property(x => x.Cnp)
            .HasConversion<string>()          
            .HasMaxLength(13)
            .IsRequired();
        
        b.Property(x => x.RiskStatus)
            .HasConversion<string>()          
            .HasMaxLength(20)
            .IsRequired();
 
    }
}