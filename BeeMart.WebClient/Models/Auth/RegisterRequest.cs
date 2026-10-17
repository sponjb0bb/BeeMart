using System.ComponentModel.DataAnnotations;

namespace BeeMart.WebClient.Models.Auth;

/// <summary>
/// Register form payload. Password is hashed server-side by the API, never here.
/// </summary>
public class RegisterRequest
{
    [Required(ErrorMessage = "Username wajib diisi.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Full Name wajib diisi.")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password wajib diisi.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Re-Password wajib diisi.")]
    [DataType(DataType.Password)]
    [Display(Name = "Re-Password")]
    [Compare(nameof(Password), ErrorMessage = "Password dan Re-Password tidak cocok.")]
    public string RePassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role wajib dipilih.")]
    public string Role { get; set; } = "Cashier";
}
