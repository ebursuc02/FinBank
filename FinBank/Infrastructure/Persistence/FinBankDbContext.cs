using Domain;
using Infrastructure.Persistence.Configs;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class FinBankDbContext(DbContextOptions<FinBankDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transfer> Transfers => Set<Transfer>();
    public DbSet<IdempotencyKey> IdempotencyKeys => Set<IdempotencyKey>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfig());
        modelBuilder.ApplyConfiguration(new AccountConfig());
        modelBuilder.ApplyConfiguration(new TransferConfig());
        modelBuilder.ApplyConfiguration(new IdempotencyKeyConfig());
    }
}