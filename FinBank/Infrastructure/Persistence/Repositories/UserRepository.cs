using Application.DTOs;
using Application.Interfaces.Repositories;
using Domain;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository(FinBankDbContext db) : IUserRepository
{
    public async Task<User?> GetAsync(Guid userId, CancellationToken ct)
        => await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId, ct);
    
    public async Task<User?> GetAccountByEmailAsync(string email, CancellationToken ct)
        => await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email, ct);
    
    public async Task AddAsync(UserDto userDto, CancellationToken ct)
    {
        var user = new User
        {
            UserId = userDto.UserId,
            Email = userDto.Email,
            Name = userDto.Name,
            PhoneNumber = userDto.PhoneNumber,
            Country = userDto.Country,
            Birthday = userDto.Birthday,
            Address = userDto.Address,
            Password = userDto.Password
        };
        await db.Users.AddAsync(user, ct);
        await db.SaveChangesAsync(ct);
    }
    
    public async Task DeleteAsync(Guid commandUserId, CancellationToken cancellationToken)
    {
        var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == commandUserId, cancellationToken);
        if (user is null) return;
        
        db.Users.Remove(user);
    }
}