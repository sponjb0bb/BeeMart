using BeeMart.Api.Data;
using BeeMart.Api.Dtos;
using BeeMart.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeeMart.Api.Services;

namespace BeeMart.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private static readonly string[] ValidRoles = { "Admin", "Cashier" };

    private readonly BeeMartDbContext _db;
    private readonly IPasswordHasher<MsUser> _hasher;
    private readonly IJwtTokenService _jwt;
    private readonly INumberGenerator _numbers;

    public AuthController(BeeMartDbContext db, IPasswordHasher<MsUser> hasher,
        IJwtTokenService jwt, INumberGenerator numbers)
    {
        _db = db; _hasher = hasher; _jwt = jwt; _numbers = numbers;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto, CancellationToken ct)
    {
        if (!ValidRoles.Contains(dto.Role))
            return BadRequest(new { message = "Role must be 'Admin' or 'Cashier'." });

        // Unique username check (spec 2c).
        bool exists = await _db.Users.AnyAsync(u => u.Username == dto.Username, ct);
        if (exists)
            return Conflict(new { message = "username has already exists" });

        var user = new MsUser
        {
            IdUser = await _numbers.NextIdUserAsync(ct),
            Username = dto.Username,
            FullName = dto.FullName,
            Role = dto.Role,
            Stsrc = true,
            Datein = DateTime.Now,
            Userin = "REGISTER",
        };
        // Hash password (spec 2b) — never store plain text.
        user.PasswordHash = _hasher.HashPassword(user, dto.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        return Ok(new AuthResponseDto
        {
            Token = _jwt.CreateToken(user),
            IdUser = user.IdUser,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role,
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto, CancellationToken ct)
    {
        // Must match username + role; password verified via hash (spec 1b-1d).
        var user = await _db.Users.FirstOrDefaultAsync(
            u => u.Username == dto.Username && u.Role == dto.Role && u.Stsrc, ct);

        if (user is null)
            return Unauthorized(new { message = "Invalid login" });

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed)
            return Unauthorized(new { message = "Invalid login" });

        return Ok(new AuthResponseDto
        {
            Token = _jwt.CreateToken(user),
            IdUser = user.IdUser,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role,
        });
    }
}
