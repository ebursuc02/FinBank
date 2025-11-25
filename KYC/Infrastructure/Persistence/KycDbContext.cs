using Domain;
using Infrastructure.Persistence.Configs;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class KycDbContext(DbContextOptions<KycDbContext> options) : DbContext(options)
{
    public DbSet<CustomerRisk> Customers => Set<CustomerRisk>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CustomerRiskConfig());
    }
}