using Domain;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransfersController : ControllerBase
{
    private readonly FinBankDbContext _db;
    public TransfersController(FinBankDbContext db) => _db = db;

    [HttpPost("test-enum")]
    public async Task<IActionResult> TestEnumMapping(CancellationToken ct)
    {
        var transferId = Guid.NewGuid();
        var newTransfer = new Transfer
        {
            TransferId = transferId,
            FromAccountId = "RO71BANK000000000000000001",
            ToAccountId = "RO71BANK000000000000000002",
            CreatedAt = DateTime.UtcNow,
            Amount = 100.00m,
            Currency = "USD",
            Status = TransferStatus.Pending,   // Enum value
            Reason = "Enum test",
            PolicyVersion = "v1"
        };

        await _db.Transfers.AddAsync(newTransfer, ct);
        await _db.SaveChangesAsync(ct);

        // Read back the transfer
        var saved = await _db.Transfers
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TransferId == transferId, ct);

        return Ok(new
        {
            Message = "Transfer created successfully",
            CreatedStatus = newTransfer.Status.ToString(),
            FetchedStatus = saved?.Status.ToString(),
            Matches = saved?.Status == newTransfer.Status
        });
    }
}