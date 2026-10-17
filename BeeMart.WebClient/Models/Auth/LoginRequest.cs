using System.ComponentModel.DataAnnotations;

namespace BeeMart.WebClient.Models.Auth;

/// <summary>
/// Login form payload. Sent to the Backend API; never queried against the DB directly.
/// </summary>
public class LoginRequest
{
    [Required(ErrorMessage = "Username wajib diisi.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password wajib diisi.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    // Dropdown values per spec: "Admin" and "Cashier".
    [Required(ErrorMessage = "Role wajib dipilih.")]
    public string Role { get; set; } = "Cashier";
}
