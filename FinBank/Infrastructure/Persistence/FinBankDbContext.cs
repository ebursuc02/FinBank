using Domain;
using Infrastructure.Persistence.Configs;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class FinBankDbContext(DbContextOptions<FinBankDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transfer> Transfers => Set<Transfer>();
    public DbSet<IdempotencyKey> IdempotencyKeys => Set<IdempotencyKey>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CustomerConfig());
        modelBuilder.ApplyConfiguration(new EmployeeConfig());
        modelBuilder.ApplyConfiguration(new AccountConfig());
        modelBuilder.ApplyConfiguration(new TransferConfig());
        modelBuilder.ApplyConfiguration(new IdempotencyKeyConfig());
    }
}