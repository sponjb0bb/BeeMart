using System.Security.Claims;
using BeeMart.Api.Data;
using BeeMart.Api.Dtos;
using BeeMart.Api.Entities;
using BeeMart.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeeMart.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public class TransactionsController : ControllerBase
{
    private const int PageSize = 20; // spec: max 20 per page

    private readonly BeeMartDbContext _db;
    private readonly INumberGenerator _numbers;

    public TransactionsController(BeeMartDbContext db, INumberGenerator numbers)
    {
        _db = db; _numbers = numbers;
    }

    private string CurrentUserId => User.FindFirstValue("idUser") ?? string.Empty;
    private string CurrentRole => User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

    /// <summary>Create a transaction: validate stock, generate invoice, write header+details atomically.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTransactionDto dto, CancellationToken ct)
    {
        if (dto.Items.Count == 0)
            return BadRequest(new { message = "Keranjang tidak boleh kosong." });

        // Payment method must exist and be active.
        bool pmOk = await _db.PaymentMethods.AnyAsync(p => p.IdPaymentMethod == dto.IdPaymentMethod && p.Stsrc, ct);
        if (!pmOk)
            return BadRequest(new { message = "Metode pembayaran tidak valid." });

        // Load all referenced products in one query.
        var ids = dto.Items.Select(i => i.IdProduct).Distinct().ToList();
        var products = await _db.Products.Where(p => ids.Contains(p.IdProduct) && p.Stsrc).ToDictionaryAsync(p => p.IdProduct, ct);

        foreach (var line in dto.Items)
        {
            if (!products.TryGetValue(line.IdProduct, out var prod))
                return BadRequest(new { message = $"Barang tidak ditemukan: {line.IdProduct}" });
            // Stock check (spec 3f).
            if (line.Qty > prod.Stock)
                return BadRequest(new { message = "Jumlah barang yang dimasukkan melebihi sisa stok." });
        }

        var userId = CurrentUserId;
        var now = DateTime.Now;

        // Atomic: invoice numbering (count-based) + stock decrement + header + details.
        await using var trx = await _db.Database.BeginTransactionAsync(ct);

        var invoiceNo = await _numbers.NextInvoiceNoAsync(userId, now, ct);

        var header = new TrTransaction
        {
            IdTransaction = Guid.NewGuid().ToString(),
            InvoiceNo = invoiceNo,
            TransactionDate = now,
            IdUser = userId,
            IdPaymentMethod = dto.IdPaymentMethod,
            TotalPrice = 0m,
            Stsrc = true,
            Datein = now,
            Userin = userId,
        };

        decimal total = 0m;
        foreach (var line in dto.Items)
        {
            var prod = products[line.IdProduct];
            // Authoritative pricing from DB, not client.
            decimal subtotal = Math.Round(prod.Price * line.Qty * (1 - prod.Discount / 100m), 2);
            total += subtotal;

            _db.TransactionDetails.Add(new TrTransactionDetail
            {
                IdTransactionDetail = Guid.NewGuid().ToString(),
                IdTransaction = header.IdTransaction,
                IdProduct = prod.IdProduct,
                Qty = line.Qty,
                Price = prod.Price,
                Discount = prod.Discount,
                SubTotal = subtotal,
                Stsrc = true,
                Datein = now,
                Userin = userId,
            });

            prod.Stock -= line.Qty; // decrement stock
            prod.Dateup = now;
            prod.Userup = userId;
        }

        header.TotalPrice = total;
        _db.Transactions.Add(header);

        await _db.SaveChangesAsync(ct);
        await trx.CommitAsync(ct);

        return Ok(new TransactionResponseDto
        {
            IdTransaction = header.IdTransaction,
            InvoiceNo = header.InvoiceNo,
            TransactionDate = header.TransactionDate,
            TotalPrice = header.TotalPrice,
        });
    }

    /// <summary>
    /// History by date, paginated (max 20), ascending by date/time.
    /// Cashier sees only own transactions; Admin sees all (spec 4 & 5).
    /// </summary>
    [HttpGet("history")]
    public async Task<ActionResult<PagedResultDto<TransactionHistoryDto>>> History(
        [FromQuery] DateTime? date, [FromQuery] int page = 1, CancellationToken ct = default)
    {
        if (date is null)
            return BadRequest(new { message = "Tanggal transaksi wajib dipilih." });
        if (page < 1) page = 1;

        var dayStart = date.Value.Date;
        var dayEnd = dayStart.AddDays(1);

        var query = _db.Transactions
            .Where(t => t.Stsrc && t.TransactionDate >= dayStart && t.TransactionDate < dayEnd);

        // Role scoping: Cashier limited to own rows.
        if (!string.Equals(CurrentRole, "Admin", StringComparison.OrdinalIgnoreCase))
            query = query.Where(t => t.IdUser == CurrentUserId);

        int totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderBy(t => t.TransactionDate) // ascending (spec 4c/5d)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .Select(t => new TransactionHistoryDto
            {
                InvoiceNo = t.InvoiceNo,
                TransactionDate = t.TransactionDate,
                Cashier = t.User!.FullName,
                ItemCount = t.Details.Sum(d => d.Qty),
                PaymentMethod = t.PaymentMethod!.MethodName,
                TotalPrice = t.TotalPrice,
            })
            .ToListAsync(ct);

        return Ok(new PagedResultDto<TransactionHistoryDto>
        {
            Items = items,
            Page = page,
            PageSize = PageSize,
            TotalCount = totalCount,
        });
    }
}
