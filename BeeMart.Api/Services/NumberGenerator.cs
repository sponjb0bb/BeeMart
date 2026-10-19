using BeeMart.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace BeeMart.Api.Services;

public interface INumberGenerator
{
    Task<string> NextIdUserAsync(CancellationToken ct = default);
    Task<string> NextInvoiceNoAsync(string idUser, DateTime date, CancellationToken ct = default);
}

public class NumberGenerator : INumberGenerator
{
    private readonly BeeMartDbContext _db;
    public NumberGenerator(BeeMartDbContext db) => _db = db;

    /// <summary>Next 5-char zero-padded user id, e.g. "00001". ponytail: max() scan is O(n) but user table is tiny.</summary>
    public async Task<string> NextIdUserAsync(CancellationToken ct = default)
    {
        // Only consider purely-numeric ids so a future non-numeric id can't break parsing.
        var ids = await _db.Users.Select(u => u.IdUser).ToListAsync(ct);
        int max = ids
            .Select(id => int.TryParse(id, out var n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max();
        return (max + 1).ToString("D5");
    }

    /// <summary>
    /// Invoice format: [5-digit userID][ddmmyy][3-digit running number].
    /// Running number is per-user-per-day, 1-based. Caller must invoke inside a DB
    /// transaction so the count-then-insert is race-safe.
    /// </summary>
    public async Task<string> NextInvoiceNoAsync(string idUser, DateTime date, CancellationToken ct = default)
    {
        var dayStart = date.Date;
        var dayEnd = dayStart.AddDays(1);
        int todayCount = await _db.Transactions
            .CountAsync(t => t.IdUser == idUser
                          && t.TransactionDate >= dayStart
                          && t.TransactionDate < dayEnd, ct);
        int running = todayCount + 1;
        return $"{idUser}{dayStart:ddMMyy}{running:D3}";
    }
}
