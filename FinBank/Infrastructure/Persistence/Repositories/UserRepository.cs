using Application.Interfaces.Repositories;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository(FinBankDbContext db) : IUserRepository
{
    public async Task<User?> GetAsync(Guid userId, CancellationToken ct)
        => await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId, ct);
    
    public async Task<User?> GetAccountAsync(string email, CancellationToken ct)
        => await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email, ct);
    
    public async Task AddAsync(User user, CancellationToken ct)
    {
        await db.Users.AddAsync(user, ct);
    }

    public async Task DeleteAsync(Guid commandUserId, CancellationToken cancellationToken)
    {
        var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == commandUserId, cancellationToken);
        if (user is null) return;
        
        db.Users.Remove(user);
    }
}