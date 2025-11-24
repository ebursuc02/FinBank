using Application.Interfaces.UnitOfWork;
using Domain;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi;

[ApiController]
[Route("api/[controller]")]
public class UowController : ControllerBase
{
    private readonly FinBankDbContext _db;
    private readonly IUnitOfWork _uow;

    public UowController(FinBankDbContext db, IUnitOfWork uow)
    {
        _db = db;
        _uow = uow;
    }

    /// <summary>
    /// Creates a new Customer + Account inside a transaction and COMMITs.
    /// Returns the created IDs and verifies they exist afterward.
    /// </summary>
    [HttpPost("commit")]
    public async Task<IActionResult> Commit(CancellationToken ct)
    {
        var customerId = Guid.NewGuid();
        var iban = $"RO00TEST{Random.Shared.Next(100000, 999999)}";

        await using (var tx = await _uow.BeginTransactionAsync(ct))
        {
            // Insert Customer
            await _db.Customers.AddAsync(new Customer
            {
                CustomerId = customerId,
                CreatedAt = DateTime.UtcNow,
                Name = "UoW Commit Test",
                PhoneNumber = "+4000000000",
                Country = "RODWADAWDAWD",
                Birthday = null,
                Address = "Test Street 1"
            }, ct);

            // Insert Account (PK = IBAN)
            await _db.Accounts.AddAsync(new Account
            {
                IBan = iban,
                CustomerId = customerId,
                CreatedAt = DateTime.UtcNow,
                Currency = "RONDAWDAWDWAWD"
            }, ct);

            await _uow.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }

        // Verify rows persisted
        var existsCustomer = await _db.Customers.AsNoTracking().AnyAsync(x => x.CustomerId == customerId, ct);
        var existsAccount  = await _db.Accounts.AsNoTracking().AnyAsync(x => x.IBan == iban, ct);

        return Ok(new
        {
            Message = "Committed successfully.",
            CustomerId = customerId,
            IBAN = iban,
            ExistsCustomer = existsCustomer,
            ExistsAccount = existsAccount
        });
    }

    /// <summary>
    /// Creates a new Customer + Account inside a transaction and ROLLBACKs.
    /// Returns verification flags showing nothing remained in the DB.
    /// </summary>
    [HttpPost("rollback")]
    public async Task<IActionResult> Rollback(CancellationToken ct)
    {
        var customerId = Guid.NewGuid();
        var iban = $"RO00TEST{Random.Shared.Next(100000, 999999)}";

        await using (var tx = await _uow.BeginTransactionAsync(ct))
        {
            // Insert Customer
            await _db.Customers.AddAsync(new Customer
            {
                CustomerId = customerId,
                CreatedAt = DateTime.UtcNow,
                Name = "UoW Rollback Test",
                PhoneNumber = "+4000000000",
                Country = "RO",
                Birthday = null,
                Address = "Test Street 2"
            }, ct);

            // Insert Account
            await _db.Accounts.AddAsync(new Account
            {
                IBan = iban,
                CustomerId = customerId,
                CreatedAt = DateTime.UtcNow,
                Currency = "RON"
            }, ct);

            // Flush pending changes to the transaction BUT do not commit
            await _uow.SaveChangesAsync(ct);

            // Explicit rollback (Dispose without Commit would also rollback)
            await tx.RollbackAsync(ct);
        }

        // Verify rows do NOT exist after rollback
        var existsCustomer = await _db.Customers.AsNoTracking().AnyAsync(x => x.CustomerId == customerId, ct);
        var existsAccount  = await _db.Accounts.AsNoTracking().AnyAsync(x => x.IBan == iban, ct);

        return Ok(new
        {
            Message = "Rolled back successfully.",
            CustomerId = customerId,
            IBAN = iban,
            ExistsCustomer = existsCustomer, // should be false
            ExistsAccount  = existsAccount   // should be false
        });
    }
}
