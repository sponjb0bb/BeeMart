using System.ComponentModel.DataAnnotations;

namespace BeeMart.Api.Dtos;

public class RegisterRequestDto
{
    [Required] public string Username { get; set; } = string.Empty;
    [Required] public string FullName { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
    [Required] public string Role { get; set; } = string.Empty; // "Admin" | "Cashier"
}

public class LoginRequestDto
{
    [Required] public string Username { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
    [Required] public string Role { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string IdUser { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
