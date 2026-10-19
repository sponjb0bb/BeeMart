using BeeMart.Api.Data;
using BeeMart.Api.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeeMart.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly BeeMartDbContext _db;
    public ProductsController(BeeMartDbContext db) => _db = db;

    /// <summary>Search products by name — max 10 active matches (spec 3d).</summary>
    [HttpGet("search")]
    public async Task<ActionResult<List<ProductDto>>> Search([FromQuery] string keyword, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return Ok(new List<ProductDto>());

        var results = await _db.Products
            .Where(p => p.Stsrc && EF.Functions.Like(p.Name, $"%{keyword}%"))
            .OrderBy(p => p.Name)
            .Take(10)
            .Select(p => new ProductDto
            {
                IdProduct = p.IdProduct,
                Name = p.Name,
                Weight = p.Weight,
                Stock = p.Stock,
                Price = p.Price,
                Discount = p.Discount,
            })
            .ToListAsync(ct);

        return Ok(results);
    }
}
