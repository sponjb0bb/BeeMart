using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BeeMart.Api.Entities;
using Microsoft.IdentityModel.Tokens;

namespace BeeMart.Api.Services;

public interface IJwtTokenService
{
    string CreateToken(MsUser user);
}

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _config;
    public JwtTokenService(IConfiguration config) => _config = config;

    public string CreateToken(MsUser user)
    {
        var jwt = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.IdUser),
            new Claim("idUser", user.IdUser),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("fullName", user.FullName),
            new Claim(ClaimTypes.Role, user.Role),
        };

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpiryMinutes"] ?? "480")),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
