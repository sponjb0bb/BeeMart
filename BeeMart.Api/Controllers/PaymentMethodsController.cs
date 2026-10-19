using BeeMart.Api.Data;
using BeeMart.Api.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeeMart.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/payment-methods")]
public class PaymentMethodsController : ControllerBase
{
    private readonly BeeMartDbContext _db;
    public PaymentMethodsController(BeeMartDbContext db) => _db = db;

    /// <summary>Active payment methods for the dynamic dropdown (spec 3h).</summary>
    [HttpGet]
    public async Task<ActionResult<List<PaymentMethodDto>>> Get(CancellationToken ct)
    {
        var methods = await _db.PaymentMethods
            .Where(p => p.Stsrc)
            .OrderBy(p => p.IdPaymentMethod)
            .Select(p => new PaymentMethodDto
            {
                IdPaymentMethod = p.IdPaymentMethod,
                MethodName = p.MethodName,
            })
            .ToListAsync(ct);

        return Ok(methods);
    }
}
